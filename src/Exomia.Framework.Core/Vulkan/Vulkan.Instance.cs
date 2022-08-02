#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;
using Exomia.Framework.Core.Allocators;
using Microsoft.Extensions.Logging;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkImageTiling;
using static Exomia.Vulkan.Api.Core.VkExtDebugUtils;
using static Exomia.Vulkan.Api.Core.VkExtValidationFeatures;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    /// <summary> Gets suitable depth stencil format. </summary>
    /// <param name="physicalDevice"> The physical device. </param>
    /// <param name="formats">        The formats. </param>
    /// <param name="tiling">         The tiling. </param>
    /// <param name="features">       The features. </param>
    /// <param name="format">         [out] Describes the format to use. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    public static bool GetSuitableDepthStencilFormat(
        VkPhysicalDevice        physicalDevice,
        VkFormat[]              formats,
        VkImageTiling           tiling,
        VkFormatFeatureFlagBits features,
        out VkFormat            format)
    {
        for (int i = 0; i < formats.Length; i++)
        {
            VkFormatProperties2 formatProperties2;
            formatProperties2.sType = VkFormatProperties2.STYPE;
            formatProperties2.pNext = null;

            vkGetPhysicalDeviceFormatProperties2(physicalDevice, format = formats[i], &formatProperties2);

            if (tiling == VK_IMAGE_TILING_LINEAR && (formatProperties2.formatProperties.linearTilingFeatures & features) == features)
            {
                return true;
            }

            if (tiling == VK_IMAGE_TILING_OPTIMAL && (formatProperties2.formatProperties.optimalTilingFeatures & features) == features)
            {
                return true;
            }
        }

        format = VK_FORMAT_UNDEFINED;
        return false;
    }

    private static void GetInstanceExtensionNames(
        string              layerName,
        ICollection<string> availableExtensionNames)
    {
        int   maxByteCount = Encoding.UTF8.GetMaxByteCount(layerName.Length) + 1;
        byte* ntLayerName  = stackalloc byte[maxByteCount];
        layerName.ToNtStringUtf8(ntLayerName, maxByteCount);
        GetInstanceExtensionNames(ntLayerName, availableExtensionNames);
    }

    private static void GetInstanceExtensionNames(
        byte*               layerName,
        ICollection<string> availableExtensionNames)
    {
        uint extensionCount = 0u;
        vkEnumerateInstanceExtensionProperties(layerName, &extensionCount, null)
            .AssertVkResult();

        VkExtensionProperties* pAvailableExtensions = stackalloc VkExtensionProperties[(int)extensionCount];
        vkEnumerateInstanceExtensionProperties(layerName, &extensionCount, pAvailableExtensions)
            .AssertVkResult();

        for (uint i = 0u; i < extensionCount; i++)
        {
            availableExtensionNames.Add(VkHelper.ToString((pAvailableExtensions + i)->extensionName));
        }
    }

    private void CleanupInstance()
    {
        if (_context->Instance != VkInstance.Null)
        {
            if (_context->SurfaceKhr != VkSurfaceKHR.Null)
            {
                vkDestroySurfaceKHR(_context->Instance, _context->SurfaceKhr, null);
                _context->SurfaceKhr = VkSurfaceKHR.Null;
            }

            if (_debugUtilsMessengerConfiguration.IsEnabled && _context->DebugUtilsMessengerExt != VkDebugUtilsMessengerEXT.Null)
            {
                vkDestroyDebugUtilsMessengerEXT(_context->Instance, _context->DebugUtilsMessengerExt, null);
                _context->DebugUtilsMessengerExt = VkDebugUtilsMessengerEXT.Null;
            }

            vkDestroyInstance(_context->Instance, null);
            _context->Instance = VkInstance.Null;
        }
    }

    private bool CreateInstance()
    {
        VkVersion instanceVersion;
        vkEnumerateInstanceVersion(&instanceVersion)
            .AssertVkResult();

        _logger.LogInformation("Vulkan instance version: {0}", instanceVersion.ToString());

        if (instanceVersion < _applicationConfiguration.ApiVersion)
        {
            _logger.LogCritical("The system doesn't support the minimum required vulkan version: {0}", _applicationConfiguration.ApiVersion.ToString());
            return false;
        }
        
        if (!CheckInstanceLayerSupport(_instanceConfiguration.EnabledLayerNames))
        {
            _logger.LogCritical("The system doesn't support the requested instance layers: {0}", string.Join(',', _instanceConfiguration.EnabledLayerNames));
            return false;
        }
        
        if (!CheckInstanceExtensionSupport(_instanceConfiguration.EnabledExtensionNames, _instanceConfiguration.EnabledLayerNames))
        {
            _logger.LogCritical("The system doesn't support the requested instance extensions: {0}", string.Join(',', _instanceConfiguration.EnabledExtensionNames));
            return false;
        }

        if (_debugUtilsMessengerConfiguration.IsEnabled && 
            !_instanceConfiguration.EnabledExtensionNames.Contains(VK_EXT_DEBUG_UTILS_EXTENSION_NAME))
        {
            _instanceConfiguration.EnabledExtensionNames.Add(VK_EXT_DEBUG_UTILS_EXTENSION_NAME);
        }

        void* pNext = _instanceConfiguration.Next;

        if (_instanceConfiguration.ValidationFeatureEnable.Count > 0 || _instanceConfiguration.ValidationFeatureDisable.Count > 0)
        {
            // TODO: needed VK_LAYER_KHRONOS_validation!?
            if (!_instanceConfiguration.EnabledLayerNames.Contains("VK_LAYER_KHRONOS_validation"))
            {
                _instanceConfiguration.EnabledLayerNames.Add("VK_LAYER_KHRONOS_validation");
            }

            if (!_instanceConfiguration.EnabledExtensionNames.Contains(VK_EXT_VALIDATION_FEATURES_EXTENSION_NAME))
            {
                _instanceConfiguration.EnabledExtensionNames.Add(VK_EXT_VALIDATION_FEATURES_EXTENSION_NAME);
            }

            VkValidationFeatureEnableEXT* pValidationFeatureEnableExt = 
                stackalloc VkValidationFeatureEnableEXT[_instanceConfiguration.ValidationFeatureEnable.Count];

            for (int i = 0; i < _instanceConfiguration.ValidationFeatureEnable.Count; i++)
            {
                *(pValidationFeatureEnableExt + i) = _instanceConfiguration.ValidationFeatureEnable[i];
            }

            VkValidationFeatureDisableEXT* pValidationFeatureDisableExt =
                stackalloc VkValidationFeatureDisableEXT[_instanceConfiguration.ValidationFeatureDisable.Count];

            for (int i = 0; i < _instanceConfiguration.ValidationFeatureDisable.Count; i++)
            {
                *(pValidationFeatureDisableExt + i) = _instanceConfiguration.ValidationFeatureDisable[i];
            }

            VkValidationFeaturesEXT validationFeatures;
            validationFeatures.sType                          = VkValidationFeaturesEXT.STYPE;
            validationFeatures.pNext                          = pNext;
            validationFeatures.enabledValidationFeatureCount  = (uint)_instanceConfiguration.ValidationFeatureEnable.Count;
            validationFeatures.pEnabledValidationFeatures     = pValidationFeatureEnableExt;
            validationFeatures.disabledValidationFeatureCount = (uint)_instanceConfiguration.ValidationFeatureDisable.Count;
            validationFeatures.pDisabledValidationFeatures    = pValidationFeatureDisableExt;

            pNext = &validationFeatures;
        }

        VkApplicationInfo applicationInfo;
        applicationInfo.sType              = VkApplicationInfo.STYPE;
        applicationInfo.pNext              = null;
        applicationInfo.pApplicationName   = Allocator.AllocateNtString(_applicationConfiguration.AppName);
        applicationInfo.applicationVersion = _applicationConfiguration.ApplicationVersion;
        applicationInfo.pEngineName        = Allocator.AllocateNtString(_applicationConfiguration.EngineName);
        applicationInfo.engineVersion      = _applicationConfiguration.EngineVersion;
        applicationInfo.apiVersion         = _applicationConfiguration.ApiVersion;

        byte** ppEnabledLayerNames = stackalloc byte*[_instanceConfiguration.EnabledLayerNames.Count];
        for (int i = 0; i < _instanceConfiguration.EnabledLayerNames.Count; i++)
        {
            *(ppEnabledLayerNames + i) = Allocator.AllocateNtString(_instanceConfiguration.EnabledLayerNames[i]);
        }

        byte** ppEnabledExtensionNames = stackalloc byte*[_instanceConfiguration.EnabledExtensionNames.Count];
        for (int i = 0; i < _instanceConfiguration.EnabledExtensionNames.Count; i++)
        {
            *(ppEnabledExtensionNames + i) = Allocator.AllocateNtString(_instanceConfiguration.EnabledExtensionNames[i]);
        }

        VkInstanceCreateInfo instanceCreateInfo;
        instanceCreateInfo.sType                   = VkInstanceCreateInfo.STYPE;
        instanceCreateInfo.pNext                   = pNext;
        instanceCreateInfo.flags                   = _instanceConfiguration.Flags;
        instanceCreateInfo.pApplicationInfo        = &applicationInfo;
        instanceCreateInfo.enabledLayerCount       = (uint)_instanceConfiguration.EnabledLayerNames.Count;
        instanceCreateInfo.ppEnabledLayerNames     = ppEnabledLayerNames;
        instanceCreateInfo.enabledExtensionCount   = (uint)_instanceConfiguration.EnabledExtensionNames.Count;
        instanceCreateInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;

        try
        {
            vkCreateInstance(&instanceCreateInfo, null, &_context->Instance)
                .AssertVkResult();
            
            _context->Version = _applicationConfiguration.ApiVersion;

            return true;
        }
        finally
        {
            for (int i = 0; i < _instanceConfiguration.EnabledExtensionNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledExtensionNames + i));
            }

            for (int i = 0; i < _instanceConfiguration.EnabledLayerNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledLayerNames + i));
            }

            Allocator.FreeNtString(applicationInfo.pEngineName);
            Allocator.FreeNtString(applicationInfo.pApplicationName);
        }
    }

    private bool InitializeInstance()
    {
        if (!CreateInstance())
        {
            _logger.LogCritical("{method} failed!", nameof(CreateInstance));
            return false;
        }

        if (_debugUtilsMessengerConfiguration.IsEnabled)
        {
            VkExtDebugUtils.Load(_context->Instance);
            SetupDebugCallback(_debugUtilsMessengerConfiguration);
        }

        VkKhrGetSurfaceCapabilities2.Load(_context->Instance);
        VkKhrSurface.Load(_context->Instance);

        if (_surfaceConfiguration.CreateSurface != null && !_surfaceConfiguration.CreateSurface(_context))
        {
            _logger.LogCritical("{method} failed!", nameof(_surfaceConfiguration.CreateSurface));
            return false;
        }

        if (!PickBestPhysicalDevice())
        {
            _logger.LogCritical("{method} failed!", nameof(PickBestPhysicalDevice));
            return false;
        }

        return true;
    }

    private bool CheckInstanceLayerSupport(IEnumerable<string> requiredInstanceLayers)
    {
        uint layerCount = 0u;
        vkEnumerateInstanceLayerProperties(&layerCount, null)
            .AssertVkResult();

        VkLayerProperties* pAvailableLayers = stackalloc VkLayerProperties[(int)layerCount];
        vkEnumerateInstanceLayerProperties(&layerCount, pAvailableLayers)
            .AssertVkResult();

        string[] availableLayerNames = new string[layerCount];
        for (uint i = 0u; i < layerCount; i++)
        {
            availableLayerNames[i] = VkHelper.ToString((pAvailableLayers + i)->layerName);
        }

        bool allFound = true;
        foreach (string instanceLayer in requiredInstanceLayers)
        {
            bool found = availableLayerNames.Any(name => name == instanceLayer);
            if (!found)
            {
                _logger.LogWarning("Instance layer '{0}' not found!", instanceLayer);
            }
            allFound &= found;
        }

        return allFound;
    }

    private bool CheckInstanceExtensionSupport(
        IEnumerable<string> requiredInstanceExtensions,
        IEnumerable<string> usedInstanceLayers)
    {
        List<string> availableExtensionNames = new List<string>(8);
        GetInstanceExtensionNames((byte*)null, availableExtensionNames);
        foreach (string layerName in usedInstanceLayers)
        {
            GetInstanceExtensionNames(layerName, availableExtensionNames);
        }

        bool allFound = true;
        foreach (string deviceExtension in requiredInstanceExtensions)
        {
            bool found = availableExtensionNames.Any(name => name == deviceExtension);
            if (!found)
            {
                _logger.LogWarning("Instance extension '{0}' not found!", deviceExtension);
            }
            allFound &= found;
        }

        return allFound;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;
using Microsoft.Extensions.Logging;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkImageTiling;

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

    private static void CleanupInstance(VkContext* context)
    {
        if (context->Instance != VkInstance.Null)
        {
            if (context->SurfaceKhr != VkSurfaceKHR.Null)
            {
                vkDestroySurfaceKHR(context->Instance, context->SurfaceKhr, null);
                context->SurfaceKhr = VkSurfaceKHR.Null;
            }

#if DEBUG
            if (context->DebugUtilsMessengerExt != VkDebugUtilsMessengerEXT.Null)
            {
                vkDestroyDebugUtilsMessengerEXT(context->Instance, context->DebugUtilsMessengerExt, null);
                context->DebugUtilsMessengerExt = VkDebugUtilsMessengerEXT.Null;
            }
#endif

            vkDestroyInstance(context->Instance, null);
            context->Instance = VkInstance.Null;
        }
    }

    private bool CreateInstance(
        ApplicationConfiguration applicationConfiguration,
        InstanceConfiguration    instanceConfiguration)
    {
        VkVersion instanceVersion;
        vkEnumerateInstanceVersion(&instanceVersion)
            .AssertVkResult();

        _logger.LogInformation("Vulkan instance version: {0}", instanceVersion.ToString());

        if (instanceVersion < applicationConfiguration.ApiVersion)
        {
            _logger.LogCritical("The system doesn't support the minimum required vulkan version: {0}", applicationConfiguration.ApiVersion.ToString());
            return false;
        }

        if (!CheckInstanceLayerSupport(instanceConfiguration.EnabledLayerNames))
        {
            _logger.LogCritical("The system doesn't support the requested instance layers: {0}", string.Join(',', instanceConfiguration.EnabledLayerNames));
            return false;
        }

        if (!CheckInstanceExtensionSupport(instanceConfiguration.EnabledExtensionNames, instanceConfiguration.EnabledLayerNames))
        {
            _logger.LogCritical("The system doesn't support the requested instance extensions: {0}", string.Join(',', instanceConfiguration.EnabledExtensionNames));
            return false;
        }

        VkApplicationInfo applicationInfo;
        applicationInfo.sType              = VkApplicationInfo.STYPE;
        applicationInfo.pNext              = null;
        applicationInfo.pApplicationName   = Allocator.AllocateNtString(applicationConfiguration.AppName);
        applicationInfo.applicationVersion = applicationConfiguration.ApplicationVersion;
        applicationInfo.pEngineName        = Allocator.AllocateNtString(applicationConfiguration.EngineName);
        applicationInfo.engineVersion      = applicationConfiguration.EngineVersion;
        applicationInfo.apiVersion         = applicationConfiguration.ApiVersion;

        byte** ppEnabledLayerNames = stackalloc byte*[instanceConfiguration.EnabledLayerNames.Count];
        for (int i = 0; i < instanceConfiguration.EnabledLayerNames.Count; i++)
        {
            *(ppEnabledLayerNames + i) = Allocator.AllocateNtString(instanceConfiguration.EnabledLayerNames[i]);
        }

        byte** ppEnabledExtensionNames = stackalloc byte*[instanceConfiguration.EnabledExtensionNames.Count];
        for (int i = 0; i < instanceConfiguration.EnabledExtensionNames.Count; i++)
        {
            *(ppEnabledExtensionNames + i) = Allocator.AllocateNtString(instanceConfiguration.EnabledExtensionNames[i]);
        }

        VkInstanceCreateInfo instanceCreateInfo;
        instanceCreateInfo.sType                   = VkInstanceCreateInfo.STYPE;
        instanceCreateInfo.pNext                   = null;
        instanceCreateInfo.flags                   = instanceConfiguration.Flags;
        instanceCreateInfo.pApplicationInfo        = &applicationInfo;
        instanceCreateInfo.enabledLayerCount       = (uint)instanceConfiguration.EnabledLayerNames.Count;
        instanceCreateInfo.ppEnabledLayerNames     = ppEnabledLayerNames;
        instanceCreateInfo.enabledExtensionCount   = (uint)instanceConfiguration.EnabledExtensionNames.Count;
        instanceCreateInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;

        try
        {
            vkCreateInstance(&instanceCreateInfo, null, &_context->Instance)
                .AssertVkResult();

            return true;
        }
        finally
        {
            for (int i = 0; i < instanceConfiguration.EnabledExtensionNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledExtensionNames + i));
            }

            for (int i = 0; i < instanceConfiguration.EnabledLayerNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledLayerNames + i));
            }

            Allocator.FreeNtString(applicationInfo.pEngineName);
            Allocator.FreeNtString(applicationInfo.pApplicationName);
        }
    }

    private bool InitializeInstance(
        ApplicationConfiguration         applicationConfiguration,
        InstanceConfiguration            instanceConfiguration,
        DebugUtilsMessengerConfiguration debugUtilsMessengerConfiguration,
        SurfaceConfiguration             surfaceConfiguration,
        PhysicalDeviceConfiguration      physicalDeviceConfiguration,
        DeviceConfiguration              deviceConfiguration)
    {
        if (!CreateInstance(applicationConfiguration, instanceConfiguration))
        {
            _logger.LogCritical("{method} failed!", nameof(CreateInstance));
            return false;
        }

#if DEBUG
        SetupDebugCallback(debugUtilsMessengerConfiguration);
#endif

        VkKhrGetSurfaceCapabilities2.Load(_context->Instance);
        VkKhrSurface.Load(_context->Instance);

        if (surfaceConfiguration.CreateSurface != null && !surfaceConfiguration.CreateSurface(_context))
        {
            _logger.LogCritical("{method} failed!", nameof(surfaceConfiguration.CreateSurface));
            return false;
        }

        if (!PickBestPhysicalDevice(physicalDeviceConfiguration, deviceConfiguration))
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
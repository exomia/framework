#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.Logging;
using static Exomia.Vulkan.Api.Core.VkQueueFlagBits;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        /// <summary> Searches for the first memory type index. </summary>
        /// <param name="physicalDevice"> The physical device. </param>
        /// <param name="typeFilter">     A filter specifying the type. </param>
        /// <param name="properties">     The properties. </param>
        /// <returns> The found memory type index. </returns>
        /// <exception cref="VulkanException"> Thrown when a Vulkan error condition occurs. </exception>
        public static uint FindMemoryTypeIndex(VkPhysicalDevice physicalDevice, uint typeFilter, VkMemoryPropertyFlagBits properties)
        {
            VkPhysicalDeviceMemoryProperties2 physicalDeviceMemoryProperties2;
            physicalDeviceMemoryProperties2.sType = VkPhysicalDeviceMemoryProperties2.STYPE;
            physicalDeviceMemoryProperties2.pNext = null;
            vkGetPhysicalDeviceMemoryProperties2(physicalDevice, &physicalDeviceMemoryProperties2);

            for (int i = 0; i < physicalDeviceMemoryProperties2.memoryProperties.memoryTypeCount; i++)
            {
                if ((typeFilter & (1 << i)) != 0 &&
                    (physicalDeviceMemoryProperties2.memoryProperties.memoryTypes[i].propertyFlags & properties) == properties)
                {
                    return (uint)i;
                }
            }

            throw new VulkanException("Failed to find suitable memory type!");
        }

        private static void GetDeviceExtensionNames(
            in VkPhysicalDevice physicalDevice,
            string              layerName,
            ICollection<string> availableExtensionNames)
        {
            int   maxByteCount = Encoding.UTF8.GetMaxByteCount(layerName.Length) + 1;
            byte* ntLayerName  = stackalloc byte[maxByteCount];
            layerName.ToNtStringUtf8(ntLayerName, maxByteCount);
            GetDeviceExtensionNames(in physicalDevice, ntLayerName, availableExtensionNames);
        }

        private static void GetDeviceExtensionNames(
            in VkPhysicalDevice physicalDevice,
            byte*               layerName,
            ICollection<string> availableExtensionNames)
        {
            uint extensionCount = 0;
            vkEnumerateDeviceExtensionProperties(physicalDevice, layerName, &extensionCount, null)
                .AssertVkResult();

            VkExtensionProperties* pAvailableExtensions = stackalloc VkExtensionProperties[(int)extensionCount];
            vkEnumerateDeviceExtensionProperties(physicalDevice, layerName, &extensionCount, pAvailableExtensions)
                .AssertVkResult();

            for (uint i = 0; i < extensionCount; i++)
            {
                availableExtensionNames.Add(VkHelper.ToString((pAvailableExtensions + i)->extensionName));
            }
        }

        private static bool PickBestQueueFamily(VkPhysicalDevice physicalDevice, out uint queueFamilyIndex, out uint maxQueueCount)
        {
            uint amountOfQueueFamilies = 0u;
            vkGetPhysicalDeviceQueueFamilyProperties2(physicalDevice, &amountOfQueueFamilies, null);
            VkQueueFamilyProperties2* pQueueFamilyProperties2 = stackalloc VkQueueFamilyProperties2[(int)amountOfQueueFamilies];
            for (uint a = 0u; a < amountOfQueueFamilies; a++)
            {
                (pQueueFamilyProperties2 + a)->sType = VkQueueFamilyProperties2.STYPE;
                (pQueueFamilyProperties2 + a)->pNext = null;
            }
            vkGetPhysicalDeviceQueueFamilyProperties2(physicalDevice, &amountOfQueueFamilies, pQueueFamilyProperties2);

            for (uint i = 0u; i < amountOfQueueFamilies; i++)
            {
                if ((pQueueFamilyProperties2 + i)->queueFamilyProperties.queueCount > 0
                    && ((pQueueFamilyProperties2 + i)->queueFamilyProperties.queueFlags & VK_QUEUE_GRAPHICS_BIT) == VK_QUEUE_GRAPHICS_BIT)
                {
                    queueFamilyIndex = i;
                    maxQueueCount    = (pQueueFamilyProperties2 + i)->queueFamilyProperties.queueCount;
                    return true;
                }
            }

            queueFamilyIndex = uint.MaxValue;
            maxQueueCount    = 0u;
            return false;
        }

        private bool PickBestPhysicalDevice(VkContext* context, PhysicalDeviceConfiguration physicalDeviceConfiguration, DeviceConfiguration deviceConfiguration)
        {
            uint physicalDeviceCount;
            vkEnumeratePhysicalDevices(context->Instance, &physicalDeviceCount, null)
                .AssertVkResult();

            VkPhysicalDevice* pPhysicalDevices = stackalloc VkPhysicalDevice[(int)physicalDeviceCount];
            vkEnumeratePhysicalDevices(context->Instance, &physicalDeviceCount, pPhysicalDevices)
                .AssertVkResult();

            for (uint i = 0; i < physicalDeviceCount; i++)
            {
                VkPhysicalDeviceProperties2 physicalDeviceProperties2;
                physicalDeviceProperties2.sType = VkPhysicalDeviceProperties2.STYPE;
                physicalDeviceProperties2.pNext = null;
                vkGetPhysicalDeviceProperties2(*(pPhysicalDevices + i), &physicalDeviceProperties2);

                if (physicalDeviceProperties2.properties.apiVersion < physicalDeviceConfiguration.RequiredMinimumVkApiVersion) { continue; }
                if (physicalDeviceProperties2.properties.deviceType != physicalDeviceConfiguration.RequiredPhysicalDeviceType) { continue; }

                context->SupportedSampleCountFlags = physicalDeviceProperties2.properties.limits.framebufferColorSampleCounts;

                if (CheckDeviceLayerSupport(*(pPhysicalDevices + i), deviceConfiguration.EnabledLayerNames) &&
                    CheckDeviceExtensionSupport(*(pPhysicalDevices + i), deviceConfiguration.EnabledExtensionNames, deviceConfiguration.EnabledLayerNames) &&
                    PickBestQueueFamily(*(pPhysicalDevices + i), out uint queueFamilyIndex, out uint maxQueueCount))
                {
                    VkBool32 supported = VkBool32.False;
                    vkGetPhysicalDeviceSurfaceSupportKHR(*(pPhysicalDevices + i), queueFamilyIndex, context->SurfaceKhr, &supported)
                        .AssertVkResult();
                    if (supported)
                    {
                        context->PhysicalDevice   = *(pPhysicalDevices + i);
                        context->QueueFamilyIndex = queueFamilyIndex;
                        context->MaxQueueCount    = maxQueueCount;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool CheckDeviceLayerSupport(in VkPhysicalDevice physicalDevice, IEnumerable<string> requiredDeviceLayers)
        {
            uint layerCount = 0;
            vkEnumerateDeviceLayerProperties(physicalDevice, &layerCount, null)
                .AssertVkResult();

            VkLayerProperties* pAvailableLayers = stackalloc VkLayerProperties[(int)layerCount];
            vkEnumerateDeviceLayerProperties(physicalDevice, &layerCount, pAvailableLayers)
                .AssertVkResult();

            string[] availableLayerNames = new string[layerCount];
            for (uint i = 0; i < layerCount; i++)
            {
                availableLayerNames[i] = VkHelper.ToString((pAvailableLayers + i)->layerName);
            }

            bool allFound = true;
            foreach (string deviceExtension in requiredDeviceLayers)
            {
                bool found = availableLayerNames.Any(name => name == deviceExtension);
                if (!found)
                {
                    _logger.Log(LogLevel.Warning, null, "Device layer '{0}' not found!", deviceExtension);
                }
                allFound &= found;
            }

            return allFound;
        }

        private bool CheckDeviceExtensionSupport(
            in VkPhysicalDevice physicalDevice,
            IEnumerable<string> requiredDeviceExtensions,
            IEnumerable<string> usedDeviceLayers)
        {
            List<string> availableExtensionNames = new List<string>(8);
            GetDeviceExtensionNames(in physicalDevice, (byte*)null, availableExtensionNames);
            foreach (string layerName in usedDeviceLayers)
            {
                GetDeviceExtensionNames(in physicalDevice, layerName, availableExtensionNames);
            }

            bool allFound = true;
            foreach (string deviceExtension in requiredDeviceExtensions)
            {
                bool found = availableExtensionNames.Any(name => name == deviceExtension);
                if (!found)
                {
                    _logger.Log(LogLevel.Warning, null, "Device extension '{0}' not found!", deviceExtension);
                }
                allFound &= found;
            }

            return allFound;
        }
    }
}
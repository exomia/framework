#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    private static bool CreateDevice(VkContext* context, DeviceConfiguration configuration)
    {
        uint additionalDeviceQueueCreateInfoCount = 0u;

        if (configuration.CreateAdditionalDeviceQueueCreateInfos != null)
        {
            configuration.CreateAdditionalDeviceQueueCreateInfos(context, &additionalDeviceQueueCreateInfoCount, null);
        }

        VkDeviceQueueCreateInfo* pDeviceQueueCreateInfos = stackalloc VkDeviceQueueCreateInfo[(int)(1u + additionalDeviceQueueCreateInfoCount)];

        float* pQueuePriorities = stackalloc float[(int)context->MaxQueueCount];
        for (uint i = 0; i < context->MaxQueueCount; i++)
        {
            *(pQueuePriorities + i) = 1.0f;
        }

        pDeviceQueueCreateInfos->sType            = VkDeviceQueueCreateInfo.STYPE;
        pDeviceQueueCreateInfos->pNext            = null;
        pDeviceQueueCreateInfos->flags            = 0u;
        pDeviceQueueCreateInfos->queueFamilyIndex = context->QueueFamilyIndex;
        pDeviceQueueCreateInfos->queueCount       = context->MaxQueueCount;
        pDeviceQueueCreateInfos->pQueuePriorities = pQueuePriorities;

        if (configuration.CreateAdditionalDeviceQueueCreateInfos != null)
        {
            configuration.CreateAdditionalDeviceQueueCreateInfos(context, &additionalDeviceQueueCreateInfoCount, pDeviceQueueCreateInfos + 1);
        }

        byte** ppEnabledLayerNames = stackalloc byte*[configuration.EnabledLayerNames.Count];
        for (int i = 0; i < configuration.EnabledLayerNames.Count; i++)
        {
            *(ppEnabledLayerNames + i) = Allocator.AllocateNtString(configuration.EnabledLayerNames[i]);
        }

        byte** ppEnabledExtensionNames = stackalloc byte*[configuration.EnabledExtensionNames.Count];
        for (int i = 0; i < configuration.EnabledExtensionNames.Count; i++)
        {
            *(ppEnabledExtensionNames + i) = Allocator.AllocateNtString(configuration.EnabledExtensionNames[i]);
        }

        VkDeviceCreateInfo deviceCreateInfo;
        deviceCreateInfo.sType                   = VkDeviceCreateInfo.STYPE;
        deviceCreateInfo.pNext                   = configuration.Next;
        deviceCreateInfo.flags                   = configuration.Flags;
        deviceCreateInfo.queueCreateInfoCount    = 1u + additionalDeviceQueueCreateInfoCount;
        deviceCreateInfo.pQueueCreateInfos       = pDeviceQueueCreateInfos;
        deviceCreateInfo.enabledLayerCount       = (uint)configuration.EnabledLayerNames.Count;
        deviceCreateInfo.ppEnabledLayerNames     = ppEnabledLayerNames;
        deviceCreateInfo.enabledExtensionCount   = (uint)configuration.EnabledExtensionNames.Count;
        deviceCreateInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;
        deviceCreateInfo.pEnabledFeatures        = null;

        VkResult result = vkCreateDevice(context->PhysicalDevice, &deviceCreateInfo, null, &context->Device);

        for (int i = 0; i < configuration.EnabledExtensionNames.Count; i++)
        {
            Allocator.FreeNtString(*(ppEnabledExtensionNames + i));
        }

        for (int i = 0; i < configuration.EnabledLayerNames.Count; i++)
        {
            Allocator.FreeNtString(*(ppEnabledLayerNames + i));
        }

        result.AssertVkResult();

        return true;
    }
}
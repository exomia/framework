#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;
using static Exomia.Vulkan.Api.Core.VkCommandPoolCreateFlagBits;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    private static void CreateDevice(
        VkContext* context,
        DeviceConfiguration configuration, 
        QueueConfiguration queueConfiguration)
    {
        uint additionalDeviceQueueCreateInfoCount = 0u;

        if (configuration.CreateAdditionalDeviceQueueCreateInfos != null)
        {
            configuration.CreateAdditionalDeviceQueueCreateInfos(context, &additionalDeviceQueueCreateInfoCount, null);
        }

        // we will add 1 to the requested queue count for internal usage;
        // if the max queue count will be exceeded -> max queue count value will be used instead.
        context->QueuesCount = Math.Min(queueConfiguration.Count + 1u, context->MaxQueueCount);
        context->Queues      = Allocator.Allocate<VkQueue>(context->QueuesCount);

        float* pQueuePriorities = stackalloc float[(int)context->QueuesCount];
        *pQueuePriorities = 1.0f; // internal usage queue priority
        for (uint i = 1; i < context->QueuesCount; i++)
        {
            *(pQueuePriorities + i) = 0.5f;
        }

        VkDeviceQueueCreateInfo* pDeviceQueueCreateInfos = stackalloc VkDeviceQueueCreateInfo[(int)(1u + additionalDeviceQueueCreateInfoCount)];
        pDeviceQueueCreateInfos->sType            = VkDeviceQueueCreateInfo.STYPE;
        pDeviceQueueCreateInfos->pNext            = null;
        pDeviceQueueCreateInfos->flags            = 0u;
        pDeviceQueueCreateInfos->queueFamilyIndex = context->QueueFamilyIndex;
        pDeviceQueueCreateInfos->queueCount       = context->QueuesCount;
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

        void* pNext = configuration.Next;
        if (context->Version >= VkVersion.VulkanApiVersion13)
        {
            VkPhysicalDeviceVulkan13Features physicalDeviceVulkan13Features = new();
            physicalDeviceVulkan13Features.sType = VkPhysicalDeviceVulkan13Features.STYPE;
            physicalDeviceVulkan13Features.pNext = pNext;

            if (configuration.SetPhysicalDeviceVulkan13Features != null)
                configuration.SetPhysicalDeviceVulkan13Features(&physicalDeviceVulkan13Features);
            pNext = &physicalDeviceVulkan13Features;
        }
        if (context->Version >= VkVersion.VulkanApiVersion12)
        {
            VkPhysicalDeviceVulkan12Features physicalDeviceVulkan12Features;
            physicalDeviceVulkan12Features.sType             = VkPhysicalDeviceVulkan12Features.STYPE;
            physicalDeviceVulkan12Features.pNext             = pNext;
            // setting defaults 
            // TODO: add extension in case VulkanApiVersion12 is not supported!?
            physicalDeviceVulkan12Features.timelineSemaphore = VkBool32.True;

            if (configuration.SetPhysicalDeviceVulkan12Features != null)
                configuration.SetPhysicalDeviceVulkan12Features(&physicalDeviceVulkan12Features);
            pNext = &physicalDeviceVulkan12Features;
        }
        if (context->Version >= VkVersion.VulkanApiVersion11)
        {
            VkPhysicalDeviceVulkan11Features physicalDeviceVulkan11Features;
            physicalDeviceVulkan11Features.sType = VkPhysicalDeviceVulkan11Features.STYPE;
            physicalDeviceVulkan11Features.pNext = pNext;

            if (configuration.SetPhysicalDeviceVulkan11Features != null)
                configuration.SetPhysicalDeviceVulkan11Features(&physicalDeviceVulkan11Features); 
            pNext = &physicalDeviceVulkan11Features;
        }

        VkDeviceCreateInfo deviceCreateInfo;
        deviceCreateInfo.sType                   = VkDeviceCreateInfo.STYPE;
        deviceCreateInfo.pNext                   = pNext;
        deviceCreateInfo.flags                   = configuration.Flags;
        deviceCreateInfo.queueCreateInfoCount    = 1u + additionalDeviceQueueCreateInfoCount;
        deviceCreateInfo.pQueueCreateInfos       = pDeviceQueueCreateInfos;
        deviceCreateInfo.enabledLayerCount       = (uint)configuration.EnabledLayerNames.Count;
        deviceCreateInfo.ppEnabledLayerNames     = ppEnabledLayerNames;
        deviceCreateInfo.enabledExtensionCount   = (uint)configuration.EnabledExtensionNames.Count;
        deviceCreateInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;
        deviceCreateInfo.pEnabledFeatures        = null;

        try
        {
            vkCreateDevice(context->PhysicalDevice, &deviceCreateInfo, null, &context->Device)
                .AssertVkResult();

            RetrieveDeviceQueue(context, queueConfiguration);

            CreateCommandPool(context->Device, context->QueueFamilyIndex, &context->CommandPool,           VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT);
            CreateCommandPool(context->Device, context->QueueFamilyIndex, &context->ShortLivedCommandPool, VK_COMMAND_POOL_CREATE_TRANSIENT_BIT);

            VkKhrSwapchain.Load(context->Device);
        }
        finally
        {
            for (int i = 0; i < configuration.EnabledExtensionNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledExtensionNames + i));
            }

            for (int i = 0; i < configuration.EnabledLayerNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledLayerNames + i));
            }
        }
    }

    private void DestroyDevice(VkContext* context)
    {
        if (context->Device != VkDevice.Null)
        {
            vkDeviceWaitIdle(context->Device)
                .AssertVkResult();

            if (context->ShortLivedCommandPool != VkCommandPool.Null)
            {
                vkDestroyCommandPool(context->Device, context->ShortLivedCommandPool, null);
                context->ShortLivedCommandPool = VkCommandPool.Null;
            }

            if (context->CommandPool != VkCommandPool.Null)
            {
                vkDestroyCommandPool(context->Device, context->CommandPool, null);
                Context->CommandPool = VkCommandPool.Null;
            }

            Allocator.Free<VkQueue>(context->Queues - 1u, context->QueuesCount + 1u);

            vkDestroyDevice(context->Device, null);
            context->Device = VkDevice.Null;
        }
    }
}
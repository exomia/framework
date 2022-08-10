#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    /// <summary> DeviceWaitIdle - Wait for a device to become idle </summary>
    public void DeviceWaitIdle()
    {
        if (_context->Device != VkDevice.Null)
        {
            vkDeviceWaitIdle(_context->Device)
               .AssertVkResult();
        }
    }

    private void CreateDevice()
    {
        uint additionalDeviceQueueCreateInfoCount = 0u;

        if (_deviceConfiguration.CreateAdditionalDeviceQueueCreateInfos != null)
        {
            _deviceConfiguration.CreateAdditionalDeviceQueueCreateInfos(_context, &additionalDeviceQueueCreateInfoCount, null);
        }

        // we will add 1 to the requested queue count for internal usage;
        // if the max queue count will be exceeded -> max queue count value will be used instead.
        _context->QueuesCount = Math.Min(_queueConfiguration.Count + 1u, _context->MaxQueueCount);
        _context->Queues      = Allocator.Allocate<VkQueue>(_context->QueuesCount);

        float* pQueuePriorities = stackalloc float[(int)_context->QueuesCount];
        *pQueuePriorities = 1.0f; // internal usage queue priority
        for (uint i = 1; i < _context->QueuesCount; i++)
        {
            *(pQueuePriorities + i) = 0.5f;
        }

        VkDeviceQueueCreateInfo* pDeviceQueueCreateInfos = stackalloc VkDeviceQueueCreateInfo[(int)(1u + additionalDeviceQueueCreateInfoCount)];
        pDeviceQueueCreateInfos->sType            = VkDeviceQueueCreateInfo.STYPE;
        pDeviceQueueCreateInfos->pNext            = null;
        pDeviceQueueCreateInfos->flags            = 0u;
        pDeviceQueueCreateInfos->queueFamilyIndex = _context->QueueFamilyIndex;
        pDeviceQueueCreateInfos->queueCount       = _context->QueuesCount;
        pDeviceQueueCreateInfos->pQueuePriorities = pQueuePriorities;

        if (_deviceConfiguration.CreateAdditionalDeviceQueueCreateInfos != null)
        {
            _deviceConfiguration.CreateAdditionalDeviceQueueCreateInfos(_context, &additionalDeviceQueueCreateInfoCount, pDeviceQueueCreateInfos + 1);
        }

        byte** ppEnabledLayerNames = stackalloc byte*[_deviceConfiguration.EnabledLayerNames.Count];
        for (int i = 0; i < _deviceConfiguration.EnabledLayerNames.Count; i++)
        {
            *(ppEnabledLayerNames + i) = Allocator.AllocateNtString(_deviceConfiguration.EnabledLayerNames[i]);
        }

        byte** ppEnabledExtensionNames = stackalloc byte*[_deviceConfiguration.EnabledExtensionNames.Count];
        for (int i = 0; i < _deviceConfiguration.EnabledExtensionNames.Count; i++)
        {
            *(ppEnabledExtensionNames + i) = Allocator.AllocateNtString(_deviceConfiguration.EnabledExtensionNames[i]);
        }

        void* pNext = _deviceConfiguration.Next;
        if (_context->Version >= VkVersion.VulkanApiVersion13)
        {
            if (_deviceConfiguration.SetPhysicalDeviceVulkan13Features != null)
            {
                VkPhysicalDeviceVulkan13Features physicalDeviceVulkan13Features = new();
                physicalDeviceVulkan13Features.sType = VkPhysicalDeviceVulkan13Features.STYPE;
                physicalDeviceVulkan13Features.pNext = pNext;
                _deviceConfiguration.SetPhysicalDeviceVulkan13Features(&physicalDeviceVulkan13Features);
                pNext = &physicalDeviceVulkan13Features;
            }
        }
        if (_context->Version >= VkVersion.VulkanApiVersion12)
        {
            if (_deviceConfiguration.SetPhysicalDeviceVulkan12Features != null)
            {
                VkPhysicalDeviceVulkan12Features physicalDeviceVulkan12Features;
                physicalDeviceVulkan12Features.sType = VkPhysicalDeviceVulkan12Features.STYPE;
                physicalDeviceVulkan12Features.pNext = pNext;
                _deviceConfiguration.SetPhysicalDeviceVulkan12Features(&physicalDeviceVulkan12Features);
                pNext = &physicalDeviceVulkan12Features;
            }
        }
        if (_context->Version >= VkVersion.VulkanApiVersion11)
        {
            if (_deviceConfiguration.SetPhysicalDeviceVulkan11Features != null)
            {
                VkPhysicalDeviceVulkan11Features physicalDeviceVulkan11Features;
                physicalDeviceVulkan11Features.sType = VkPhysicalDeviceVulkan11Features.STYPE;
                physicalDeviceVulkan11Features.pNext = pNext;
                _deviceConfiguration.SetPhysicalDeviceVulkan11Features(&physicalDeviceVulkan11Features);
                pNext = &physicalDeviceVulkan11Features;
            }
        }

        VkPhysicalDeviceFeatures2 physicalDeviceFeatures2;
        physicalDeviceFeatures2.sType                      = VkPhysicalDeviceFeatures2.STYPE;
        physicalDeviceFeatures2.pNext                      = pNext;
        physicalDeviceFeatures2.features.samplerAnisotropy = VkBool32.True;
        if (_deviceConfiguration.SetVkPhysicalDeviceFeatures != null)
        {
            _deviceConfiguration.SetVkPhysicalDeviceFeatures(&physicalDeviceFeatures2);
        }
        pNext = &physicalDeviceFeatures2;

        VkDeviceCreateInfo deviceCreateInfo;
        deviceCreateInfo.sType                   = VkDeviceCreateInfo.STYPE;
        deviceCreateInfo.pNext                   = pNext;
        deviceCreateInfo.flags                   = _deviceConfiguration.Flags;
        deviceCreateInfo.queueCreateInfoCount    = 1u + additionalDeviceQueueCreateInfoCount;
        deviceCreateInfo.pQueueCreateInfos       = pDeviceQueueCreateInfos;
        deviceCreateInfo.enabledLayerCount       = (uint)_deviceConfiguration.EnabledLayerNames.Count;
        deviceCreateInfo.ppEnabledLayerNames     = ppEnabledLayerNames;
        deviceCreateInfo.enabledExtensionCount   = (uint)_deviceConfiguration.EnabledExtensionNames.Count;
        deviceCreateInfo.ppEnabledExtensionNames = ppEnabledExtensionNames;
        deviceCreateInfo.pEnabledFeatures        = null;

        try
        {
            vkCreateDevice(_context->PhysicalDevice, &deviceCreateInfo, null, &_context->Device)
               .AssertVkResult();

            RetrieveDeviceQueue();

            CreateCommandPool(_context->Device, _context->QueueFamilyIndex, &_context->CommandPool,           VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT);
            CreateCommandPool(_context->Device, _context->QueueFamilyIndex, &_context->ShortLivedCommandPool, VK_COMMAND_POOL_CREATE_TRANSIENT_BIT);

            Load(_context->Device);
        }
        finally
        {
            for (int i = 0; i < _deviceConfiguration.EnabledExtensionNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledExtensionNames + i));
            }

            for (int i = 0; i < _deviceConfiguration.EnabledLayerNames.Count; i++)
            {
                Allocator.FreeNtString(*(ppEnabledLayerNames + i));
            }
        }
    }

    private void DestroyDevice()
    {
        if (_context->Device != VkDevice.Null)
        {
            vkDeviceWaitIdle(_context->Device)
               .AssertVkResult();

            if (_context->ShortLivedCommandPool != VkCommandPool.Null)
            {
                vkDestroyCommandPool(_context->Device, _context->ShortLivedCommandPool, null);
                _context->ShortLivedCommandPool = VkCommandPool.Null;
            }

            if (_context->CommandPool != VkCommandPool.Null)
            {
                vkDestroyCommandPool(_context->Device, _context->CommandPool, null);
                Context->CommandPool = VkCommandPool.Null;
            }

            Allocator.Free(_context->Queues - 1u, _context->QueuesCount + 1u);

            vkDestroyDevice(_context->Device, null);
            _context->Device = VkDevice.Null;
        }
    }
}
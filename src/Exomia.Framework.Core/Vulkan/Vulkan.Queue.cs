#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Configurations;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        private static bool RetrieveDeviceQueue(VkContext* context, QueueConfiguration configuration)
        {
            VkDeviceQueueInfo2 vkDeviceQueueInfo2;
            vkDeviceQueueInfo2.sType            = VkDeviceQueueInfo2.STYPE;
            vkDeviceQueueInfo2.pNext            = null;
            vkDeviceQueueInfo2.flags            = configuration.Flags;
            vkDeviceQueueInfo2.queueFamilyIndex = context->QueueFamilyIndex;
            vkDeviceQueueInfo2.queueIndex       = 0u;
            vkGetDeviceQueue2(context->Device, &vkDeviceQueueInfo2, &context->Queue);

            return true;
        }
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan;

/// <content> A vulkan. This class cannot be inherited. </content>
sealed unsafe partial class Vulkan
{
    /// <summary> Creates command pool. </summary>
    /// <param name="device">                    The device. </param>
    /// <param name="queueFamilyIndex">          The configuration. </param>
    /// <param name="commandPool">               [in,out] If non-null, the command pool. </param>
    /// <param name="commandPoolCreateFlagBits"> (Optional) The command pool create flag bits. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    public static bool CreateCommandPool(VkDevice device, uint queueFamilyIndex, VkCommandPool* commandPool, VkCommandPoolCreateFlagBits commandPoolCreateFlagBits = 0u)
    {
        VkCommandPoolCreateInfo commandPoolCreateInfo;
        commandPoolCreateInfo.sType            = VkCommandPoolCreateInfo.STYPE;
        commandPoolCreateInfo.pNext            = null;
        commandPoolCreateInfo.flags            = commandPoolCreateFlagBits;
        commandPoolCreateInfo.queueFamilyIndex = queueFamilyIndex;

        vkCreateCommandPool(device, &commandPoolCreateInfo, null, commandPool)
            .AssertVkResult();

        return true;
    }
}
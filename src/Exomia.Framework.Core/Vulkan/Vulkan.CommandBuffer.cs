#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    /// <summary> Creates command buffer. </summary>
    /// <param name="device">             The device. </param>
    /// <param name="commandPool">        The command pool. </param>
    /// <param name="commandBufferCount"> Number of command buffers. </param>
    /// <param name="commandBuffers">     [in,out] If non-null, the command buffers. </param>
    /// <param name="commandBufferLevel"> (Optional) The command buffer level. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    public static void CreateCommandBuffers(
        VkDevice             device,
        VkCommandPool        commandPool,
        uint                 commandBufferCount,
        VkCommandBuffer*     commandBuffers,
        VkCommandBufferLevel commandBufferLevel = VK_COMMAND_BUFFER_LEVEL_PRIMARY)
    {
        VkCommandBufferAllocateInfo commandBufferAllocateInfo;
        commandBufferAllocateInfo.sType              = VkCommandBufferAllocateInfo.STYPE;
        commandBufferAllocateInfo.pNext              = null;
        commandBufferAllocateInfo.commandPool        = commandPool;
        commandBufferAllocateInfo.level              = commandBufferLevel;
        commandBufferAllocateInfo.commandBufferCount = commandBufferCount;

        vkAllocateCommandBuffers(device, &commandBufferAllocateInfo, commandBuffers)
            .AssertVkResult();
    }
}
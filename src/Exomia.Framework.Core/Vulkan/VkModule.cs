#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> A vk module. </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkModule
{
    /// <summary> The identifier. </summary>
    public readonly ushort Id;

    /// <summary> The command buffers. </summary>
    public VkCommandBuffer* CommandBuffers;

    /// <summary> Convert this object into a string representation. </summary>
    /// <returns> A string that represents this object. </returns>
    public override string ToString()
    {
        return $"{Id} [alive: {CommandBuffers != null}]";
    }

    internal static VkModule Create(VkContext* vkContext, SwapchainContext* swapchainContext, ushort id)
    {
        VkModule module;
        *(ushort*)&module = id;

        module.CommandBuffers = Allocator.Allocate<VkCommandBuffer>(swapchainContext->MaxFramesInFlight);
        Vulkan.CreateCommandBuffers(
            vkContext->Device,
            vkContext->CommandPool,
            swapchainContext->MaxFramesInFlight,
            module.CommandBuffers,
            VK_COMMAND_BUFFER_LEVEL_SECONDARY);

        return module;
    }

    internal static void Destroy(VkContext* context, SwapchainContext* swapchainContext, ref VkModule* module)
    {
        if (context->CommandPool != VkCommandPool.Null && module->CommandBuffers != null)
        {
            try
            {
                vkFreeCommandBuffers(
                    context->Device,
                    context->CommandPool,
                    swapchainContext->MaxFramesInFlight,
                    module->CommandBuffers);
            }
            finally
            {
                Allocator.Free(ref module->CommandBuffers, swapchainContext->MaxFramesInFlight);
            }
        }

        module = null;
    }
}
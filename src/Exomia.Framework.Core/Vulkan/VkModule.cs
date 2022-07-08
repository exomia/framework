#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using Exomia.Framework.Core.Allocators;

using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> A vk module. </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct VkModule
{
    /// <summary> The identifier. </summary>
    public readonly ushort Id;

    /// <summary> The command buffers. </summary>
    public VkCommandBuffer* CommandBuffers;

    public override string ToString()
    {
        return $"[id: {Id}]";
    }

    internal static VkModule Create(VkContext* context, ushort id)
    {
        VkModule module;
        *(ushort*)&module              = id;

        module.CommandBuffers = Allocator.Allocate<VkCommandBuffer>(context->SwapchainImageCount);
        if (!Vulkan.CreateCommandBuffers(context->Device, context->CommandPool, context->SwapchainImageCount, module.CommandBuffers, VK_COMMAND_BUFFER_LEVEL_SECONDARY))
        {
            throw new Exception($"{nameof(VkModule)}.{nameof(Create)}->{nameof(Vulkan)}.{nameof(Vulkan.CreateCommandBuffers)} failed.");
        }

        return module;
    }

    internal static void Destroy(VkContext* context, ref VkModule* module)
    {
        if (context->CommandPool != VkCommandPool.Null && module->CommandBuffers != null)
        {
            try
            {
                vkFreeCommandBuffers(context->Device, context->CommandPool, context->SwapchainImageCount, module->CommandBuffers);
            }
            finally
            {
                Allocator.Free(ref module->CommandBuffers, context->SwapchainImageCount);
            }
        }

        module = null;
    }
}
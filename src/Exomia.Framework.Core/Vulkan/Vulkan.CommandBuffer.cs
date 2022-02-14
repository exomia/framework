#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkSubpassContents;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        /// <summary> Creates command buffer. </summary>
        /// <param name="device">             The device. </param>
        /// <param name="commandPool">        The command pool. </param>
        /// <param name="commandBufferCount"> Number of command buffers. </param>
        /// <param name="commandBuffers">     [in,out] If non-null, the command buffers. </param>
        /// <param name="commandBufferLevel"> (Optional) The command buffer level. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        public static bool CreateCommandBuffers(
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

            return true;
        }

        /// <summary> Adds a secondary command buffers. </summary>
        /// <param name="context"> [in,out] If non-null, the context. </param>
        /// <param name="buffers"> [in,out] If non-null, the buffers. </param>
        /// <param name="slot">    (Optional) The slot. </param>
        /// <returns> The slot index which is occupied and reserved. </returns>
        public uint AddSecondaryCommandBuffers(VkContext* context, VkCommandBuffer* buffers, uint slot = uint.MaxValue)
        {
            lock (_buffersLock)
            {
                if (_subCommandBuffersCurrentSlot >= _subCommandBuffersSlotCount)
                {
                    for (uint i = 0u; i < _subCommandBuffersCount; i++)
                    {
                        VkCommandBuffer* oldBuffer = *(_subCommandBuffers + i);
                        Unsafe.CopyBlock(
                            *(_subCommandBuffers + i) = Allocator.Allocate<VkCommandBuffer>(_subCommandBuffersSlotCount * 2),
                            oldBuffer,
                            (uint)sizeof(VkCommandBuffer));
                        Allocator.Free(oldBuffer, _subCommandBuffersSlotCount);
                    }
                    _subCommandBuffersSlotCount *= 2;
                }

                if (slot != uint.MaxValue)
                {
                    for (uint i = 0u; i < _subCommandBuffersCount; i++)
                    {
                        *(*(_subCommandBuffers + i) + slot) = *(buffers + i);
                    }

                    //RecordDefaultCommandBuffer(context);

                    return slot;
                }

                uint current = _subCommandBuffersCurrentSlot;
                for (uint i = 0u; i < _subCommandBuffersCount; i++)
                {
                    *(*(_subCommandBuffers + i) + current) = *(buffers + i);
                }
                _subCommandBuffersCurrentSlot++;

                //RecordDefaultCommandBuffer(context);

                return current;
            }
        }

        private void FreeSubCommandBuffers()
        {
            if (_subCommandBuffers != null)
            {
                for (uint i = 0u; i < _subCommandBuffersCount; i++)
                {
                    Allocator.Free(*(_subCommandBuffers + i), _subCommandBuffersCount);
                }
                Allocator.Free(_subCommandBuffers, _subCommandBuffersCount * (uint)IntPtr.Size);
            }
        }

        private void AllocateSubCommandBuffers(VkContext* context)
        {
            FreeSubCommandBuffers();

            _subCommandBuffersCount = context->SwapchainImageCount;
            _subCommandBuffers      = (VkCommandBuffer**)Allocator.Allocate(_subCommandBuffersCount * (uint)IntPtr.Size);
            for (uint i = 0u; i < _subCommandBuffersCount; i++)
            {
                *(_subCommandBuffers + i) = Allocator.Allocate<VkCommandBuffer>(_subCommandBuffersSlotCount);
            }
        }

        private bool RecordDefaultCommandBuffer(VkContext* context)
        {
            VkCommandBufferBeginInfo commandBufferBeginInfo;
            commandBufferBeginInfo.sType            = VkCommandBufferBeginInfo.STYPE;
            commandBufferBeginInfo.pNext            = null;
            commandBufferBeginInfo.flags            = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;
            commandBufferBeginInfo.pInheritanceInfo = null;

            VkClearValue* pClearValues = stackalloc VkClearValue[2];
            (pClearValues + 0)->color                = VkColors.Black;
            (pClearValues + 1)->depthStencil.depth   = 1.0f;
            (pClearValues + 1)->depthStencil.stencil = 0u;

            //for (uint i = 0u; i < context->SwapchainImageCount; i++)
            {
                VkCommandBuffer commandBuffer = *(context->CommandBuffers + /*i*/ +context->ImageIndex);

                vkBeginCommandBuffer(commandBuffer, &commandBufferBeginInfo)
                    .AssertVkResult();

                VkRenderPassBeginInfo renderPassBeginInfo;
                renderPassBeginInfo.sType                    = VkRenderPassBeginInfo.STYPE;
                renderPassBeginInfo.pNext                    = null;
                renderPassBeginInfo.renderPass               = context->RenderPass;
                renderPassBeginInfo.framebuffer              = *(context->Framebuffers + /*i*/ +context->ImageIndex);
                renderPassBeginInfo.renderArea.offset.x      = 0;
                renderPassBeginInfo.renderArea.offset.y      = 0;
                renderPassBeginInfo.renderArea.extent.width  = context->Width;
                renderPassBeginInfo.renderArea.extent.height = context->Height;
                renderPassBeginInfo.clearValueCount          = 2u;
                renderPassBeginInfo.pClearValues             = pClearValues;

                VkSubpassBeginInfo subpassBeginInfo;
                subpassBeginInfo.sType    = VkSubpassBeginInfo.STYPE;
                subpassBeginInfo.pNext    = null;
                subpassBeginInfo.contents = VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS;
                vkCmdBeginRenderPass2(commandBuffer, &renderPassBeginInfo, &subpassBeginInfo);

                if (_subCommandBuffersCurrentSlot > 0)
                {
                    vkCmdExecuteCommands(commandBuffer, _subCommandBuffersCurrentSlot, *(_subCommandBuffers + /*i*/ +context->ImageIndex));
                }

                VkSubpassEndInfo subpassEndInfo;
                subpassEndInfo.sType = VkSubpassEndInfo.STYPE;
                subpassEndInfo.pNext = null;
                vkCmdEndRenderPass2(commandBuffer, &subpassEndInfo);

                vkEndCommandBuffer(commandBuffer)
                    .AssertVkResult();
            }

            return true;
        }
    }
}
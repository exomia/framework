#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkCommandBufferLevel;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkSubpassContents;

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

    private bool RecordCommandBuffer(VkContext* context)
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


        VkCommandBuffer commandBuffer = *(context->CommandBuffers + context->ImageIndex);

        vkBeginCommandBuffer(commandBuffer, &commandBufferBeginInfo)
            .AssertVkResult();

        VkRenderPassBeginInfo renderPassBeginInfo;
        renderPassBeginInfo.sType                    = VkRenderPassBeginInfo.STYPE;
        renderPassBeginInfo.pNext                    = null;
        renderPassBeginInfo.renderPass               = context->RenderPass;
        renderPassBeginInfo.framebuffer              = *(context->Framebuffers + context->ImageIndex);
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

        if ((_moduleCommandBuffersAreDirty & (1u << (int)Context->FrameInFlight)) != 0)
        {
            // swap dirty flag
            _moduleCommandBuffersAreDirty &= ~(1u << (int)Context->FrameInFlight);

            uint index             = 0u;
            uint modulesDirtyCount = _modulesCurrentCount;
            while (modulesDirtyCount > 0u)
            {
                VkModule* module = (_modules + index);
                if (module->CommandBuffers != null)
                {
                    *(*(_moduleCommandBuffers + Context->FrameInFlight) + index) = *(module->CommandBuffers + Context->FrameInFlight);
                    modulesDirtyCount--;
                }
                index++;
            }
        }

        if (_modulesCurrentCount > 0)
        {
            vkCmdExecuteCommands(commandBuffer, _modulesCurrentCount, *(_moduleCommandBuffers + context->FrameInFlight));
        }

        VkSubpassEndInfo subpassEndInfo;
        subpassEndInfo.sType = VkSubpassEndInfo.STYPE;
        subpassEndInfo.pNext = null;
        vkCmdEndRenderPass2(commandBuffer, &subpassEndInfo);

        vkEndCommandBuffer(commandBuffer)
            .AssertVkResult();

        return true;
    }
}
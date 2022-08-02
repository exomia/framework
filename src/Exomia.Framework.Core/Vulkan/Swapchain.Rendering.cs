#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Vulkan.Exceptions;

#pragma warning disable 1591

namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Swapchain
{
    public bool BeginFrame()
    {
#if DEBUG
        if (_isFrameStarted)
        {
            throw new Exception("Can't call begin frame while a frame is already in progress!");
        }
#endif

        vkWaitForFences(
                _vkContext->Device,
                1u,            _context->InFlightFences + _context->FrameInFlight,
                VkBool32.True, ulong.MaxValue)
#if DEBUG
            .AssertVkResult()
#endif
            ;

        if (_framebufferShouldResize)
        {
            _framebufferShouldResize = false;
            Recreate();
        }

        VkAcquireNextImageInfoKHR acquireNextImageInfoKhr;
        acquireNextImageInfoKhr.sType      = VkAcquireNextImageInfoKHR.STYPE;
        acquireNextImageInfoKhr.pNext      = null;
        acquireNextImageInfoKhr.swapchain  = _swapchain;
        acquireNextImageInfoKhr.timeout    = ulong.MaxValue;
        acquireNextImageInfoKhr.semaphore  = *(_context->SemaphoresImageAvailable + _context->FrameInFlight);
        acquireNextImageInfoKhr.fence      = VkFence.Null;
        acquireNextImageInfoKhr.deviceMask = 1u;

        VkResult result = vkAcquireNextImage2KHR(_vkContext->Device, &acquireNextImageInfoKhr, &_context->ImageIndex);
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (result)
        {
            case VK_SUCCESS:
            case VK_SUBOPTIMAL_KHR:
                break;
            case VK_ERROR_OUT_OF_DATE_KHR:
                Recreate();
                return false;
            case VK_NOT_READY:
            case VK_TIMEOUT:
                return false;
            default: throw new VulkanException(result, "Failed to acquire swap chain image!");
        }

        if (*(_context->ImagesInFlightFence + _context->ImageIndex) != VkFence.Null)
        {
            vkWaitForFences(
                    _vkContext->Device,
                    1u,            _context->ImagesInFlightFence + _context->ImageIndex,
                    VkBool32.True, ulong.MaxValue)
#if DEBUG
                .AssertVkResult()
#endif
                ;
        }

        *(_context->ImagesInFlightFence + _context->ImageIndex) = *(_context->InFlightFences + _context->FrameInFlight);

#if DEBUG
        _isFrameStarted = true;
#endif
        return true;
    }

    public void BeginRenderPass(
        VkCommandBuffer   commandBuffer,
        VkSubpassContents subpassContents = VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE)
    {
        VkRenderPassBeginInfo renderPassBeginInfo;
        renderPassBeginInfo.sType                    = VkRenderPassBeginInfo.STYPE;
        renderPassBeginInfo.pNext                    = null;
        renderPassBeginInfo.renderPass               = _renderPass;
        renderPassBeginInfo.framebuffer              = *(_context->Framebuffers + _context->ImageIndex);
        renderPassBeginInfo.renderArea.offset.x      = 0;
        renderPassBeginInfo.renderArea.offset.y      = 0;
        renderPassBeginInfo.renderArea.extent.width  = _context->Width;
        renderPassBeginInfo.renderArea.extent.height = _context->Height;

        VkClearValue* pClearValues = stackalloc VkClearValue[2];
        (pClearValues + 0)->color                = VkColors.DimGray;
        (pClearValues + 1)->depthStencil.depth   = 1.0f;
        (pClearValues + 1)->depthStencil.stencil = 0u;

        renderPassBeginInfo.clearValueCount = 2u;
        renderPassBeginInfo.pClearValues    = pClearValues;

        VkSubpassBeginInfo subpassBeginInfo;
        subpassBeginInfo.sType    = VkSubpassBeginInfo.STYPE;
        subpassBeginInfo.pNext    = null;
        subpassBeginInfo.contents = subpassContents;
        vkCmdBeginRenderPass2(commandBuffer, &renderPassBeginInfo, &subpassBeginInfo);
    }

    public void EndRenderPass(VkCommandBuffer commandBuffer)
    {
#if DEBUG
        if (!_isFrameStarted)
        {
            throw new Exception($"Can't call {nameof(EndRenderPass)} while a frame is not in progress!");
        }
#endif

        //CmdExecuteCommandsFromModules(commandBuffer);

        VkSubpassEndInfo subpassEndInfo;
        subpassEndInfo.sType = VkSubpassEndInfo.STYPE;
        subpassEndInfo.pNext = null;
        vkCmdEndRenderPass2(commandBuffer, &subpassEndInfo);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Submit(VkCommandBuffer commandBuffer)
    {
        Submit(&commandBuffer, 0u, 1u);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Submit(VkCommandBuffer* commandBuffer, uint count)
    {
        Submit(commandBuffer, 0u, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Submit(VkCommandBuffer* commandBuffers, uint index, uint count)
    {
        Submit(commandBuffers, index, count, *(_context->InFlightFences + _context->FrameInFlight));
    }

    public void Submit(VkCommandBuffer* commandBuffers, uint index, uint count, VkFence fence)
    {
        VkPipelineStageFlagBits* pWaitDstStageMask = stackalloc VkPipelineStageFlagBits[]
        {
            VkPipelineStageFlagBits.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT
        };

        VkSubmitInfo submitInfo;
        submitInfo.sType              = VkSubmitInfo.STYPE;
        submitInfo.pNext              = null;
        submitInfo.commandBufferCount = count;
        submitInfo.pCommandBuffers    = commandBuffers + index;
        submitInfo.waitSemaphoreCount = 1u;
        submitInfo.pWaitDstStageMask  = pWaitDstStageMask;

        if ((_firstSubmitInFrame & (1u << (int)_context->FrameInFlight)) == 0)
        {
            _firstSubmitInFrame        |= (1u << (int)_context->FrameInFlight); // set bit
            submitInfo.pWaitSemaphores =  _context->SemaphoresImageAvailable + _context->FrameInFlight;
        }
        else
        {
            submitInfo.pWaitSemaphores = _context->SemaphoresRenderingDone + _context->FrameInFlight;
        }

        submitInfo.signalSemaphoreCount = 1u;
        submitInfo.pSignalSemaphores    = _context->SemaphoresRenderingDone + _context->FrameInFlight;

        vkResetFences(_vkContext->Device, 1u, &fence)
#if DEBUG
            .AssertVkResult()
#endif
            ;

        vkQueueSubmit(*(_vkContext->Queues - 1u), 1u, &submitInfo, fence)
#if DEBUG
            .AssertVkResult()
#endif
            ;
    }


    public void EndFrame()
    {
#if DEBUG
        if (!_isFrameStarted)
        {
            throw new Exception($"Can't call {nameof(EndFrame)} while a frame is not in progress!");
        }
        _isFrameStarted = false;
#endif

        _firstSubmitInFrame &= ~(1u << (int)_context->FrameInFlight); // clear bit

        VkSwapchainKHR   swapchainKhr = _swapchain;
        VkPresentInfoKHR presentInfoKhr;
        presentInfoKhr.sType              = VkPresentInfoKHR.STYPE;
        presentInfoKhr.pNext              = null;
        presentInfoKhr.waitSemaphoreCount = 1u;
        presentInfoKhr.pWaitSemaphores    = _context->SemaphoresRenderingDone + _context->FrameInFlight;
        presentInfoKhr.swapchainCount     = 1u;
        presentInfoKhr.pSwapchains        = &swapchainKhr;
        presentInfoKhr.pImageIndices      = &_context->ImageIndex;
        presentInfoKhr.pResults           = null;

        VkResult result = vkQueuePresentKHR(*(_vkContext->Queues - 1u), &presentInfoKhr);
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (result)
        {
            case VK_SUCCESS:
                break;
            case VK_SUBOPTIMAL_KHR:
            case VK_ERROR_OUT_OF_DATE_KHR:
                vkWaitForFences(
                        _vkContext->Device,
                        _context->MaxFramesInFlight, _context->InFlightFences,
                        VkBool32.True,               ulong.MaxValue)
#if DEBUG
                    .AssertVkResult()
#endif
                    ;

                Recreate();
                break;
            case VK_ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT:
                Console.WriteLine("fullscreen lost...");
                break;
            default: throw new VulkanException(result, "Failed to present swap chain image!");
        }

        _context->FrameInFlight = (_context->FrameInFlight + 1u) % _context->MaxFramesInFlight;
    }
}
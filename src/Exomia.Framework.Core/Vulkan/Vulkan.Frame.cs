#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Exceptions;
using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;

namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Vulkan
{
    internal bool BeginFrame()
    {
        vkWaitForFences(Context->Device, 1u, Context->InFlightFences + Context->FrameInFlight, VK_TRUE, ulong.MaxValue)
            .AssertVkResult();

        VkAcquireNextImageInfoKHR acquireNextImageInfoKhr;
        acquireNextImageInfoKhr.sType      = VkAcquireNextImageInfoKHR.STYPE;
        acquireNextImageInfoKhr.pNext      = null;
        acquireNextImageInfoKhr.swapchain  = Context->Swapchain;
        acquireNextImageInfoKhr.timeout    = ulong.MaxValue;
        acquireNextImageInfoKhr.semaphore  = *(Context->SemaphoresImageAvailable + Context->FrameInFlight);
        acquireNextImageInfoKhr.fence      = VkFence.Null;
        acquireNextImageInfoKhr.deviceMask = 1u;

        VkResult result = vkAcquireNextImage2KHR(Context->Device, &acquireNextImageInfoKhr, &Context->ImageIndex);
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (result)
        {
            case VK_SUCCESS:
            case VK_SUBOPTIMAL_KHR:
                break;
            case VK_ERROR_OUT_OF_DATE_KHR:
                vkWaitForFences(Context->Device, Context->MaxFramesInFlight, Context->InFlightFences, VK_TRUE, ulong.MaxValue)
                    .AssertVkResult();
                RecreateSwapChainInternal(Context, Context->Swapchain);
                return false;
            case VK_NOT_READY:
            case VK_TIMEOUT:
                return false;
            default: throw new VulkanException(result, "Failed to acquire swap chain image!");
        }

        if (*(Context->ImagesInFlightFence + Context->ImageIndex) != VkFence.Null)
        {
            vkWaitForFences(Context->Device, 1u, Context->ImagesInFlightFence + Context->ImageIndex, VK_TRUE, ulong.MaxValue)
                .AssertVkResult();
        }

        *(Context->ImagesInFlightFence + Context->ImageIndex) = *(Context->InFlightFences + Context->FrameInFlight);

        return true;
    }
    
    internal void EndFrame()
    {
        vkResetFences(Context->Device, 1u, Context->InFlightFences + Context->FrameInFlight)
            .AssertVkResult();

        RecordCommandBuffer(Context);

        VkPipelineStageFlagBits* pWaitDstStageMask = stackalloc VkPipelineStageFlagBits[1]
        {
            VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT
        };

        VkSubmitInfo submitInfo;
        submitInfo.sType                = VkSubmitInfo.STYPE;
        submitInfo.pNext                = null;
        submitInfo.waitSemaphoreCount   = 1u;
        submitInfo.pWaitSemaphores      = Context->SemaphoresImageAvailable + Context->FrameInFlight;
        submitInfo.pWaitDstStageMask    = pWaitDstStageMask;
        submitInfo.commandBufferCount   = 1u;
        submitInfo.pCommandBuffers      = Context->CommandBuffers + Context->ImageIndex;
        submitInfo.signalSemaphoreCount = 1u;
        submitInfo.pSignalSemaphores    = Context->SemaphoresRenderingDone + Context->FrameInFlight;

        vkQueueSubmit(Context->Queue, 1u, &submitInfo, *(Context->InFlightFences + Context->FrameInFlight))
            .AssertVkResult();

        //// check if the bit for the rendering done semaphores for the current frame in flight is dirty
        //if ((_moduleCommandBufferIsDirty & (1u << (int)Context->FrameInFlight)) != 0)
        //{
        //    // swap dirty flag
        //    _moduleCommandBufferIsDirty &= ~(1u << (int)Context->FrameInFlight);

        //    // start: recreate pWaitSemaphores for current frame in flight
        //    *(*(_semaphoresRenderingDone + Context->FrameInFlight)) = *(Context->SemaphoresRenderingDone + Context->FrameInFlight);

        //    uint index             = 0u;
        //    uint modulesDirtyCount = _modulesCurrentCount;
        //    while (modulesDirtyCount > 0u)
        //    {
        //        VkModule* module = (_modules + index);
        //        if (module->SemaphoresRenderingDone != null)
        //        {
        //            *(*(_semaphoresRenderingDone + Context->FrameInFlight) + 1u + index) = *(module->SemaphoresRenderingDone + Context->FrameInFlight);
        //            modulesDirtyCount--;
        //        }
        //        index++;
        //    }
        //    // end: recreate pWaitSemaphores for current frame in flight
        //}

        VkPresentInfoKHR presentInfoKhr;
        presentInfoKhr.sType              = VkPresentInfoKHR.STYPE;
        presentInfoKhr.pNext              = null;
        presentInfoKhr.waitSemaphoreCount = 1u;
        presentInfoKhr.pWaitSemaphores    = Context->SemaphoresRenderingDone + Context->FrameInFlight;
        presentInfoKhr.swapchainCount     = 1u;
        presentInfoKhr.pSwapchains        = &Context->Swapchain;
        presentInfoKhr.pImageIndices      = &Context->ImageIndex;
        presentInfoKhr.pResults           = null;

        VkResult result = vkQueuePresentKHR(Context->Queue, &presentInfoKhr);
        if (result == VK_SUBOPTIMAL_KHR ||
            result == VK_ERROR_OUT_OF_DATE_KHR ||
            Context->FramebufferResized)
        {
            Context->FramebufferResized = false;

            vkWaitForFences(Context->Device, Context->MaxFramesInFlight, Context->InFlightFences, VK_TRUE, ulong.MaxValue)
                .AssertVkResult();

            Context->Width  = _requestedWidth;
            Context->Height = _requestedHeight;

            RecreateSwapChainInternal(Context, Context->Swapchain);

            return;
        }

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (result)
        {
            case VK_SUCCESS:
                break;
            case VK_ERROR_FULL_SCREEN_EXCLUSIVE_MODE_LOST_EXT:
                Console.WriteLine("fullscreen lost...");
                moep:
                VkResult result2 = ((delegate*<VkDevice, VkSwapchainKHR, VkResult>)Context->Device
                    .GetDeviceProcAddr("vkAcquireFullScreenExclusiveModeEXT"))(Context->Device, Context->Swapchain);
                Console.WriteLine(result2);
                if (result2 != VK_SUCCESS)
                {
                    goto moep;
                }
                break;
            default: throw new VulkanException(result, "Failed to present swap chain image!");
        }

        Context->FrameInFlight = (Context->FrameInFlight + 1u) % Context->MaxFramesInFlight;
    }
}
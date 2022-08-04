#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkCommandBufferUsageFlagBits;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> A renderer. This class cannot be inherited. </summary>
public sealed unsafe class Renderer : IDisposable
{
    private readonly VkContext*        _vkContext;
    private readonly Swapchain         _swapchain;
    private readonly SwapchainContext* _swapchainContext;
    private          VkCommandBuffer*  _commandBuffers;

    /// <summary> Initializes a new instance of the <see cref="Renderer" /> class. </summary>
    public Renderer(Swapchain swapchain)
    {
        _swapchain        = swapchain;
        _swapchainContext = swapchain.Context;
        _vkContext        = swapchain.VkContext;
        _commandBuffers   = Allocator.Allocate<VkCommandBuffer>(_swapchainContext->MaxFramesInFlight);

        Vulkan.CreateCommandBuffers(
            _vkContext->Device,
            _vkContext->CommandPool,
            _swapchainContext->MaxFramesInFlight,
            _commandBuffers);
    }

    /// <summary> Begins the rendering. </summary>
    /// <param name="commandBuffer"> [in,out] If non-null, [out] The command buffer. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
    public bool Begin(out VkCommandBuffer commandBuffer)
    {
#if DEBUG
        if (!_swapchain.IsFrameStarted)
        {
            throw new Exception($"Can't call {nameof(Begin)} while a frame is in progress!");
        }
#endif

        VkCommandBufferBeginInfo commandBufferBeginInfo;
        commandBufferBeginInfo.sType            = VkCommandBufferBeginInfo.STYPE;
        commandBufferBeginInfo.pNext            = null;
        commandBufferBeginInfo.flags            = VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT;
        commandBufferBeginInfo.pInheritanceInfo = null;

        vkBeginCommandBuffer(
                commandBuffer = *(_commandBuffers + _swapchainContext->FrameInFlight),
                &commandBufferBeginInfo)
#if DEBUG
           .AssertVkResult()
#endif
            ;

        return true;
    }

    /// <summary> Begins the render pass. </summary>
    /// <param name="commandBuffer"> Buffer for command data. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BeginRenderPass(VkCommandBuffer commandBuffer)
    {
        _swapchain.BeginRenderPass(commandBuffer, VkSubpassContents.VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS);
    }

    /// <summary> Ends the render pass. </summary>
    /// <param name="commandBuffer"> Buffer for command data. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndRenderPass(VkCommandBuffer commandBuffer)
    {
        _swapchain.EndRenderPass(commandBuffer);
    }

    /// <summary> Ends the rendering. </summary>
    /// <param name="commandBuffer"> If non-null, the command buffers. </param>
    /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
    public void End(VkCommandBuffer commandBuffer)
    {
#if DEBUG
        if (!_swapchain.IsFrameStarted)
        {
            throw new Exception($"Can't call {nameof(End)} while a frame is not in progress!");
        }
#endif
        vkEndCommandBuffer(*(_commandBuffers + _swapchainContext->FrameInFlight))
#if DEBUG
           .AssertVkResult()
#endif
            ;

        _swapchain.Submit(commandBuffer);
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            if (_vkContext->Device      != VkDevice.Null      &&
                _vkContext->CommandPool != VkCommandPool.Null &&
                _commandBuffers         != null)
            {
                vkDeviceWaitIdle(_vkContext->Device)
                   .AssertVkResult();

                vkFreeCommandBuffers(_vkContext->Device, _vkContext->CommandPool, _swapchainContext->MaxFramesInFlight, _commandBuffers);
                Allocator.Free(ref _commandBuffers, _swapchainContext->MaxFramesInFlight);
            }
        }
        GC.SuppressFinalize(this);
    }

    /// <summary> Finalizes an instance of the <see cref="Renderer" /> class. </summary>
    ~Renderer()
    {
        Dispose();
    }

    #endregion
}
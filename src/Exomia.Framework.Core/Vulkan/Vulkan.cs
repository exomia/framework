#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.IoC.Attributes;
using Exomia.Logging;
using static Exomia.Vulkan.Api.Core.VkFormatFeatureFlagBits;
using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;
using static Exomia.Vulkan.Api.Core.VkFenceCreateFlagBits;
using static Exomia.Vulkan.Api.Core.VkCommandPoolCreateFlagBits;

namespace Exomia.Framework.Core.Vulkan
{
    public sealed unsafe partial class Vulkan : IDisposable
    {
        /// <summary> Occurs when the swapchain was recreated. </summary>
        public event EventHandler<Vulkan>? SwapChainRecreated;

        /// <summary> Occurs when the swapchain is cleaned up. </summary>
        public event EventHandler<Vulkan>? CleanupSwapChain;

        private readonly ILogger<Vulkan>                  _logger;
        private readonly ApplicationConfiguration         _applicationConfiguration;
        private readonly InstanceConfiguration            _instanceConfiguration;
        private readonly DebugUtilsMessengerConfiguration _debugUtilsMessengerConfiguration;
        private readonly SurfaceConfiguration             _surfaceConfiguration;
        private readonly PhysicalDeviceConfiguration      _physicalDeviceConfiguration;
        private readonly DepthStencilConfiguration        _depthStencilConfiguration;
        private readonly DeviceConfiguration              _deviceConfiguration;
        private readonly QueueConfiguration               _queueConfiguration;
        private readonly SwapchainConfiguration           _swapchainConfiguration;
        private readonly RenderPassConfiguration          _renderPassConfiguration;

        private readonly object            _buffersLock = new object();
        private          VkCommandBuffer** _subCommandBuffers;
        private          uint              _subCommandBuffersCount       = 0u;
        private          uint              _subCommandBuffersSlotCount   = 8u;
        private          uint              _subCommandBuffersCurrentSlot = 0u;

        private LogHandler _logHandler = null!;

        /// <summary> The context. </summary>
        private VkContext* _context;

        /// <summary> Gets the context. </summary>
        /// <value> The context. </value>
        public VkContext* Context
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _context; }
        }

        /// <summary> Initializes a new instance of the <see cref="Vulkan" /> class. </summary>
        /// <param name="renderForm">                       The render form. </param>
        /// <param name="applicationConfiguration">         The application configuration. </param>
        /// <param name="instanceConfiguration">            The instance configuration. </param>
        /// <param name="debugUtilsMessengerConfiguration"> The debug utilities messenger configuration. </param>
        /// <param name="surfaceConfiguration">             The surface configuration. </param>
        /// <param name="physicalDeviceConfiguration">      The physical device configuration. </param>
        /// <param name="depthStencilConfiguration">        The depth stencil configuration. </param>
        /// <param name="deviceConfiguration">              The device configuration. </param>
        /// <param name="queueConfiguration">               The queue configuration. </param>
        /// <param name="swapchainConfiguration">           The swapchain configuration. </param>
        /// <param name="renderPassConfiguration">          The render pass configuration. </param>
        /// <param name="logger">                           The logger. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public Vulkan(IRenderForm                                                        renderForm,
                      ApplicationConfiguration                                           applicationConfiguration,
                      InstanceConfiguration                                              instanceConfiguration,
                      DebugUtilsMessengerConfiguration                                   debugUtilsMessengerConfiguration,
                      SurfaceConfiguration                                               surfaceConfiguration,
                      PhysicalDeviceConfiguration                                        physicalDeviceConfiguration,
                      DepthStencilConfiguration                                          depthStencilConfiguration,
                      DeviceConfiguration                                                deviceConfiguration,
                      QueueConfiguration                                                 queueConfiguration,
                      SwapchainConfiguration                                             swapchainConfiguration,
                      RenderPassConfiguration                                            renderPassConfiguration,
                      [IoCOptional(typeof(SimpleConsoleLogger<Vulkan>))] ILogger<Vulkan> logger)
        {
            _applicationConfiguration         = applicationConfiguration ?? throw new ArgumentNullException(nameof(applicationConfiguration));
            _instanceConfiguration            = instanceConfiguration ?? throw new ArgumentNullException(nameof(instanceConfiguration));
            _debugUtilsMessengerConfiguration = debugUtilsMessengerConfiguration ?? throw new ArgumentNullException(nameof(debugUtilsMessengerConfiguration));
            _surfaceConfiguration             = surfaceConfiguration ?? throw new ArgumentNullException(nameof(surfaceConfiguration));
            _physicalDeviceConfiguration      = physicalDeviceConfiguration ?? throw new ArgumentNullException(nameof(physicalDeviceConfiguration));
            _depthStencilConfiguration        = depthStencilConfiguration ?? throw new ArgumentNullException(nameof(depthStencilConfiguration));
            _deviceConfiguration              = deviceConfiguration ?? throw new ArgumentNullException(nameof(deviceConfiguration));
            _queueConfiguration               = queueConfiguration ?? throw new ArgumentNullException(nameof(queueConfiguration));
            _swapchainConfiguration           = swapchainConfiguration ?? throw new ArgumentNullException(nameof(swapchainConfiguration));
            _renderPassConfiguration          = renderPassConfiguration ?? throw new ArgumentNullException(nameof(renderPassConfiguration));
            _logger                           = logger ?? throw new ArgumentNullException(nameof(logger));

            *(_context = Allocator.Allocate<VkContext>(1u)) = VkContext.Create();
            renderForm.Resized += form =>
            {
                Context->Width              = (uint)form.Width;
                Context->Height             = (uint)form.Height;
                Context->FramebufferResized = true;
            };
        }

        private static bool CreateSyncObjects(VkContext* context)
        {
            VkFenceCreateInfo fenceCreateInfo;
            fenceCreateInfo.sType = VkFenceCreateInfo.STYPE;
            fenceCreateInfo.pNext = null;
            fenceCreateInfo.flags = VK_FENCE_CREATE_SIGNALED_BIT;

            VkSemaphoreCreateInfo semaphoreCreateInfo;
            semaphoreCreateInfo.sType = VkSemaphoreCreateInfo.STYPE;
            semaphoreCreateInfo.pNext = null;
            semaphoreCreateInfo.flags = 0;

            context->InFlightFences           = Allocator.Allocate<VkFence>(context->MaxFramesInFlight);
            context->SemaphoresImageAvailable = Allocator.Allocate<VkSemaphore>(context->MaxFramesInFlight);
            context->SemaphoresRenderingDone  = Allocator.Allocate<VkSemaphore>(context->MaxFramesInFlight);

            for (uint i = 0u; i < context->MaxFramesInFlight; i++)
            {
                vkCreateFence(context->Device, &fenceCreateInfo, null, context->InFlightFences + i)
                    .AssertVkResult();
                vkCreateSemaphore(context->Device, &semaphoreCreateInfo, null, context->SemaphoresImageAvailable + i)
                    .AssertVkResult();
                vkCreateSemaphore(context->Device, &semaphoreCreateInfo, null, context->SemaphoresRenderingDone + i)
                    .AssertVkResult();
            }

            return true;
        }

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

            RecordDefaultCommandBuffer(Context);

            VkPipelineStageFlagBits* pPipelineStageFlagBits = stackalloc VkPipelineStageFlagBits[1] { VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT };

            VkSubmitInfo submitInfo;
            submitInfo.sType                = VkSubmitInfo.STYPE;
            submitInfo.pNext                = null;
            submitInfo.waitSemaphoreCount   = 1u;
            submitInfo.pWaitSemaphores      = Context->SemaphoresImageAvailable + Context->FrameInFlight;
            submitInfo.pWaitDstStageMask    = pPipelineStageFlagBits;
            submitInfo.commandBufferCount   = 1u;
            submitInfo.pCommandBuffers      = Context->CommandBuffers + Context->ImageIndex;
            submitInfo.signalSemaphoreCount = 1u;
            submitInfo.pSignalSemaphores    = Context->SemaphoresRenderingDone + Context->FrameInFlight;

            vkQueueSubmit(Context->Queue, 1u, &submitInfo, *(Context->InFlightFences + Context->FrameInFlight))
                .AssertVkResult();

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

        internal bool Initialize()
        {
            if (!CreateInstance(Context, _applicationConfiguration, _instanceConfiguration))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(CreateInstance)} failed!"));
                return false;
            }

#if DEBUG
            if (!SetupDebugCallback(Context, _debugUtilsMessengerConfiguration))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(SetupDebugCallback)} failed!"));
                return false;
            }
#endif

            if (_surfaceConfiguration.CreateSurface != null && !_surfaceConfiguration.CreateSurface(Context))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(_surfaceConfiguration.CreateSurface)} failed!"));
                return false;
            }

            VkKhrSurface.Load(Context->Instance);

            if (!PickBestPhysicalDevice(Context, _physicalDeviceConfiguration, _deviceConfiguration))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(PickBestPhysicalDevice)} failed!"));
                return false;
            }

            if (!CreateDevice(Context, _deviceConfiguration))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(CreateDevice)} failed!"));
                return false;
            }

            if (!RetrieveDeviceQueue(Context, _queueConfiguration))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(RetrieveDeviceQueue)} failed!"));
                return false;
            }

            if (!GetSuitableDepthStencilFormat(
                    Context,
                    _depthStencilConfiguration.Formats,
                    _depthStencilConfiguration.Tiling,
                    VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT,
                    out Context->DepthStencilFormat))
            {
                _logger.Log(
                    LogLevel.Critical,
                    new VulkanException($"The system doesn't support one of the specified depth stencil formats ({string.Join(',', _depthStencilConfiguration.Formats)})!"));
                return false;
            }

            VkKhrGetSurfaceCapabilities2.Load(Context->Instance);
            Load(Context->Instance, Context->Device);

            if (!RecreateSwapChainInternal(Context, VkSwapchainKHR.Null))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(RecreateSwapChainInternal)} failed!"));
                return false;
            }

            Context->ImagesInFlightFence = Allocator.Allocate<VkFence>(Context->SwapchainImageCount, 0);

            if (!CreateSyncObjects(Context))
            {
                _logger.Log(LogLevel.Critical, new VulkanException($"{nameof(CreateSyncObjects)} failed!"));
                return false;
            }

            _logger.Log(LogLevel.Information, null, $"Vulkan {nameof(Initialize)} successfully.");
            return true;
        }

        internal void Cleanup()
        {
            if (Context->Device != VkDevice.Null)
            {
                _logger.Log(LogLevel.Information, null, "[Cleanup] Wait for device idle...");
                _logger.Log(LogLevel.Information, null, "[Cleanup] device idle: {0:G}", vkDeviceWaitIdle(Context->Device));

                if (Context->InFlightFences != null)
                {
                    for (uint i = 0u; i < Context->MaxFramesInFlight; i++)
                    {
                        vkDestroySemaphore(Context->Device, *(Context->SemaphoresRenderingDone + i),  null);
                        vkDestroySemaphore(Context->Device, *(Context->SemaphoresImageAvailable + i), null);
                        vkDestroyFence(Context->Device, *(Context->InFlightFences + i), null);
                    }
                    Allocator.Free(ref Context->SemaphoresRenderingDone,  Context->MaxFramesInFlight);
                    Allocator.Free(ref Context->SemaphoresImageAvailable, Context->MaxFramesInFlight);
                    Allocator.Free(ref Context->InFlightFences,           Context->MaxFramesInFlight);

                    Context->FrameInFlight = 0u;
                }

                FreeSubCommandBuffers();
                _subCommandBuffersCurrentSlot = 0;

                CleanupSwapChainInternal();

                if (Context->Swapchain != VkSwapchainKHR.Null)
                {
                    vkDestroySwapchainKHR(Context->Device, Context->Swapchain, null);
                    Context->Swapchain = VkSwapchainKHR.Null;
                }

                vkDestroyDevice(Context->Device, null);
                Context->Device = VkDevice.Null;
            }

            if (Context->SurfaceKhr != VkSurfaceKHR.Null)
            {
                vkDestroySurfaceKHR(Context->Instance, Context->SurfaceKhr, null);
                Context->SurfaceKhr = VkSurfaceKHR.Null;
            }
#if DEBUG
            if (Context->DebugUtilsMessengerExt != VkDebugUtilsMessengerEXT.Null)
            {
                vkDestroyDebugUtilsMessengerEXT(Context->Instance, Context->DebugUtilsMessengerExt, null);
                Context->DebugUtilsMessengerExt = VkDebugUtilsMessengerEXT.Null;
            }
#endif
            if (Context->Instance != VkInstance.Null)
            {
                vkDestroyInstance(Context->Instance, null);
                Context->Instance = VkInstance.Null;
            }

            _logger.Log(LogLevel.Information, null, $"[Cleanup] {nameof(Cleanup)} done...");
        }

        private bool RecreateSwapChainInternal(VkContext* context, VkSwapchainKHR oldSwapchainKhr)
        {
            _logger.Log(LogLevel.Debug, null, nameof(RecreateSwapChainInternal));
            if (context->Device != VkDevice.Null)
            {
                vkDeviceWaitIdle(context->Device)
                    .AssertVkResult();
            }

            if (context->Swapchain != VkSwapchainKHR.Null)
            {
                CleanupSwapChainInternal();
            }

            _swapchainConfiguration.OldSwapChain = oldSwapchainKhr;
            if (!CreateSwapchain(context, _swapchainConfiguration))
            {
                return false;
            }

            if (oldSwapchainKhr != VkSwapchainKHR.Null)
            {
                vkDestroySwapchainKHR(context->Device, oldSwapchainKhr, null);
            }

            if (!RetrieveSwapchainImages(context))
            {
                return false;
            }

            AllocateSubCommandBuffers(_context);

            if (!CreateRenderPass(context, _renderPassConfiguration))
            {
                return false;
            }

            if (!CreateDepthStencilImage(context))
            {
                return false;
            }

            if (!CreateFrameBuffers(context))
            {
                return false;
            }

            if (!CreateCommandPool(context->Device, context->QueueFamilyIndex, &context->CommandPool, VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT))
            {
                return false;
            }

            if (!CreateCommandPool(context->Device, context->QueueFamilyIndex, &context->ShortLivedCommandPool, VK_COMMAND_POOL_CREATE_TRANSIENT_BIT))
            {
                return false;
            }

            context->CommandBuffers = Allocator.Allocate<VkCommandBuffer>(context->SwapchainImageCount);
            if (!CreateCommandBuffers(context->Device, context->CommandPool, context->SwapchainImageCount, context->CommandBuffers))
            {
                return false;
            }

            context->ImagesInFlightFence = Allocator.Allocate<VkFence>(context->SwapchainImageCount, 0);

            SwapChainRecreated?.Invoke(this);

            return true;
        }

        private void CleanupSwapChainInternal()
        {
            CleanupSwapChain?.Invoke(this);

            if (Context->ImagesInFlightFence != null)
            {
                Allocator.Free(Context->ImagesInFlightFence, Context->SwapchainImageCount);
                Context->ImagesInFlightFence = null;
            }

            if (Context->ShortLivedCommandPool != VkCommandPool.Null)
            {
                vkDestroyCommandPool(Context->Device, Context->ShortLivedCommandPool, null);
                Context->ShortLivedCommandPool = VkCommandPool.Null;
            }

            if (Context->CommandPool != VkCommandPool.Null)
            {
                if (Context->CommandBuffers != null)
                {
                    try
                    {
                        vkFreeCommandBuffers(Context->Device, Context->CommandPool, Context->SwapchainImageCount, Context->CommandBuffers);
                    }
                    finally
                    {
                        Allocator.Free(ref Context->CommandBuffers, Context->SwapchainImageCount);
                    }
                }

                vkDestroyCommandPool(Context->Device, Context->CommandPool, null);
                Context->CommandPool = VkCommandPool.Null;
            }

            if (Context->Framebuffers != null)
            {
                try
                {
                    for (uint i = 0u; i < Context->SwapchainImageCount; i++)
                    {
                        vkDestroyFramebuffer(Context->Device, *(Context->Framebuffers + i), null);
                    }
                }
                finally
                {
                    Allocator.Free(ref Context->Framebuffers, Context->SwapchainImageCount);
                }
            }

            if (Context->DepthStencilImageView != VkImageView.Null)
            {
                vkDestroyImageView(Context->Device, Context->DepthStencilImageView, null);
                Context->DepthStencilImageView = VkImageView.Null;
            }

            if (Context->DepthStencilDeviceMemory != VkDeviceMemory.Null)
            {
                vkFreeMemory(Context->Device, Context->DepthStencilDeviceMemory, null);
                Context->DepthStencilDeviceMemory = VkDeviceMemory.Null;
            }

            if (Context->DepthStencilImage != VkImage.Null)
            {
                vkDestroyImage(Context->Device, Context->DepthStencilImage, null);
                Context->DepthStencilImage = VkImage.Null;
            }

            if (Context->RenderPass != VkRenderPass.Null)
            {
                vkDestroyRenderPass(Context->Device, Context->RenderPass, null);
                Context->RenderPass = VkRenderPass.Null;
            }

            if (Context->SwapchainImageViews != null)
            {
                try
                {
                    for (uint i = 0u; i < Context->SwapchainImageCount; i++)
                    {
                        vkDestroyImageView(Context->Device, *(Context->SwapchainImageViews + i), null);
                    }
                }
                finally
                {
                    Allocator.Free(ref Context->SwapchainImageViews, Context->SwapchainImageCount);
                }
            }

            if (Context->SwapchainImages != null)
            {
                Allocator.Free(ref Context->SwapchainImages, Context->SwapchainImageCount);
            }

            Context->SwapchainImageCount = 0;
        }


        #region IDisposable Support

        private bool _disposed;

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Cleanup();

                Allocator.Free(ref _context, 1u);

                _disposed = true;
            }
        }

        /// <summary> Finalizes an instance of the <see cref="Vulkan" /> class. </summary>
        ~Vulkan()
        {
            Dispose(false);
        }

        #endregion
    }
}
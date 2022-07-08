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
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Exomia.Vulkan.Api.Core.VkFormatFeatureFlagBits;
using static Exomia.Vulkan.Api.Core.VkFenceCreateFlagBits;
using static Exomia.Vulkan.Api.Core.VkCommandPoolCreateFlagBits;

namespace Exomia.Framework.Core.Vulkan;

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

    private VkContext* _context;

    private uint _requestedWidth;
    private uint _requestedHeight;

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
    /// <param name="debugUtilsMessengerConfiguration"> The debug utilities messenger configuration. </param>
    /// <param name="depthStencilConfiguration">        The depth stencil configuration. </param>
    /// <param name="deviceConfiguration">              The device configuration. </param>
    /// <param name="instanceConfiguration">            The instance configuration. </param>
    /// <param name="physicalDeviceConfiguration">      The physical device configuration. </param>
    /// <param name="queueConfiguration">               The queue configuration. </param>
    /// <param name="renderPassConfiguration">          The render pass configuration. </param>
    /// <param name="surfaceConfiguration">             The surface configuration. </param>
    /// <param name="swapchainConfiguration">           The swapchain configuration. </param>
    /// <param name="logger">                           The logger. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    public Vulkan(IRenderForm                                renderForm,
                  IOptions<ApplicationConfiguration>         applicationConfiguration,
                  IOptions<DebugUtilsMessengerConfiguration> debugUtilsMessengerConfiguration,
                  IOptions<DepthStencilConfiguration>        depthStencilConfiguration,
                  IOptions<DeviceConfiguration>              deviceConfiguration,
                  IOptions<InstanceConfiguration>            instanceConfiguration,
                  IOptions<PhysicalDeviceConfiguration>      physicalDeviceConfiguration,
                  IOptions<QueueConfiguration>               queueConfiguration,
                  IOptions<RenderPassConfiguration>          renderPassConfiguration,
                  IOptions<SurfaceConfiguration>             surfaceConfiguration,
                  IOptions<SwapchainConfiguration>           swapchainConfiguration,
                  ILogger<Vulkan>                            logger)
    {
        _applicationConfiguration         = applicationConfiguration.Value ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        _debugUtilsMessengerConfiguration = debugUtilsMessengerConfiguration.Value ?? throw new ArgumentNullException(nameof(debugUtilsMessengerConfiguration));
        _depthStencilConfiguration        = depthStencilConfiguration.Value ?? throw new ArgumentNullException(nameof(depthStencilConfiguration));
        _deviceConfiguration              = deviceConfiguration.Value ?? throw new ArgumentNullException(nameof(deviceConfiguration));
        _instanceConfiguration            = instanceConfiguration.Value ?? throw new ArgumentNullException(nameof(instanceConfiguration));
        _physicalDeviceConfiguration      = physicalDeviceConfiguration.Value ?? throw new ArgumentNullException(nameof(physicalDeviceConfiguration));
        _queueConfiguration               = queueConfiguration.Value ?? throw new ArgumentNullException(nameof(queueConfiguration));
        _renderPassConfiguration          = renderPassConfiguration.Value ?? throw new ArgumentNullException(nameof(renderPassConfiguration));
        _surfaceConfiguration             = surfaceConfiguration.Value ?? throw new ArgumentNullException(nameof(surfaceConfiguration));
        _swapchainConfiguration           = swapchainConfiguration.Value ?? throw new ArgumentNullException(nameof(swapchainConfiguration));
        _logger                           = logger ?? throw new ArgumentNullException(nameof(logger));

        *(_context = Allocator.Allocate<VkContext>(1u)) = VkContext.Create();

        renderForm.Resized += form =>
        {
            _requestedWidth             = (uint)form.Width;
            _requestedHeight            = (uint)form.Height;
            Context->FramebufferResized = true;
        };

        /* MODULE INITIALIZE */
        _moduleFreeIndices = new Stack<uint>(8);
        _modulesLookup     = new Dictionary<ushort, uint>(8);
        _modules           = Allocator.Allocate<VkModule>(_modulesCount);
    }

    private bool CreateSyncObjects(VkContext* context)
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

    internal bool Initialize()
    {
        using (_logger.BeginScope("Initializing..."))
        {
            if (!CreateInstance(Context, _applicationConfiguration, _instanceConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(CreateInstance));
                return false;
            }

            VkKhrGetSurfaceCapabilities2.Load(Context->Instance);

#if DEBUG
            if (!SetupDebugCallback(Context, _debugUtilsMessengerConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(SetupDebugCallback));
                return false;
            }
#endif

            if (_surfaceConfiguration.CreateSurface != null && !_surfaceConfiguration.CreateSurface(Context))
            {
                _logger.LogCritical("{method} failed!", nameof(_surfaceConfiguration.CreateSurface));
                return false;
            }

            VkKhrSurface.Load(Context->Instance);

            if (!PickBestPhysicalDevice(Context, _physicalDeviceConfiguration, _deviceConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(PickBestPhysicalDevice));
                return false;
            }

            if (!CreateDevice(Context, _deviceConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(CreateDevice));
                return false;
            }

            if (!RetrieveDeviceQueue(Context, _queueConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(RetrieveDeviceQueue));
                return false;
            }

            if (!GetSuitableDepthStencilFormat(
                    Context,
                    _depthStencilConfiguration.Formats,
                    _depthStencilConfiguration.Tiling,
                    VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT,
                    out Context->DepthStencilFormat))
            {
                _logger.LogCritical("The system doesn't support one of the specified depth stencil formats ({formats})!", string.Join(',', _depthStencilConfiguration.Formats));
                return false;
            }

            VkKhrSwapchain.Load(Context->Device);

            if (!RecreateSwapChainInternal(Context, VkSwapchainKHR.Null))
            {
                _logger.LogCritical("{method} failed!", nameof(RecreateSwapChainInternal));
                return false;
            }

            if (!CreateSyncObjects(Context))
            {
                _logger.LogCritical("{method} failed!", nameof(CreateSyncObjects));
                return false;
            }

            _moduleCommandBuffers = Allocator.AllocatePtr<VkCommandBuffer>(_context->MaxFramesInFlight);
            for (uint i = 0; i < _context->MaxFramesInFlight; i++)
            {
                *(_moduleCommandBuffers + i) = Allocator.Allocate<VkCommandBuffer>(_modulesCount);
            }
            _moduleCommandBuffersAreDirty = Math2.SetOnes(_context->MaxFramesInFlight);

            _logger.LogInformation("Vulkan {method} successfully.", nameof(Initialize));
            return true;
        }
    }

    internal void Cleanup()
    {
        if (Context->Device != VkDevice.Null)
        {
            _logger.LogInformation("[Cleanup] Wait for device idle...");
            _logger.LogInformation("[Cleanup] device idle: {state}", vkDeviceWaitIdle(Context->Device));

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

        _logger.LogInformation("[Cleanup] {method} done...", nameof(Cleanup));
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

            /* MODULE DISPOSE */
            Allocator.Free<VkModule>(ref _modules, _modulesCount);
            _modulesLookup.Clear();
            _moduleFreeIndices.Clear();

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
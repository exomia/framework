#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkFormatFeatureFlagBits;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkImageType;
using static Exomia.Vulkan.Api.Core.VkImageTiling;
using static Exomia.Vulkan.Api.Core.VkImageUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkImageViewType;
using static Exomia.Vulkan.Api.Core.VkImageLayout;
using static Exomia.Vulkan.Api.Core.VkComponentSwizzle;
using static Exomia.Vulkan.Api.Core.VkSharingMode;
using static Exomia.Vulkan.Api.Core.VkMemoryPropertyFlagBits;
using static Exomia.Vulkan.Api.Core.VkFenceCreateFlagBits;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan;

/// <summary> A swapchain. This class cannot be inherited. </summary>
public sealed unsafe partial class Swapchain : IDisposable
{
    private readonly SwapchainConfiguration    _swapchainConfiguration;
    private readonly DepthStencilConfiguration _depthStencilConfiguration;
    private readonly RenderPassConfiguration   _renderPassConfiguration;

    private readonly VkContext*        _vkContext;
    private          VkSwapchainKHR    _swapchain;
    private          SwapchainContext* _context;
    private          RenderPass        _renderPass = null!;

    private uint _firstSubmitInFrame = 0u;

    private bool _framebufferShouldResize;
    private uint _requestedWidth;
    private uint _requestedHeight;

    /// <summary> Gets the vk context. </summary>
    /// <value> The vk context. </value>
    public VkContext* VkContext
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _vkContext; }
    }

    /// <summary> Gets the swapchain context. </summary>
    /// <value> The swapchain context. </value>
    public SwapchainContext* Context
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _context; }
    }

    /// <summary> Gets the render pass. </summary>
    /// <value> The render pass. </value>
    public RenderPass RenderPass
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _renderPass; }
    }

    /// <summary> Initializes a new instance of the <see cref="Swapchain" /> class. </summary>
    /// <param name="vkContext">                 The vk context. </param>
    /// <param name="swapchainConfiguration">    The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The depth stencil configuration. </param>
    /// <param name="renderPassConfiguration">  The render pass configurations. </param>
    /// <param name="oldSwapchain">              (Optional) The old swapchain. </param>
    public Swapchain(
        VkContext*                vkContext,
        SwapchainConfiguration    swapchainConfiguration,
        DepthStencilConfiguration depthStencilConfiguration,
        RenderPassConfiguration   renderPassConfiguration,
        Swapchain?                oldSwapchain = null)
    {
        _vkContext                 = vkContext;
        _swapchainConfiguration    = swapchainConfiguration;
        _depthStencilConfiguration = depthStencilConfiguration;
        _renderPassConfiguration   = renderPassConfiguration;

        *(_context = Allocator.Allocate<SwapchainContext>(1u)) = new SwapchainContext
        {
            MaxFramesInFlight = swapchainConfiguration.MaxFramesInFlight,
            Width             = (_requestedWidth  = _vkContext->InitialWidth),
            Height            = (_requestedHeight = _vkContext->InitialHeight)
        };

        InitializeModules();

        CreateSwapChain(oldSwapchain);

        Initialize();
    }

    /// <summary> Implicit cast that converts the given <see cref="Swapchain" /> to a <see cref="VkSwapchainKHR" />. </summary>
    /// <param name="swapchain"> The swapchain. </param>
    /// <returns> The result of the operation. </returns>
    public static implicit operator VkSwapchainKHR(Swapchain swapchain)
    {
        return swapchain._swapchain;
    }

    /// <summary> Resize the swapchain to specified <paramref name="width" /> and <paramref name="height" />. </summary>
    /// <param name="width">  The width. </param>
    /// <param name="height"> The height. </param>
    public void Resize(uint width, uint height)
    {
        _requestedWidth          = width;
        _requestedHeight         = height;
        _framebufferShouldResize = true;
    }

    private void Recreate()
    {
        Console.WriteLine("recreating swapchain...");

        Cleanup();

        CreateSwapChain(this);
        Initialize();

        SwapChainRecreated?.Invoke(this, _vkContext);
    }

    /// <summary> Creates a new <see cref="Swapchain" />. </summary>
    /// <param name="oldSwapchain">            (Optional) The old swapchain. </param>
    /// <returns> A <see cref="Swapchain" />. </returns>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    /// <exception cref="VulkanException">   Thrown when a Vulkan error condition occurs. </exception>
    private void CreateSwapChain(Swapchain? oldSwapchain = null)
    {
        if (_swapchainConfiguration.BeforeSwapchainCreation != null &&
            !_swapchainConfiguration.BeforeSwapchainCreation(_vkContext))
        {
            throw new VulkanException("The before swapchain creation failed!");
        }

        VkSurfaceCapabilities2KHR surfaceCapabilities2Khr;
        surfaceCapabilities2Khr.sType = VkSurfaceCapabilities2KHR.STYPE;
        surfaceCapabilities2Khr.pNext = null;

        VkPhysicalDeviceSurfaceInfo2KHR physicalDeviceSurfaceInfo2Khr;
        physicalDeviceSurfaceInfo2Khr.sType   = VkPhysicalDeviceSurfaceInfo2KHR.STYPE;
        physicalDeviceSurfaceInfo2Khr.pNext   = null;
        physicalDeviceSurfaceInfo2Khr.surface = _vkContext->SurfaceKhr;

        VkResult result = vkGetPhysicalDeviceSurfaceCapabilities2KHR(_vkContext->PhysicalDevice, &physicalDeviceSurfaceInfo2Khr, &surfaceCapabilities2Khr);

        result.AssertVkResult();

        if (!Vulkan.GetSuitableImageFormat(
                _vkContext,
                _swapchainConfiguration.ImageFormats,
                &physicalDeviceSurfaceInfo2Khr,
                out VkSurfaceFormatKHR surfaceFormatKhr))
        {
            throw new VulkanException(
                "The system doesn't support one of the specified image formats ({0})!",
                string.Join(',', _swapchainConfiguration.ImageFormats));
        }

        if (!Vulkan.GetSuitablePresentMode(
                _vkContext,
                _swapchainConfiguration.PresentModes,
                out VkPresentModeKHR presentModeKhr))
        {
            throw new VulkanException(
                "The system doesn't support one of the specified present modes ({0})!",
                string.Join(',', _swapchainConfiguration.PresentModes));
        }

        if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedTransforms & _swapchainConfiguration.PreTransform)
            != _swapchainConfiguration.PreTransform)
        {
            throw new VulkanException(
                "The system doesn't support the pre transform '{0}' (supported: {1})!",
                _swapchainConfiguration.PreTransform,
                surfaceCapabilities2Khr.surfaceCapabilities.supportedTransforms);
        }

        if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedCompositeAlpha & _swapchainConfiguration.CompositeAlpha)
            != _swapchainConfiguration.CompositeAlpha)
        {
            throw new VulkanException(
                "The system doesn't support the composite alpha '{0}' (supported: {1})!",
                _swapchainConfiguration.CompositeAlpha,
                surfaceCapabilities2Khr.surfaceCapabilities.supportedCompositeAlpha);
        }

        if ((surfaceCapabilities2Khr.surfaceCapabilities.supportedUsageFlags & VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT)
            != VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT)
        {
            throw new VulkanException(
                "The system doesn't support the usage flag '{0}' (supported: {1})!",
                VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
                surfaceCapabilities2Khr.surfaceCapabilities.supportedUsageFlags);
        }

        VkSwapchainCreateInfoKHR swapchainCreateInfoKhr;
        swapchainCreateInfoKhr.sType   = VkSwapchainCreateInfoKHR.STYPE;
        swapchainCreateInfoKhr.pNext   = _swapchainConfiguration.Next;
        swapchainCreateInfoKhr.flags   = 0;
        swapchainCreateInfoKhr.surface = _vkContext->SurfaceKhr;
        swapchainCreateInfoKhr.minImageCount = Math.Clamp(
            _swapchainConfiguration.MinImageCount,
            surfaceCapabilities2Khr.surfaceCapabilities.minImageCount,
            surfaceCapabilities2Khr.surfaceCapabilities.maxImageCount);
        swapchainCreateInfoKhr.imageFormat     = _context->Format = surfaceFormatKhr.format;
        swapchainCreateInfoKhr.imageColorSpace = surfaceFormatKhr.colorSpace;
        swapchainCreateInfoKhr.imageExtent.width = _context->Width = Math.Max(
            Math.Min(_requestedWidth, surfaceCapabilities2Khr.surfaceCapabilities.maxImageExtent.width),
            surfaceCapabilities2Khr.surfaceCapabilities.minImageExtent.width);
        swapchainCreateInfoKhr.imageExtent.height = _context->Height = Math.Max(
            Math.Min(_requestedHeight, surfaceCapabilities2Khr.surfaceCapabilities.maxImageExtent.height),
            surfaceCapabilities2Khr.surfaceCapabilities.minImageExtent.height);
        swapchainCreateInfoKhr.imageArrayLayers = Math.Min(
            _swapchainConfiguration.ImageArrayLayers,
            surfaceCapabilities2Khr.surfaceCapabilities.maxImageArrayLayers);
        swapchainCreateInfoKhr.imageUsage            = _swapchainConfiguration.ImageUsage;
        swapchainCreateInfoKhr.imageSharingMode      = _swapchainConfiguration.ImageSharingMode;
        swapchainCreateInfoKhr.queueFamilyIndexCount = _swapchainConfiguration.QueueFamilyIndexCount;
        swapchainCreateInfoKhr.pQueueFamilyIndices   = _swapchainConfiguration.QueueFamilyIndices;
        swapchainCreateInfoKhr.preTransform          = _swapchainConfiguration.PreTransform;
        swapchainCreateInfoKhr.compositeAlpha        = _swapchainConfiguration.CompositeAlpha;
        swapchainCreateInfoKhr.presentMode           = presentModeKhr;
        swapchainCreateInfoKhr.clipped               = _swapchainConfiguration.Clipped;
        swapchainCreateInfoKhr.oldSwapchain          = oldSwapchain ?? VkSwapchainKHR.Null;

        VkSwapchainKHR swapchainKhr;
        vkCreateSwapchainKHR(_vkContext->Device, &swapchainCreateInfoKhr, null, &swapchainKhr)
            .AssertVkResult();

        if (_swapchainConfiguration.AfterSwapchainCreation != null &&
            !_swapchainConfiguration.AfterSwapchainCreation(_vkContext))
        {
            throw new VulkanException("The after swapchain creation failed!");
        }

        _swapchain = swapchainKhr;
    }

    private void Initialize()
    {
        CreateImageViews();

        if (!Vulkan.GetSuitableDepthStencilFormat(
                _vkContext->PhysicalDevice,
                _depthStencilConfiguration.Formats,
                _depthStencilConfiguration.Tiling,
                VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT,
                out _context->DepthStencilFormat))
        {
            throw new VulkanException(
                "The system doesn't support one of the specified depth stencil formats ({0})!",
                string.Join(',', _depthStencilConfiguration.Formats));
        }

        CreateDepthResources();
        CreateRenderPass();
        CreateFrameBuffers();
        CreateSyncObjects();

        _moduleCommandBuffers = Allocator.AllocatePtr<VkCommandBuffer>(_context->MaxFramesInFlight);
        for (uint i = 0; i < _context->MaxFramesInFlight; i++)
        {
            *(_moduleCommandBuffers + i) = Allocator.Allocate<VkCommandBuffer>(_modulesCount);
        }
        _moduleCommandBuffersAreDirty = 0u;
    }

    private void CreateImageViews()
    {
        vkGetSwapchainImagesKHR(_vkContext->Device, _swapchain, &_context->SwapchainImageCount, null)
            .AssertVkResult();

        _context->SwapchainImages = Allocator.Allocate<VkImage>(_context->SwapchainImageCount);

        vkGetSwapchainImagesKHR(_vkContext->Device, _swapchain, &_context->SwapchainImageCount, _context->SwapchainImages)
            .AssertVkResult();

        VkImageViewCreateInfo imageViewCreateInfo;
        imageViewCreateInfo.sType                           = VkImageViewCreateInfo.STYPE;
        imageViewCreateInfo.pNext                           = null;
        imageViewCreateInfo.flags                           = 0u;
        imageViewCreateInfo.viewType                        = VK_IMAGE_VIEW_TYPE_2D;
        imageViewCreateInfo.format                          = _context->Format;
        imageViewCreateInfo.components.r                    = VK_COMPONENT_SWIZZLE_IDENTITY;
        imageViewCreateInfo.components.g                    = VK_COMPONENT_SWIZZLE_IDENTITY;
        imageViewCreateInfo.components.b                    = VK_COMPONENT_SWIZZLE_IDENTITY;
        imageViewCreateInfo.components.a                    = VK_COMPONENT_SWIZZLE_IDENTITY;
        imageViewCreateInfo.subresourceRange.aspectMask     = Vulkan.GetImageAspectMask(_context->Format);
        imageViewCreateInfo.subresourceRange.baseMipLevel   = 0u;
        imageViewCreateInfo.subresourceRange.levelCount     = 1u;
        imageViewCreateInfo.subresourceRange.baseArrayLayer = 0u;
        imageViewCreateInfo.subresourceRange.layerCount     = 1u;

        _context->SwapchainImageViews = Allocator.Allocate<VkImageView>(_context->SwapchainImageCount);

        for (uint i = 0u; i < _context->SwapchainImageCount; i++)
        {
            imageViewCreateInfo.image = *(_context->SwapchainImages + i);

            vkCreateImageView(_vkContext->Device, &imageViewCreateInfo, null, _context->SwapchainImageViews + i)
                .AssertVkResult();
        }
    }

    private void CreateDepthResources()
    {
        //TODO: give the user the choice to disable depth / stencil
        if (_context->DepthStencilFormat == VK_FORMAT_UNDEFINED)
        {
            throw new Exception("Can't create depth resources!");
        }

        VkImageCreateInfo imageCreateInfo;
        imageCreateInfo.sType                 = VkImageCreateInfo.STYPE;
        imageCreateInfo.pNext                 = null;
        imageCreateInfo.flags                 = 0u;
        imageCreateInfo.imageType             = VK_IMAGE_TYPE_2D;
        imageCreateInfo.format                = _context->DepthStencilFormat;
        imageCreateInfo.extent.width          = _context->Width;
        imageCreateInfo.extent.height         = _context->Height;
        imageCreateInfo.extent.depth          = 1u;
        imageCreateInfo.mipLevels             = 1u;
        imageCreateInfo.arrayLayers           = 1u;
        imageCreateInfo.samples               = VK_SAMPLE_COUNT_1_BIT;
        imageCreateInfo.tiling                = VK_IMAGE_TILING_OPTIMAL;
        imageCreateInfo.usage                 = VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT;
        imageCreateInfo.sharingMode           = VK_SHARING_MODE_EXCLUSIVE;
        imageCreateInfo.queueFamilyIndexCount = 0u;
        imageCreateInfo.pQueueFamilyIndices   = null;
        imageCreateInfo.initialLayout         = VK_IMAGE_LAYOUT_UNDEFINED;

        vkCreateImage(_vkContext->Device, &imageCreateInfo, null, &_context->DepthStencilImage)
            .AssertVkResult();

        VkImageMemoryRequirementsInfo2 imageMemoryRequirementsInfo2;
        imageMemoryRequirementsInfo2.sType = VkImageMemoryRequirementsInfo2.STYPE;
        imageMemoryRequirementsInfo2.pNext = null;
        imageMemoryRequirementsInfo2.image = _context->DepthStencilImage;

        VkMemoryRequirements2 memoryRequirements2;
        memoryRequirements2.sType = VkMemoryRequirements2.STYPE;
        memoryRequirements2.pNext = null;

        vkGetImageMemoryRequirements2(_vkContext->Device, &imageMemoryRequirementsInfo2, &memoryRequirements2);

        VkMemoryAllocateInfo memoryAllocateInfo;
        memoryAllocateInfo.sType          = VkMemoryAllocateInfo.STYPE;
        memoryAllocateInfo.pNext          = null;
        memoryAllocateInfo.allocationSize = memoryRequirements2.memoryRequirements.size;
        memoryAllocateInfo.memoryTypeIndex = Vulkan.FindMemoryTypeIndex(
            _vkContext->PhysicalDevice,
            memoryRequirements2.memoryRequirements.memoryTypeBits,
            VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

        vkAllocateMemory(_vkContext->Device, &memoryAllocateInfo, null, &_context->DepthStencilDeviceMemory)
            .AssertVkResult();

        VkBindImageMemoryInfo bindImageMemoryInfo;
        bindImageMemoryInfo.sType        = VkBindImageMemoryInfo.STYPE;
        bindImageMemoryInfo.pNext        = null;
        bindImageMemoryInfo.image        = _context->DepthStencilImage;
        bindImageMemoryInfo.memory       = _context->DepthStencilDeviceMemory;
        bindImageMemoryInfo.memoryOffset = VkDeviceSize.Zero;

        vkBindImageMemory2(_vkContext->Device, 1u, &bindImageMemoryInfo)
            .AssertVkResult();

        Vulkan.CreateImageView(
            _vkContext->Device, 
            _context->DepthStencilImage, 
            &_context->DepthStencilImageView,
            _context->DepthStencilFormat, 
            Vulkan.GetImageAspectMask(_context->DepthStencilFormat));
    }

    private void CreateRenderPass()
    {
        _renderPass = RenderPass.Create(_vkContext, _context, _renderPassConfiguration);
    }

    private void CreateFrameBuffers()
    {
        if (_context->SwapchainImageCount   == 0u ||
            _context->DepthStencilImageView == VkImageView.Null)
        {
            throw new Exception("Can't create framebuffers!");
        }

        _context->Framebuffers = Allocator.Allocate<VkFramebuffer>(_context->SwapchainImageCount);

        VkImageView* pImageViews = stackalloc VkImageView[2];
        *(pImageViews + 1) = _context->DepthStencilImageView;

        for (uint i = 0u; i < _context->SwapchainImageCount; i++)
        {
            *pImageViews = *(_context->SwapchainImageViews + i);

            VkFramebufferCreateInfo framebufferCreateInfo;
            framebufferCreateInfo.sType           = VkFramebufferCreateInfo.STYPE;
            framebufferCreateInfo.pNext           = null;
            framebufferCreateInfo.flags           = 0;
            framebufferCreateInfo.renderPass      = _renderPass;
            framebufferCreateInfo.attachmentCount = 2u;
            framebufferCreateInfo.pAttachments    = pImageViews;
            framebufferCreateInfo.width           = _context->Width;
            framebufferCreateInfo.height          = _context->Height;
            framebufferCreateInfo.layers          = 1u;

            vkCreateFramebuffer(_vkContext->Device, &framebufferCreateInfo, null, _context->Framebuffers + i)
                .AssertVkResult();
        }
    }

    private void CreateSyncObjects()
    {
        VkFenceCreateInfo fenceCreateInfo;
        fenceCreateInfo.sType = VkFenceCreateInfo.STYPE;
        fenceCreateInfo.pNext = null;
        fenceCreateInfo.flags = VK_FENCE_CREATE_SIGNALED_BIT;

        VkSemaphoreCreateInfo semaphoreCreateInfo;
        semaphoreCreateInfo.sType = VkSemaphoreCreateInfo.STYPE;
        semaphoreCreateInfo.sType = VkSemaphoreCreateInfo.STYPE;
        semaphoreCreateInfo.pNext = null;
        semaphoreCreateInfo.flags = 0;

        _context->QueuesFences             = Allocator.AllocatePtr<VkFence>(_context->MaxFramesInFlight);
        _context->InFlightFences           = Allocator.Allocate<VkFence>(_context->MaxFramesInFlight);
        _context->SemaphoresImageAvailable = Allocator.Allocate<VkSemaphore>(_context->MaxFramesInFlight);
        _context->SemaphoresRenderingDone  = Allocator.Allocate<VkSemaphore>(_context->MaxFramesInFlight);

        for (uint i = 0u; i < _context->MaxFramesInFlight; i++)
        {
            *(_context->QueuesFences + i) = Allocator.Allocate<VkFence>(_vkContext->QueuesCount);
            for (uint q = 0u; q < _vkContext->QueuesCount; q++)
            {
                vkCreateFence(_vkContext->Device, &fenceCreateInfo, null, *(_context->QueuesFences + i) + q)
                    .AssertVkResult();
            }
            vkCreateFence(_vkContext->Device, &fenceCreateInfo, null, _context->InFlightFences + i)
                .AssertVkResult();
            vkCreateSemaphore(_vkContext->Device, &semaphoreCreateInfo, null, _context->SemaphoresImageAvailable + i)
                .AssertVkResult();
            vkCreateSemaphore(_vkContext->Device, &semaphoreCreateInfo, null, _context->SemaphoresRenderingDone + i)
                .AssertVkResult();
        }

        _context->ImagesInFlightFence = Allocator.Allocate<VkFence>(_context->SwapchainImageCount, 0);
    }

    private void Cleanup()
    {
        CleanupSwapChain?.Invoke(this, _vkContext);

        if (_moduleCommandBuffers != null)
        {
            for (uint i = 0; i < _context->MaxFramesInFlight; i++)
            {
                Allocator.Free(ref *(_moduleCommandBuffers + i), _modulesCount);
            }

            Allocator.FreePtr(ref _moduleCommandBuffers, _context->MaxFramesInFlight);
        }

        if (_vkContext->Device != VkDevice.Null)
        {
            vkDeviceWaitIdle(_vkContext->Device)
                .AssertVkResult();

            if (_context->InFlightFences != null)
            {
                for (uint i = 0u; i < _context->MaxFramesInFlight; i++)
                {
                    vkDestroySemaphore(_vkContext->Device, *(_context->SemaphoresRenderingDone  + i), null);
                    vkDestroySemaphore(_vkContext->Device, *(_context->SemaphoresImageAvailable + i), null);
                    vkDestroyFence(_vkContext->Device, *(_context->InFlightFences               + i), null);

                    for (uint q = 0u; q < _vkContext->QueuesCount; q++)
                    {
                        vkDestroyFence(_vkContext->Device, *(*(_context->QueuesFences + i) + q), null);
                    }
                    Allocator.Free(ref *(_context->QueuesFences + i), _vkContext->QueuesCount);
                }
                Allocator.Free(ref _context->SemaphoresRenderingDone,  _context->MaxFramesInFlight);
                Allocator.Free(ref _context->SemaphoresImageAvailable, _context->MaxFramesInFlight);
                Allocator.Free(ref _context->InFlightFences,           _context->MaxFramesInFlight);
                Allocator.FreePtr(ref _context->QueuesFences, _context->MaxFramesInFlight);
            }

            Allocator.Free(ref _context->ImagesInFlightFence, _context->SwapchainImageCount);

            if (_context->Framebuffers != null)
            {
                for (uint i = 0u; i < _context->SwapchainImageCount; i++)
                {
                    vkDestroyFramebuffer(_vkContext->Device, *(_context->Framebuffers + i), null);
                }
                Allocator.Free(ref _context->Framebuffers, _context->SwapchainImageCount);
            }

            _renderPass.Dispose();

            if (_context->DepthStencilImageView != VkImageView.Null)
            {
                vkDestroyImageView(_vkContext->Device, _context->DepthStencilImageView, null);
                _context->DepthStencilImageView = VkImageView.Null;
            }

            if (_context->DepthStencilDeviceMemory != VkDeviceMemory.Null)
            {
                vkFreeMemory(_vkContext->Device, _context->DepthStencilDeviceMemory, null);
                _context->DepthStencilDeviceMemory = VkDeviceMemory.Null;
            }

            if (_context->DepthStencilImage != VkImage.Null)
            {
                vkDestroyImage(_vkContext->Device, _context->DepthStencilImage, null);
                _context->DepthStencilImage = VkImage.Null;
            }

            if (_context->SwapchainImageViews != null)
            {
                for (uint i = 0; i < _context->SwapchainImageCount; i++)
                {
                    vkDestroyImageView(_vkContext->Device, *(_context->SwapchainImageViews + i), null);
                }

                Allocator.Free(ref _context->SwapchainImageViews, _context->SwapchainImageCount);
            }

            if (_context->SwapchainImages != null)
            {
                Allocator.Free(ref _context->SwapchainImages, _context->SwapchainImageCount);
            }

            if (_swapchain != VkSwapchainKHR.Null)
            {
                vkDestroySwapchainKHR(_vkContext->Device, _swapchain, null);
                _swapchain = VkSwapchainKHR.Null;
            }
        }
    }

#if DEBUG
    private bool _isFrameStarted = false;
    public bool IsFrameStarted
    {
        get { return _isFrameStarted; }
    }
#endif

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            CleanupModules();

            if (_context != null)
            {
                Cleanup();

                Allocator.Free(ref _context, 1u);
            }
        }
        GC.SuppressFinalize(this);
    }

    /// <summary> Finalizes an instance of the <see cref="Swapchain" /> class. </summary>
    ~Swapchain()
    {
        Dispose();
    }

    #endregion
}
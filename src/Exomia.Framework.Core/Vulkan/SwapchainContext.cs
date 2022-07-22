#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan;

#pragma warning disable 1591
/// <summary> A swapchain context. </summary>
public unsafe struct SwapchainContext
{
    public uint Width;
    public uint Height;
    public uint MaxFramesInFlight;
    public uint ImageIndex;
    public uint FrameInFlight;
    public uint SwapchainImageCount;

    public VkFormat     Format;
    public VkImage*     SwapchainImages;
    public VkImageView* SwapchainImageViews;

    public VkFormat       DepthStencilFormat;
    public VkImage        DepthStencilImage;
    public VkDeviceMemory DepthStencilDeviceMemory;
    public VkImageView    DepthStencilImageView;

    public VkFramebuffer* Framebuffers;
    public VkSemaphore*   SemaphoresImageAvailable;
    public VkSemaphore*   SemaphoresRenderingDone;
    public VkFence*       InFlightFences;
    public VkFence*       ImagesInFlightFence;
    public VkFence**      QueuesFences;
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkImageUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkSharingMode;
using static Exomia.Vulkan.Api.Core.VkSurfaceTransformFlagBitsKHR;
using static Exomia.Vulkan.Api.Core.VkPresentModeKHR;
using static Exomia.Vulkan.Api.Core.VkCompositeAlphaFlagBitsKHR;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A swapchain configuration. This class cannot be inherited. </summary>
public sealed unsafe class SwapchainConfiguration
{
    public void*                      Next                    { get; set; } = null;
    public uint                       MinImageCount           { get; set; } = 3;
    public VkFormat[]                 ImageFormats            { get; set; } = { VK_FORMAT_B8G8R8A8_SRGB, VK_FORMAT_B8G8R8A8_UNORM };
    public uint                       ImageArrayLayers        { get; set; } = 1;
    public VkImageUsageFlagBits       ImageUsage              { get; set; } = VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT;
    public VkSharingMode              ImageSharingMode        { get; set; } = VK_SHARING_MODE_EXCLUSIVE;
    public uint                       QueueFamilyIndexCount   { get; set; } = 0;
    public uint*                      QueueFamilyIndices      { get; set; } = null;
    public VkSurfaceTransformFlagsKHR PreTransform            { get; set; } = VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR;
    public VkPresentModeKHR[]         PresentModes            { get; set; } = { VK_PRESENT_MODE_MAILBOX_KHR, VK_PRESENT_MODE_FIFO_KHR };
    public VkCompositeAlphaFlagsKHR   CompositeAlpha          { get; set; } = VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR;
    public VkBool32                   Clipped                 { get; set; } = VkBool32.True;
    public SetupContextHandler?       BeforeSwapchainCreation { get; set; } = null;
    public SetupContextHandler?       AfterSwapchainCreation  { get; set; } = null;
    public uint                       MaxFramesInFlight       { get; set; } = 2;
}
#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> A swapchain configuration. This class cannot be inherited. </summary>
    public sealed unsafe class SwapchainConfiguration
    {
        public        void*                      Next                        { get; set; } = null;
        public        uint                       MinImageCount               { get; set; } = 2;
        public        VkFormat[]                 ImageFormats                { get; set; } = {VkFormat.B8G8R8A8_SRGB, VkFormat.B8G8R8A8_UNORM};
        public        uint                       ImageArrayLayers            { get; set; } = 1;
        public        VkImageUsageFlagBits       ImageUsage                  { get; set; } = VkImageUsageFlagBits.COLOR_ATTACHMENT_BIT;
        public        VkSharingMode              ImageSharingMode            { get; set; } = VkSharingMode.EXCLUSIVE;
        public        uint                       QueueFamilyIndexCount       { get; set; } = 0;
        public unsafe uint*                      QueueFamilyIndices          { get; set; } = null;
        public        VkSurfaceTransformFlagsKHR PreTransform                { get; set; } = VkSurfaceTransformFlagsKHR.IDENTITY_BIT_KHR;
        public        VkPresentModeKHR[]         PresentModes                { get; set; } = {VkPresentModeKHR.MAILBOX_KHR, VkPresentModeKHR.FIFO_KHR};
        public        VkCompositeAlphaFlagsKHR   CompositeAlpha              { get; set; } = VkCompositeAlphaFlagsKHR.OPAQUE_BIT_KHR;
        public        VkBool32                   Clipped                     { get; set; } = VkBool32.True;
        public        SetupContextHandler?       BeginSwapchainCreation      { get; set; } = null;
        public        SetupContextHandler?       SwapchainCreationSuccessful { get; set; } = null;
        public        VkSwapchainKHR             OldSwapChain                { get; set; } = VkSwapchainKHR.Null;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan
{
    /// <summary> A vk context. </summary>
    public unsafe struct VkContext
    {
        public   VkInstance               Instance;
        public   VkDebugUtilsMessengerEXT DebugUtilsMessengerExt;
        public   VkSurfaceKHR             SurfaceKhr;
        internal uint                     Width;
        internal uint                     Height;
        public   VkPhysicalDevice         PhysicalDevice;
        public   VkSampleCountFlagBits    SupportedSampleCountFlags;
        public   uint                     QueueFamilyIndex;
        public   uint                     MaxQueueCount;
        public   VkDevice                 Device;
        public   VkQueue                  Queue;
        public   VkSwapchainKHR           Swapchain;
        public   VkFormat                 Format;
        public   uint                     SwapchainImageCount;
        public   VkImage*                 SwapchainImages;
        public   VkImageView*             SwapchainImageViews;
        public   VkFormat                 DepthStencilFormat;
        public   VkImage                  DepthStencilImage;
        public   VkDeviceMemory           DepthStencilDeviceMemory;
        public   VkImageView              DepthStencilImageView;
        public   VkRenderPass             RenderPass;
        public   VkFramebuffer*           Framebuffers;
        public   VkCommandPool            CommandPool;
        public   VkCommandPool            ShortLivedCommandPool;
        public   VkCommandBuffer*         CommandBuffers;
        public   VkSemaphore*             SemaphoresImageAvailable;
        public   VkSemaphore*             SemaphoresRenderingDone;
        internal uint                     MaxFramesInFlight;
        internal uint                     FrameInFlight;
        public   VkFence*                 InFlightFences;
        public   VkFence*                 ImagesInFlightFence;
        internal uint                     ImageIndex;
        internal bool                     FramebufferResized;

        internal static VkContext Create()
        {
            VkContext context;
            context.Instance                  = VkInstance.Null;
            context.DebugUtilsMessengerExt    = VkDebugUtilsMessengerEXT.Null;
            context.SurfaceKhr                = VkSurfaceKHR.Null;
            context.Width                     = 0u;
            context.Height                    = 0u;
            context.PhysicalDevice            = VkPhysicalDevice.Null;
            context.SupportedSampleCountFlags = VK_SAMPLE_COUNT_1_BIT;
            context.QueueFamilyIndex          = uint.MaxValue;
            context.MaxQueueCount             = 0u;
            context.Device                    = VkDevice.Null;
            context.Queue                     = VkQueue.Null;
            context.Swapchain                 = VkSwapchainKHR.Null;
            context.Format                    = VK_FORMAT_UNDEFINED;
            context.SwapchainImageCount       = 0u;
            context.SwapchainImages           = null;
            context.SwapchainImageViews       = null;
            context.DepthStencilFormat        = VK_FORMAT_UNDEFINED;
            context.DepthStencilImage         = VkImage.Null;
            context.DepthStencilDeviceMemory  = VkDeviceMemory.Null;
            context.DepthStencilImageView     = VkImageView.Null;
            context.RenderPass                = VkRenderPass.Null;
            context.Framebuffers              = null;
            context.CommandPool               = VkCommandPool.Null;
            context.ShortLivedCommandPool     = VkCommandPool.Null;
            context.CommandBuffers            = null;
            context.SemaphoresImageAvailable  = null;
            context.SemaphoresRenderingDone   = null;
            context.MaxFramesInFlight         = 2u;
            context.FrameInFlight             = 0u;
            context.InFlightFences            = null;
            context.ImagesInFlightFence       = null;
            context.ImageIndex                = uint.MaxValue;
            context.FramebufferResized        = false;
            return context;
        }
    }
}
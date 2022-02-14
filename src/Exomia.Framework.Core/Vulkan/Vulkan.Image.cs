#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Allocators;
using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkImageType;
using static Exomia.Vulkan.Api.Core.VkImageTiling;
using static Exomia.Vulkan.Api.Core.VkImageUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkImageViewType;
using static Exomia.Vulkan.Api.Core.VkImageLayout;
using static Exomia.Vulkan.Api.Core.VkComponentSwizzle;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;
using static Exomia.Vulkan.Api.Core.VkSharingMode;
using static Exomia.Vulkan.Api.Core.VkMemoryPropertyFlagBits;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        private static bool RetrieveSwapchainImages(VkContext* context)
        {
            vkGetSwapchainImagesKHR(context->Device, context->Swapchain, &context->SwapchainImageCount, null)
                .AssertVkResult();

            context->SwapchainImages = Allocator.Allocate<VkImage>(context->SwapchainImageCount);
            vkGetSwapchainImagesKHR(context->Device, context->Swapchain, &context->SwapchainImageCount, context->SwapchainImages)
                .AssertVkResult();

            VkImageViewCreateInfo imageViewCreateInfo;
            imageViewCreateInfo.sType                           = VkImageViewCreateInfo.STYPE;
            imageViewCreateInfo.pNext                           = null;
            imageViewCreateInfo.flags                           = 0u;
            imageViewCreateInfo.viewType                        = VK_IMAGE_VIEW_TYPE_2D;
            imageViewCreateInfo.format                          = context->Format;
            imageViewCreateInfo.components.r                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.g                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.b                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.a                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.subresourceRange.aspectMask     = VK_IMAGE_ASPECT_COLOR_BIT;
            imageViewCreateInfo.subresourceRange.baseMipLevel   = 0u;
            imageViewCreateInfo.subresourceRange.levelCount     = 1u;
            imageViewCreateInfo.subresourceRange.baseArrayLayer = 0u;
            imageViewCreateInfo.subresourceRange.layerCount     = 1u;

            context->SwapchainImageViews = Allocator.Allocate<VkImageView>(context->SwapchainImageCount);
            for (uint i = 0u; i < context->SwapchainImageCount; i++)
            {
                imageViewCreateInfo.image = *(context->SwapchainImages + i);

                vkCreateImageView(context->Device, &imageViewCreateInfo, null, context->SwapchainImageViews + i)
                    .AssertVkResult();
            }

            return true;
        }

        private static bool CreateDepthStencilImage(VkContext* context)
        {
            if (context->DepthStencilFormat == VK_FORMAT_UNDEFINED) { return false; }

            VkImageCreateInfo imageCreateInfo;
            imageCreateInfo.sType                 = VkImageCreateInfo.STYPE;
            imageCreateInfo.pNext                 = null;
            imageCreateInfo.flags                 = 0u;
            imageCreateInfo.imageType             = VK_IMAGE_TYPE_2D;
            imageCreateInfo.format                = context->DepthStencilFormat;
            imageCreateInfo.extent.width          = context->Width;
            imageCreateInfo.extent.height         = context->Height;
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

            vkCreateImage(context->Device, &imageCreateInfo, null, &context->DepthStencilImage)
                .AssertVkResult();

            VkImageMemoryRequirementsInfo2 imageMemoryRequirementsInfo2;
            imageMemoryRequirementsInfo2.sType = VkImageMemoryRequirementsInfo2.STYPE;
            imageMemoryRequirementsInfo2.pNext = null;
            imageMemoryRequirementsInfo2.image = context->DepthStencilImage;

            VkMemoryRequirements2 memoryRequirements2;
            memoryRequirements2.sType = VkMemoryRequirements2.STYPE;
            memoryRequirements2.pNext = null;

            vkGetImageMemoryRequirements2(context->Device, &imageMemoryRequirementsInfo2, &memoryRequirements2);

            VkMemoryAllocateInfo memoryAllocateInfo;
            memoryAllocateInfo.sType           = VkMemoryAllocateInfo.STYPE;
            memoryAllocateInfo.pNext           = null;
            memoryAllocateInfo.allocationSize  = memoryRequirements2.memoryRequirements.size;
            memoryAllocateInfo.memoryTypeIndex = FindMemoryTypeIndex(context->PhysicalDevice, memoryRequirements2.memoryRequirements.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

            vkAllocateMemory(context->Device, &memoryAllocateInfo, null, &context->DepthStencilDeviceMemory)
                .AssertVkResult();

            VkBindImageMemoryInfo bindImageMemoryInfo;
            bindImageMemoryInfo.sType        = VkBindImageMemoryInfo.STYPE;
            bindImageMemoryInfo.pNext        = null;
            bindImageMemoryInfo.image        = context->DepthStencilImage;
            bindImageMemoryInfo.memory       = context->DepthStencilDeviceMemory;
            bindImageMemoryInfo.memoryOffset = VkDeviceSize.Zero;

            vkBindImageMemory2(context->Device, 1u, &bindImageMemoryInfo)
                .AssertVkResult();

            VkImageViewCreateInfo imageViewCreateInfo;
            imageViewCreateInfo.sType                           = VkImageViewCreateInfo.STYPE;
            imageViewCreateInfo.pNext                           = null;
            imageViewCreateInfo.flags                           = 0u;
            imageViewCreateInfo.image                           = context->DepthStencilImage;
            imageViewCreateInfo.viewType                        = VK_IMAGE_VIEW_TYPE_2D;
            imageViewCreateInfo.format                          = context->DepthStencilFormat;
            imageViewCreateInfo.components.r                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.g                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.b                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.components.a                    = VK_COMPONENT_SWIZZLE_IDENTITY;
            imageViewCreateInfo.subresourceRange.aspectMask     = VK_IMAGE_ASPECT_DEPTH_BIT | VK_IMAGE_ASPECT_STENCIL_BIT;
            imageViewCreateInfo.subresourceRange.baseMipLevel   = 0u;
            imageViewCreateInfo.subresourceRange.levelCount     = 1u;
            imageViewCreateInfo.subresourceRange.baseArrayLayer = 0u;
            imageViewCreateInfo.subresourceRange.layerCount     = 1u;

            vkCreateImageView(context->Device, &imageViewCreateInfo, null, &context->DepthStencilImageView)
                .AssertVkResult();

            return true;
        }
    }
}
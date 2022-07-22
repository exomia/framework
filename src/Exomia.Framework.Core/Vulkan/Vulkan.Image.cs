#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkFormat;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;

namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Vulkan
{
    /// <summary> Gets suitable image format. </summary>
    /// <param name="context">                       [in,out] If non-null, the context. </param>
    /// <param name="formats">                       The formats. </param>
    /// <param name="physicalDeviceSurfaceInfo2Khr"> [in,out] If non-null, the physical device surface information 2 khr. </param>
    /// <param name="surfaceFormatKhr">              [out] The surface format khr. </param>
    /// <returns> True if it succeeds, false if it fails. </returns>
    public static bool GetSuitableImageFormat(
        VkContext*                       context,
        VkFormat[]                       formats,
        VkPhysicalDeviceSurfaceInfo2KHR* physicalDeviceSurfaceInfo2Khr,
        out VkSurfaceFormatKHR           surfaceFormatKhr)
    {
        uint formatCount = 0u;
        vkGetPhysicalDeviceSurfaceFormats2KHR(context->PhysicalDevice, physicalDeviceSurfaceInfo2Khr, &formatCount, null)
            .AssertVkResult();

        VkSurfaceFormat2KHR* pSurfaceFormat2Khr = stackalloc VkSurfaceFormat2KHR[(int)formatCount];
        for (uint i = 0u; i < formatCount; i++)
        {
            (pSurfaceFormat2Khr + i)->sType = VkSurfaceFormat2KHR.STYPE;
            (pSurfaceFormat2Khr + i)->pNext = null;
        }

        vkGetPhysicalDeviceSurfaceFormats2KHR(context->PhysicalDevice, physicalDeviceSurfaceInfo2Khr, &formatCount, pSurfaceFormat2Khr)
            .AssertVkResult();

        for (int i = 0; i < formats.Length; i++)
        {
            for (uint s = 0u; s < formatCount; s++)
            {
                if (formats[i] == pSurfaceFormat2Khr[s].surfaceFormat.format)
                {
                    surfaceFormatKhr = pSurfaceFormat2Khr[s].surfaceFormat;
                    return true;
                }
            }
        }

        surfaceFormatKhr = default;
        return false;
    }

    /// <summary> Gets image aspect mask. </summary>
    /// <param name="format"> Describes the format to use. </param>
    /// <returns> The image aspect mask. </returns>
    public static VkImageAspectFlags GetImageAspectMask(VkFormat format)
    {
        return format switch
        {
            VK_FORMAT_D16_UNORM_S8_UINT
                or VK_FORMAT_D24_UNORM_S8_UINT
                or VK_FORMAT_D32_SFLOAT_S8_UINT => VK_IMAGE_ASPECT_DEPTH_BIT | VK_IMAGE_ASPECT_STENCIL_BIT,
            VK_FORMAT_D16_UNORM
                or VK_FORMAT_D32_SFLOAT
                or VK_FORMAT_X8_D24_UNORM_PACK32 => VK_IMAGE_ASPECT_DEPTH_BIT,
            VK_FORMAT_UNDEFINED => throw new ArgumentOutOfRangeException(nameof(format), format, "The format is not supported"),
            _                   => VK_IMAGE_ASPECT_COLOR_BIT
        };
    }

    ///// <summary> Transition image layout. </summary>
    ///// <param name="commandBuffer"> Buffer for command data. </param>
    ///// <param name="image">         The image. </param>
    ///// <param name="aspects">       The aspects. </param>
    ///// <param name="oldLayout">     The old layout. </param>
    ///// <param name="newLayout">     The new layout. </param>
    ///// <param name="mipLevels">     (Optional) The mip levels. </param>
    //public static void TransitionImageLayout(
    //    VkCommandBuffer    commandBuffer,
    //    VkImage            image,
    //    VkImageAspectFlags aspects,
    //    VkImageLayout      oldLayout,
    //    VkImageLayout      newLayout,
    //    uint               mipLevels = 0u)
    //{
    //    VkPipelineStageFlags sourceStage      = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;
    //    VkPipelineStageFlags destinationStage = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT;

    //    VkImageMemoryBarrier imageMemoryBarrier;
    //    imageMemoryBarrier.sType                           = VkImageMemoryBarrier.STYPE;
    //    imageMemoryBarrier.pNext                           = null;
    //    imageMemoryBarrier.oldLayout                       = oldLayout;
    //    imageMemoryBarrier.newLayout                       = newLayout;
    //    imageMemoryBarrier.srcQueueFamilyIndex             = VK_QUEUE_FAMILY_IGNORED;
    //    imageMemoryBarrier.dstQueueFamilyIndex             = VK_QUEUE_FAMILY_IGNORED;
    //    imageMemoryBarrier.image                           = image;
    //    imageMemoryBarrier.subresourceRange.aspectMask     = aspects;
    //    imageMemoryBarrier.subresourceRange.baseMipLevel   = 0u;
    //    imageMemoryBarrier.subresourceRange.levelCount     = mipLevels;
    //    imageMemoryBarrier.subresourceRange.baseArrayLayer = 0u;
    //    imageMemoryBarrier.subresourceRange.layerCount     = 1u;

    //    if (oldLayout == VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED && newLayout == VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL)
    //    {
    //        imageMemoryBarrier.srcAccessMask = 0;
    //        imageMemoryBarrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_READ_BIT | VkAccessFlagBits.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT;

    //        destinationStage = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
    //    }
    //    else if (oldLayout == VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED && newLayout == VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL)
    //    {
    //        imageMemoryBarrier.srcAccessMask = 0;
    //        imageMemoryBarrier.dstAccessMask = VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT | VkAccessFlagBits.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;

    //        destinationStage = VkPipelineStageFlagBits.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
    //    }

    //    vkCmdPipelineBarrier(
    //        commandBuffer,
    //        sourceStage,
    //        destinationStage,
    //        0,
    //        0u, null,
    //        0u, null,
    //        1u, &imageMemoryBarrier);
    //}
}
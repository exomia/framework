#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Vulkan;
using static Exomia.Vulkan.Api.Core.VkImageType;
using static Exomia.Vulkan.Api.Core.VkImageTiling;
using static Exomia.Vulkan.Api.Core.VkImageUsageFlagBits;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;
using Buffer = Exomia.Framework.Core.Vulkan.Buffers.Buffer;

namespace Exomia.Framework.Core.Graphics;

/// <summary> A texture. This class cannot be inherited. </summary>
public sealed unsafe class Texture : IDisposable
{
    private readonly VkDevice       _device;
    private readonly VkImage        _image;
    private readonly VkDeviceMemory _imageMemory;
    private readonly VkImageView    _imageView;

    /// <summary> The identifier. </summary>
    public readonly ulong ID;

    /// <summary> The width. </summary>
    public readonly uint Width;

    /// <summary> The height. </summary>
    public readonly uint Height;

    /// <summary> Texture constructor. </summary>
    /// <param name="device"> The device. </param>
    /// <param name="image"> The image. </param>
    /// <param name="imageMemory"> The image memory. </param>
    /// <param name="imageView"> The image view. </param>
    /// <param name="width"> width. </param>
    /// <param name="height"> height. </param>
    public Texture(VkDevice device, VkImage image, VkDeviceMemory imageMemory, VkImageView imageView, uint width, uint height)
    {
        _device      = device;
        _image       = image;
        _imageMemory = imageMemory;
        _imageView   = imageView;
        ID           = (ulong)(void*)_image;
        Width        = width;
        Height       = height;
    }

    /// <summary> Implicit cast that converts the given Texture to a <see cref="VkImage" />. </summary>
    /// <param name="texture"> The texture. </param>
    /// <returns> The result of the operation. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator VkImage(Texture texture)
    {
        return texture._image;
    }

    /// <summary> Implicit cast that converts the given Texture to a <see cref="VkImageView" />. </summary>
    /// <param name="texture"> The texture. </param>
    /// <returns> The result of the operation. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator VkImageView(Texture texture)
    {
        return texture._imageView;
    }


    /// <summary> Creates a new Texture. </summary>
    /// <param name="vkContext"> [in,out] If non-null, context for the vk. </param>
    /// <param name="width"> The width. </param>
    /// <param name="height"> The height. </param>
    /// <param name="data"> The data. </param>
    /// <param name="imageCreateFlagBits"> (Optional) The image create flag bits. </param>
    /// <returns> A Texture. </returns>
    public static Texture Create(
        VkContext*            vkContext,
        uint                  width,
        uint                  height,
        byte[]                data,
        VkImageCreateFlagBits imageCreateFlagBits = 0)
    {
        // TODO: add Vulkan.CreateImage(...) method -> duplicated code here and in swapchain CreateDepthResources
        using (Buffer staging = Buffer.CreateStagingBuffer<byte>(vkContext, data.Length))
        {
            byte* dst = staging.Map<byte>();
            fixed (byte* src = data)
            {
                Unsafe.CopyBlock(dst, src, (uint)data.Length);
            }
            staging.Unmap();

            VkImageCreateInfo imageCreateInfo;
            imageCreateInfo.sType                 = VkImageCreateInfo.STYPE;
            imageCreateInfo.pNext                 = null;
            imageCreateInfo.flags                 = imageCreateFlagBits;
            imageCreateInfo.imageType             = VK_IMAGE_TYPE_2D;
            imageCreateInfo.format                = VK_FORMAT_R8G8B8A8_SRGB;
            imageCreateInfo.extent.width          = width;
            imageCreateInfo.extent.height         = height;
            imageCreateInfo.extent.depth          = 1u;
            imageCreateInfo.mipLevels             = 1u;
            imageCreateInfo.arrayLayers           = 1u;
            imageCreateInfo.samples               = VK_SAMPLE_COUNT_1_BIT;
            imageCreateInfo.tiling                = VK_IMAGE_TILING_OPTIMAL;
            imageCreateInfo.usage                 = VK_IMAGE_USAGE_TRANSFER_DST_BIT | VK_IMAGE_USAGE_SAMPLED_BIT;
            imageCreateInfo.sharingMode           = VK_SHARING_MODE_EXCLUSIVE;
            imageCreateInfo.queueFamilyIndexCount = 0u;
            imageCreateInfo.pQueueFamilyIndices   = null;
            imageCreateInfo.initialLayout         = VK_IMAGE_LAYOUT_UNDEFINED;

            VkImage image;
            vkCreateImage(vkContext->Device, &imageCreateInfo, null, &image)
               .AssertVkResult();

            VkMemoryRequirements memRequirements;
            vkGetImageMemoryRequirements(vkContext->Device, image, &memRequirements);

            VkMemoryAllocateInfo allocInfo;
            allocInfo.sType          = VkMemoryAllocateInfo.STYPE;
            allocInfo.allocationSize = memRequirements.size;
            allocInfo.memoryTypeIndex = Vulkan.Vulkan.FindMemoryTypeIndex(
                vkContext->PhysicalDevice, memRequirements.memoryTypeBits, VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT);

            VkDeviceMemory imageMemory;
            vkAllocateMemory(vkContext->Device, &allocInfo, null, &imageMemory)
               .AssertVkResult();

            vkBindImageMemory(vkContext->Device, image, imageMemory, 0)
               .AssertVkResult();

            VkCommandBuffer commandBuffer = Vulkan.Vulkan.BeginImmediateSubmit(
                vkContext->Device, vkContext->ShortLivedCommandPool);

            Vulkan.Vulkan.TransitionImageLayout(commandBuffer, image,
                VK_IMAGE_ASPECT_COLOR_BIT,
                VK_IMAGE_LAYOUT_UNDEFINED,
                VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                1u);

            VkBufferImageCopy bufferImageCopy;
            bufferImageCopy.bufferOffset                    = 0;
            bufferImageCopy.bufferRowLength                 = 0;
            bufferImageCopy.bufferImageHeight               = 0;
            bufferImageCopy.imageSubresource.aspectMask     = VK_IMAGE_ASPECT_COLOR_BIT;
            bufferImageCopy.imageSubresource.mipLevel       = 0;
            bufferImageCopy.imageSubresource.baseArrayLayer = 0;
            bufferImageCopy.imageSubresource.layerCount     = 1;
            bufferImageCopy.imageOffset.x                   = 0;
            bufferImageCopy.imageOffset.y                   = 0;
            bufferImageCopy.imageOffset.z                   = 0;
            bufferImageCopy.imageExtent.width               = width;
            bufferImageCopy.imageExtent.height              = height;
            bufferImageCopy.imageExtent.depth               = 1;

            // ReSharper disable once AccessToDisposedClosure
            vkCmdCopyBufferToImage(commandBuffer, staging, image, VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, 1u, &bufferImageCopy);

            Vulkan.Vulkan.TransitionImageLayout(commandBuffer, image,
                VK_IMAGE_ASPECT_COLOR_BIT,
                VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL,
                VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
                1u);

            Vulkan.Vulkan.EndImmediateSubmit(
                vkContext->Device, *(vkContext->Queues - 1), vkContext->ShortLivedCommandPool, commandBuffer);

            VkImageView imageView;
            Vulkan.Vulkan.CreateImageView(
                vkContext->Device,
                image,
                &imageView,
                VK_FORMAT_R8G8B8A8_SRGB,
                VK_IMAGE_ASPECT_COLOR_BIT);

            return new Texture(vkContext->Device, image, imageMemory, imageView, width, height);
        }
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary>
    ///     Texture destructor.
    /// </summary>
    ~Texture()
    {
        Dispose();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_disposed)
        {
            vkDestroyImageView(_device, _imageView, null);
            vkDestroyImage(_device, _image, null);
            vkFreeMemory(_device, _imageMemory, null);
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }

    #endregion
}
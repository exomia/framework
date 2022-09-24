#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;

namespace Exomia.Framework.Core.Graphics;

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in Vector2 position, in VkColor color)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, 1f, 1f), true,
            s_nullRectangle, color,                                          0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="color"> The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in RectangleF destinationRectangle, in VkColor color)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            s_nullRectangle, color,                0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in VkColor color)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, 1f, 1f), true,
            sourceRectangle, color,                                          0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in Rectangle? sourceRectangle,
                       in VkColor    color)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            sourceRectangle, color,                0f, s_vector2Zero, 1.0f, TextureEffects.None);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in RectangleF  destinationRectangle,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         destinationRectangle, false,
            sourceRectangle, color,                rotation, origin, opacity, effects);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          scale,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, scale, scale), true,
            sourceRectangle, color,                                                rotation, origin, opacity, effects);
    }

    /// <summary> Renders a texture. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       in Vector2     scale,
                       float          opacity,
                       TextureEffects effects)
    {
        RenderTexture(
            texture,         new RectangleF(position.X, position.Y, scale.X, scale.Y), true,
            sourceRectangle, color,                                                    rotation, origin, opacity, effects);
    }

    private void RenderTexture(Texture        texture,
                               in RectangleF  destination,
                               bool           scaleDestination,
                               in Rectangle?  sourceRectangle,
                               in VkColor     color,
                               float          rotation,
                               in Vector2     origin,
                               float          opacity,
                               TextureEffects effects,
                               float          mode = TEXTURE_MODE)

    {
        if (!_textureInfos.TryGetValue(texture.ID, out TextureInfo textureInfo))
        {
            bool lockTaken = false;
            try
            {
                _textureSpinLock.Enter(ref lockTaken);
                if (!_textureInfos.TryGetValue(texture.ID, out textureInfo))
                {
                    textureInfo = new TextureInfo(texture.ID, texture.Width, texture.Height);

                    VkDescriptorSetLayout* textureLayouts = stackalloc VkDescriptorSetLayout[(int)_swapchainContext->MaxFramesInFlight];
                    for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
                    {
                        *(textureLayouts + i) = _context->TextureDescriptorSetLayout;
                    }

                    VkDescriptorSetAllocateInfo textureDescriptorSetAllocateInfo;
                    textureDescriptorSetAllocateInfo.sType              = VkDescriptorSetAllocateInfo.STYPE;
                    textureDescriptorSetAllocateInfo.pNext              = null;
                    textureDescriptorSetAllocateInfo.descriptorPool     = _context->TextureDescriptorPool;
                    textureDescriptorSetAllocateInfo.descriptorSetCount = _swapchainContext->MaxFramesInFlight;
                    textureDescriptorSetAllocateInfo.pSetLayouts        = textureLayouts;

                    textureInfo.DescriptorSets = Allocator.Allocate<VkDescriptorSet>(_swapchainContext->MaxFramesInFlight);
                    vkAllocateDescriptorSets(_vkContext->Device, &textureDescriptorSetAllocateInfo, textureInfo.DescriptorSets)
                       .AssertVkResult();

                    for (uint i = 0u; i < _swapchainContext->MaxFramesInFlight; i++)
                    {
                        VkDescriptorImageInfo descriptorImageInfo;
                        descriptorImageInfo.imageLayout = VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
                        descriptorImageInfo.imageView   = texture;
                        descriptorImageInfo.sampler     = _context->TextureSampler;

                        VkWriteDescriptorSet writeDescriptorSet;
                        writeDescriptorSet.sType            = VkWriteDescriptorSet.STYPE;
                        writeDescriptorSet.pNext            = null;
                        writeDescriptorSet.dstSet           = *(textureInfo.DescriptorSets + i);
                        writeDescriptorSet.dstBinding       = 0u;
                        writeDescriptorSet.dstArrayElement  = 0u;
                        writeDescriptorSet.descriptorCount  = 1u;
                        writeDescriptorSet.descriptorType   = VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER;
                        writeDescriptorSet.pImageInfo       = &descriptorImageInfo;
                        writeDescriptorSet.pBufferInfo      = null;
                        writeDescriptorSet.pTexelBufferView = null;

                        vkUpdateDescriptorSets(
                            _vkContext->Device,
                            1u, &writeDescriptorSet,
                            0u, null);
                    }

                    _textureInfos.Add(texture.ID, textureInfo);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _textureSpinLock.Exit(false);
                }
            }
        }
    }
}
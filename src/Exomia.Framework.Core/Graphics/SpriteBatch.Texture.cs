#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using static Exomia.Vulkan.Api.Core.VkDescriptorType;

namespace Exomia.Framework.Core.Graphics;

/// <content> A sprite batch. This class cannot be inherited. </content>
public sealed partial class SpriteBatch
{
    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in Vector2 position, in VkColor color)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = new RectangleF(position, 1f, 1f);
        spriteInfo.ScaleDestination = true;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = 0.0f;
        spriteInfo.Origin           = s_vector2Zero;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = 0.0f;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in RectangleF destinationRectangle, in VkColor color)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = 0.0f;
        spriteInfo.Origin           = s_vector2Zero;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = 0.0f;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in VkColor color)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = new RectangleF(position, 1f, 1f);
        spriteInfo.ScaleDestination = true;
        spriteInfo.Source           = sourceRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = 0.0f;
        spriteInfo.Origin           = s_vector2Zero;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = 0.0f;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in Rectangle? sourceRectangle,
                       in VkColor    color)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = sourceRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = 0.0f;
        spriteInfo.Origin           = s_vector2Zero;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = 0.0f;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="layerDepth"> (Optional) The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture    texture,
                       in Vector2 position,
                       in VkColor color,
                       float      rotation,
                       in Vector2 origin,
                       float      layerDepth = 0f)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = new RectangleF(position, 1f, 1f);
        spriteInfo.ScaleDestination = true;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = layerDepth;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture       texture,
                       in RectangleF destinationRectangle,
                       in VkColor    color,
                       float         rotation,
                       in Vector2    origin,
                       float         layerDepth = 0f)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = 1.0f;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = layerDepth;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="effects"> The Effects. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in RectangleF  destinationRectangle,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = sourceRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = opacity;
        spriteInfo.Effects          = effects;
        spriteInfo.Depth            = layerDepth;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="effects"> The Effects. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       float          scale,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = new RectangleF(position, scale, scale);
        spriteInfo.ScaleDestination = true;
        spriteInfo.Source           = sourceRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = opacity;
        spriteInfo.Effects          = effects;
        spriteInfo.Depth            = layerDepth;
        RenderSprite(spriteInfo, texture);
    }

    /// <summary> Renders a texture to the screen. </summary>
    /// <param name="texture"> The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="scale"> The scale. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="effects"> The Effects. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Render(Texture        texture,
                       in Vector2     position,
                       in Rectangle?  sourceRectangle,
                       in VkColor     color,
                       float          rotation,
                       in Vector2     origin,
                       in Vector2     scale,
                       float          opacity,
                       TextureEffects effects,
                       float          layerDepth)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = new RectangleF(position, scale);
        spriteInfo.ScaleDestination = true;
        spriteInfo.Source           = sourceRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = opacity;
        spriteInfo.Effects          = effects;
        spriteInfo.Depth            = layerDepth;
        RenderSprite(spriteInfo, texture);
    }

    private unsafe void RenderSprite(in SpriteInfo spriteInfo, Texture texture)
    {
#if DEBUG
        if (!_isBeginCalled)
        {
            throw new InvalidOperationException("Begin must be called before draw");
        }
#endif

        if (_spriteQueueCount >= _spriteQueueLength)
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (_spriteQueueCount >= _spriteQueueLength)
                {
                    uint size = _spriteQueueCount << 1;
                    Allocator.Resize(ref _textureQueue, _spriteQueueLength,     size);
                    Allocator.Resize(ref _spriteQueue,  ref _spriteQueueLength, size);
                }
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit(false);
                }
            }
        }

        if (!_textureInfos.TryGetValue(texture.ID, out TextureInfo textureInfo))
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
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
                        descriptorImageInfo.imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL;
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
                    _spinLock.Exit(false);
                }
            }
        }

        uint slot = Interlocked.Increment(ref _spriteQueueCount) - 1u;

        *(_spriteQueue  + slot) = spriteInfo;
        *(_textureQueue + slot) = textureInfo;
    }
}
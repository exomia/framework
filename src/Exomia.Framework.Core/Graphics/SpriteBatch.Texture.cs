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

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteBatch
{
    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">  The texture. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color">    The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in VkColor color)
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color">                The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in RectangleF destinationRectangle, in VkColor color)
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in VkColor color)
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The Color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture       texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">    The texture. </param>
    /// <param name="position">   The position. </param>
    /// <param name="color">      The Color. </param>
    /// <param name="rotation">   The Rotation. </param>
    /// <param name="origin">     The Origin. </param>
    /// <param name="layerDepth"> (Optional) The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture    texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color">                The Color. </param>
    /// <param name="rotation">             The Rotation. </param>
    /// <param name="origin">               The Origin. </param>
    /// <param name="layerDepth">           The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture       texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The Color. </param>
    /// <param name="rotation">             The Rotation. </param>
    /// <param name="origin">               The Origin. </param>
    /// <param name="opacity">              The Opacity. </param>
    /// <param name="effects">              The Effects. </param>
    /// <param name="layerDepth">           The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture        texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The Color. </param>
    /// <param name="rotation">        The Rotation. </param>
    /// <param name="origin">          The Origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The Opacity. </param>
    /// <param name="effects">         The Effects. </param>
    /// <param name="layerDepth">      The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture        texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The Color. </param>
    /// <param name="rotation">        The Rotation. </param>
    /// <param name="origin">          The Origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The Opacity. </param>
    /// <param name="effects">         The Effects. </param>
    /// <param name="layerDepth">      The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture        texture,
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
        spriteInfo.TextureInfo      = new TextureInfo(texture.Width, texture.Height);
        DrawSprite(spriteInfo);
    }

    private unsafe void DrawSprite(in SpriteInfo spriteInfo)
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
                    uint size = _spriteQueueLength << 1;
                    Allocator.Resize(ref _spriteQueue,   ref _spriteQueueLength, size);
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

        *(_spriteQueue + (Interlocked.Increment(ref _spriteQueueCount) - 1u)) = spriteInfo;
    }
}
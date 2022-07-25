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
    /// <param name="color">    The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in VkColor color)
    {
        DrawSprite(
            texture, new RectangleF(position, 1f, 1f), true,          s_nullRectangle,
            color,   0f,                               s_vector2Zero, 1.0f, TextureEffects.None, 0f);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="color">                The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in RectangleF destinationRectangle, in VkColor color)
    {
        DrawSprite(
            texture, destinationRectangle, false,         s_nullRectangle,
            color,   0f,                   s_vector2Zero, 1.0f, TextureEffects.None, 0f);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in VkColor color)
    {
        DrawSprite(
            texture, new RectangleF(position, 1f, 1f), true,          sourceRectangle,
            color,   0f,                               s_vector2Zero, 1.0f, TextureEffects.None, 0f);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The color. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture       texture,
                     in RectangleF destinationRectangle,
                     in Rectangle? sourceRectangle,
                     in VkColor    color)
    {
        DrawSprite(
            texture, destinationRectangle, false,         sourceRectangle,
            color,   0f,                   s_vector2Zero, 1.0f, TextureEffects.None, 0f);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">    The texture. </param>
    /// <param name="position">   The position. </param>
    /// <param name="color">      The color. </param>
    /// <param name="rotation">   The rotation. </param>
    /// <param name="origin">     The origin. </param>
    /// <param name="layerDepth"> (Optional) The depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture    texture,
                     in Vector2 position,
                     in VkColor color,
                     float      rotation,
                     in Vector2 origin,
                     float      layerDepth = 0f)
    {
        DrawSprite(
            texture, new RectangleF(position, 1f, 1f), true,   s_nullRectangle,
            color,   rotation,                         origin, 1.0f, TextureEffects.None, layerDepth);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="color">                The color. </param>
    /// <param name="rotation">             The rotation. </param>
    /// <param name="origin">               The origin. </param>
    /// <param name="layerDepth">           The depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Draw(Texture       texture,
                     in RectangleF destinationRectangle,
                     in VkColor    color,
                     float         rotation,
                     in Vector2    origin,
                     float         layerDepth = 0f)
    {
        DrawSprite(
            texture, destinationRectangle, false,  s_nullRectangle,
            color,   rotation,             origin, 1.0f, TextureEffects.None, layerDepth);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">              The texture. </param>
    /// <param name="destinationRectangle"> The destination rectangle. </param>
    /// <param name="sourceRectangle">      The source rectangle. </param>
    /// <param name="color">                The color. </param>
    /// <param name="rotation">             The rotation. </param>
    /// <param name="origin">               The origin. </param>
    /// <param name="opacity">              The opacity. </param>
    /// <param name="effects">              The effects. </param>
    /// <param name="layerDepth">           The depth of the layer. </param>
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
        DrawSprite(
            texture, destinationRectangle, false,  sourceRectangle,
            color,   rotation,             origin, opacity, effects, layerDepth);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    /// <param name="rotation">        The rotation. </param>
    /// <param name="origin">          The origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The opacity. </param>
    /// <param name="effects">         The effects. </param>
    /// <param name="layerDepth">      The depth of the layer. </param>
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
        DrawSprite(
            texture, new RectangleF(position, scale, scale), true,   sourceRectangle,
            color,   rotation,                               origin, opacity, effects, layerDepth);
    }

    /// <summary>
    ///     Draws a texture to the screen.
    /// </summary>
    /// <param name="texture">         The texture. </param>
    /// <param name="position">        The position. </param>
    /// <param name="sourceRectangle"> The source rectangle. </param>
    /// <param name="color">           The color. </param>
    /// <param name="rotation">        The rotation. </param>
    /// <param name="origin">          The origin. </param>
    /// <param name="scale">           The scale. </param>
    /// <param name="opacity">         The opacity. </param>
    /// <param name="effects">         The effects. </param>
    /// <param name="layerDepth">      The depth of the layer. </param>
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
        DrawSprite(
            texture, new RectangleF(position, scale), true,   sourceRectangle,
            color,   rotation,                        origin, opacity, effects, layerDepth);
    }

    private unsafe void DrawSprite(Texture        texture,
                                   in RectangleF  destination,
                                   bool           scaleDestination,
                                   in Rectangle?  sourceRectangle,
                                   in VkColor     color,
                                   float          rotation,
                                   in Vector2     origin,
                                   float          opacity,
                                   TextureEffects effects,
                                   float          depth)
    {
#if DEBUG
        if (!_isBeginCalled)
        {
            throw new InvalidOperationException("Begin must be called before draw");
        }
#endif
        if (texture.TexturePointer == IntPtr.Zero)
        {
            //throw new ArgumentNullException(nameof(texture));
        }

        if (_spriteQueueCount >= _spriteQueueLength)
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (_spriteQueueCount >= _spriteQueueLength)
                {
                    uint size = _spriteQueueLength * 2;
                    Allocator.Resize(ref _spriteTextures, _spriteQueueLength,     size);
                    Allocator.Resize(ref _sortIndices,    _spriteQueueLength,     size);
                    Allocator.Resize(ref _sortedSprites,  _spriteQueueLength,     size);
                    Allocator.Resize(ref _spriteQueue,    ref _spriteQueueLength, size);
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

        if (!_textureInfos.TryGetValue(texture.TexturePointer, out TextureInfo textureInfo))
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                if (!_textureInfos.TryGetValue(texture.TexturePointer, out textureInfo))
                {
                    textureInfo = new TextureInfo( /*texture.TextureView, */texture.Width, texture.Height);
                    _textureInfos.Add(texture.TexturePointer, textureInfo);
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

        uint spriteQueueCount = Interlocked.Increment(ref _spriteQueueCount) - 1u;
        
        SpriteInfo* spriteInfo = _spriteQueue + spriteQueueCount;
        
        if (sourceRectangle.HasValue)
        {
            Rectangle rectangle = sourceRectangle.Value;
            spriteInfo->Sw = rectangle.Right  - (spriteInfo->Sx = rectangle.Top);
            spriteInfo->Sh = rectangle.Bottom - (spriteInfo->Sy = rectangle.Left);
        }
        else
        {
            spriteInfo->Sx = 0;
            spriteInfo->Sy = 0;
            spriteInfo->Sw = texture.Width;
            spriteInfo->Sh = texture.Height;
        }

        spriteInfo->Dx = destination.Left;
        spriteInfo->Dy = destination.Top;

        if (scaleDestination)
        {
            spriteInfo->Dw = destination.Width  * spriteInfo->Sw;
            spriteInfo->Dh = destination.Height * spriteInfo->Sh;
        }
        else
        {
            spriteInfo->Dw = (destination.Right  - destination.Left);
            spriteInfo->Dh = (destination.Bottom - destination.Top);
        }

        if (spriteInfo->Dw < 0.0f)
        {
            spriteInfo->Dx += spriteInfo->Dw;
            spriteInfo->Dw =  -spriteInfo->Dw;
        }

        if (spriteInfo->Dh < 0.0f)
        {
            spriteInfo->Dy += spriteInfo->Dh;
            spriteInfo->Dh =  -spriteInfo->Dh;
        }

        spriteInfo->Origin        = origin;
        spriteInfo->Rotation      = rotation;
        spriteInfo->Depth         = depth;
        spriteInfo->SpriteEffects = effects;
        spriteInfo->Color         = color;
        spriteInfo->Opacity       = opacity;

        *(_spriteTextures + spriteQueueCount) = textureInfo;
    }
}
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

namespace Exomia.Framework.Core.Graphics;

/// <content> A sprite batch. This class cannot be inherited. </content>
public sealed partial class SpriteBatch
{
    /// <summary> Render text. </summary>
    /// <param name="font"> The font. </param>
    /// <param name="text"> The text. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderText(SpriteFont         font,
                           ReadOnlySpan<char> text,
                           in Vector2         position,
                           in VkColor         color,
                           float              rotation   = 0.0f,
                           float              layerDepth = 0.0f)
    {
        font.Render(
            RenderTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, TextureEffects.None, layerDepth);
    }

    /// <summary> Render text. </summary>
    /// <param name="font"> The font. </param>
    /// <param name="text"> The text. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderText(SpriteFont         font,
                           ReadOnlySpan<char> text,
                           in Vector2         position,
                           in VkColor         color,
                           float              rotation,
                           in Vector2         origin,
                           float              opacity,
                           TextureEffects     effects,
                           float              layerDepth = 0.0f)
    {
        font.Render(
            RenderTextInternal, text, position, color, rotation, origin, opacity, effects, layerDepth);
    }

    /// <summary> Render text. </summary>
    /// <param name="font"> The font. </param>
    /// <param name="text"> The text. </param>
    /// <param name="start"> The start. </param>
    /// <param name="end"> The end. </param>
    /// <param name="position"> The position. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderText(SpriteFont         font,
                           ReadOnlySpan<char> text,
                           int                start,
                           int                end,
                           in Vector2         position,
                           in VkColor         color,
                           float              rotation,
                           in Vector2         origin,
                           float              opacity,
                           TextureEffects     effects,
                           float              layerDepth = 0.0f)
    {
        font.Render(
            RenderTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
    }

    /// <summary> Render text. </summary>
    /// <param name="font"> The font. </param>
    /// <param name="text"> The text. </param>
    /// <param name="start"> The start. </param>
    /// <param name="end"> The end. </param>
    /// <param name="position"> The position. </param>
    /// <param name="dimension"> The dimension. </param>
    /// <param name="color"> The color. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="origin"> The origin. </param>
    /// <param name="opacity"> The opacity. </param>
    /// <param name="effects"> The effects. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderText(SpriteFont         font,
                           ReadOnlySpan<char> text,
                           int                start,
                           int                end,
                           in Vector2         position,
                           in Vector2         dimension,
                           in VkColor         color,
                           float              rotation,
                           in Vector2         origin,
                           float              opacity,
                           TextureEffects     effects,
                           float              layerDepth = 0.0f)
    {
        font.Render(
            RenderTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, layerDepth);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void RenderTextInternal(Texture        texture,
                                     in Vector2     position,
                                     in Rectangle   sourceRectangle,
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
}
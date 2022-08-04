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

public sealed partial class SpriteBatch
{
    /// <summary>
    ///     Draw rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawRectangle(in RectangleF destinationRectangle,
                              in VkColor    color,
                              float         lineWidth,
                              float         rotation,
                              float         opacity,
                              float         layerDepth)
    {
        DrawRectangle(destinationRectangle, color, lineWidth, rotation, s_vector2Zero, opacity, layerDepth);
    }

    /// <summary>
    ///     Draw rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    public void DrawRectangle(in RectangleF destinationRectangle,
                              in VkColor    color,
                              float         lineWidth,
                              float         rotation,
                              in Vector2    origin,
                              float         opacity,
                              float         layerDepth)
    {
        Vector2[] vertex;

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (rotation == 0.0f)
        {
            vertex = new[]
            {
                destinationRectangle.TopLeft, destinationRectangle.TopRight,
                destinationRectangle.BottomRight, destinationRectangle.BottomLeft
            };
        }
        else
        {
            vertex = new Vector2[4];

            Vector2 o = origin;

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (destinationRectangle.Width != 0f)
            {
                o.X /= destinationRectangle.Width;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (destinationRectangle.Height != 0f)
            {
                o.Y /= destinationRectangle.Height;
            }

            float cos = MathF.Cos(rotation);
            float sin = MathF.Sin(rotation);
            for (int j = 0; j < VERTICES_PER_SPRITE; j++)
            {
                Vector2 corner = s_cornerOffsets[j];
                float   posX   = (corner.X - o.X) * destinationRectangle.Width;
                float   posY   = (corner.Y - o.Y) * destinationRectangle.Height;

                vertex[j] = new Vector2(
                    (destinationRectangle.X + (posX * cos)) - (posY * sin),
                    destinationRectangle.Y                  + (posX * sin) + (posY * cos));
            }
        }

        DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
    }

    /// <summary>
    ///     Draw fill rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawFillRectangle(in RectangleF destinationRectangle, in VkColor color)
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
        DrawSprite(spriteInfo, _whiteTexture);
    }

    /// <summary>
    ///     Draw fill rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawFillRectangle(in RectangleF destinationRectangle, in VkColor color, float layerDepth)
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
        spriteInfo.Depth            = layerDepth;
        DrawSprite(spriteInfo, _whiteTexture);
    }

    /// <summary>
    ///     Draw fill rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawFillRectangle(in RectangleF destinationRectangle,
                                  in VkColor    color,
                                  float         opacity,
                                  float         layerDepth)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = 0.0f;
        spriteInfo.Origin           = s_vector2Zero;
        spriteInfo.Opacity          = opacity;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = layerDepth;
        DrawSprite(spriteInfo, _whiteTexture);
    }

    /// <summary>
    ///     Draw fill rectangle.
    /// </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> The Depth of the layer. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawFillRectangle(in RectangleF destinationRectangle,
                                  in VkColor    color,
                                  float         rotation,
                                  in Vector2    origin,
                                  float         opacity,
                                  float         layerDepth)
    {
        SpriteInfo spriteInfo;
        spriteInfo.Destination      = destinationRectangle;
        spriteInfo.ScaleDestination = false;
        spriteInfo.Source           = s_nullRectangle;
        spriteInfo.Color            = color;
        spriteInfo.Rotation         = rotation;
        spriteInfo.Origin           = origin;
        spriteInfo.Opacity          = opacity;
        spriteInfo.Effects          = TextureEffects.None;
        spriteInfo.Depth            = layerDepth;
        DrawSprite(spriteInfo, _whiteTexture);
    }
}
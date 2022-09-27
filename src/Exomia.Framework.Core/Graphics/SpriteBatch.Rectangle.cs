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
    /// <summary> Renders a rectangle. </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderRectangle(in RectangleF destinationRectangle,
                                in VkColor    color,
                                float         lineWidth,
                                float         rotation,
                                float         opacity,
                                float         layerDepth = 0.0f)
    {
        RenderRectangle(destinationRectangle, color, lineWidth, rotation, s_vector2Zero, opacity, layerDepth);
    }

    /// <summary> Renders a rectangle. </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="lineWidth"> The width of the line. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    public void RenderRectangle(in RectangleF destinationRectangle,
                                in VkColor    color,
                                float         lineWidth,
                                float         rotation,
                                in Vector2    origin,
                                float         opacity,
                                float         layerDepth = 0.0f)
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

        RenderPolygon(vertex, color, lineWidth, opacity, layerDepth);
    }

    /// <summary> Renders a filled rectangle. </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderFillRectangle(in RectangleF destinationRectangle,
                                    in VkColor    color,
                                    float         opacity    = 1.0f,
                                    float         layerDepth = 0.0f)
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
        RenderSprite(spriteInfo, _whiteTexture);
    }

    /// <summary> Renders a filled rectangle. </summary>
    /// <param name="destinationRectangle"> The Destination rectangle. </param>
    /// <param name="color"> The Color. </param>
    /// <param name="rotation"> The Rotation. </param>
    /// <param name="origin"> The Origin. </param>
    /// <param name="opacity"> The Opacity. </param>
    /// <param name="layerDepth"> (Optional) The layer depth [0.0;1.0]. </param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderFillRectangle(in RectangleF destinationRectangle,
                                    in VkColor    color,
                                    float         rotation,
                                    in Vector2    origin,
                                    float         opacity,
                                    float         layerDepth = 0.0f)
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
        RenderSprite(spriteInfo, _whiteTexture);
    }
}
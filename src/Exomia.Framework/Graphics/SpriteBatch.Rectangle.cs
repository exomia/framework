﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteBatch
    {
        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            The width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
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
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            The width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           The depth of the layer. </param>
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
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
                    destinationRectangle.TopLeft, destinationRectangle.TopRight, destinationRectangle.BottomRight,
                    destinationRectangle.BottomLeft
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

                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                for (int j = 0; j < VERTICES_PER_SPRITE; j++)
                {
                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - o.X) * destinationRectangle.Width;
                    float   posY   = (corner.Y - o.Y) * destinationRectangle.Height;

                    vertex[j] = new Vector2(
                        (destinationRectangle.X + (posX * cos)) - (posY * sin),
                        destinationRectangle.Y + (posX * sin) + (posY * cos));
                }
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="layerDepth">           The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawFillRectangle(in RectangleF destinationRectangle, in Color color, float layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, 0.0f, s_vector2Zero, 1.0f, TextureEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         opacity,
                                      float         layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, 0.0f, s_vector2Zero, opacity, TextureEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="layerDepth">           The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         rotation,
                                      in Vector2    origin,
                                      float         opacity,
                                      float         layerDepth)
        {
            DrawSprite(
                _whiteTexture, destinationRectangle, false, s_nullRectangle,
                color, rotation, origin, opacity, TextureEffects.None, layerDepth);
        }
    }
}
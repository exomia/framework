#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Core.Graphics
{
    public sealed partial class SpriteBatch
    {
        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="point1">     The first point. </param>
        /// <param name="point2">     The second point. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in VkColor   color,
                             float      lineWidth,
                             float      opacity,
                             float      layerDepth)
        {
            DrawLine(point1, point2, color, lineWidth, opacity, 1.0f, layerDepth);
        }

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="line">       The line. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Line2 line,
                             in VkColor color,
                             float    lineWidth,
                             float    opacity,
                             float    layerDepth)
        {
            DrawLine(in line, color, lineWidth, opacity, 1.0f, layerDepth);
        }

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="point1">       The first point. </param>
        /// <param name="point2">       The second point. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> The length factor. </param>
        /// <param name="layerDepth">   The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in VkColor   color,
                             float      lineWidth,
                             float      opacity,
                             float      lengthFactor,
                             float      layerDepth)
        {
            DrawLine(new Line2(in point1, in point2), color, lineWidth, opacity, lengthFactor, layerDepth);
        }

        /// <summary>
        ///     Draw line.
        /// </summary>
        /// <param name="line">         The line. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> The length factor. </param>
        /// <param name="layerDepth">   The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Line2 line,
                             in VkColor color,
                             float    lineWidth,
                             float    opacity,
                             float    lengthFactor,
                             float    layerDepth)
        {
            float dx = line.X2 - line.X1;
            float dy = line.Y2 - line.Y1;
            DrawSprite(
                _whiteTexture, new RectangleF(
                    line.X1, line.Y1, MathF.Sqrt((dx * dx) + (dy * dy)) * lengthFactor, lineWidth), false,
                s_nullRectangle, color, MathF.Atan2(dy, dx),
                s_vector2Zero, opacity, TextureEffects.None, layerDepth);
        }
    }
}
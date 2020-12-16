#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Mathematics;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        /// <summary>
        ///     Draw a line from <paramref name="point1" /> to <paramref name="point2" />.
        /// </summary>
        /// <param name="point1">       The first point. </param>
        /// <param name="point2">       The second point. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in Color   color,
                             float      lineWidth,
                             float      rotation,
                             in Vector2 origin,
                             float      opacity,
                             float      lengthFactor = 1.0f)
        {
            DrawLine(new Line2(in point1, in point2), color, lineWidth, rotation, origin, opacity, lengthFactor);
        }

        /// <summary>
        ///     Draw a line.
        /// </summary>
        /// <param name="line">         The line. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        public void DrawLine(in Line2   line,
                             in Color   color,
                             float      lineWidth,
                             float      rotation,
                             in Vector2 origin,
                             float      opacity,
                             float      lengthFactor = 1.0f)
        {
            Line2 l = rotation == 0.0f ? line : Line2.RotateAround(in line, rotation, origin);

            Vector4 scaledColor;
            scaledColor.X = color.R * opacity;
            scaledColor.Y = color.G * opacity;
            scaledColor.Z = color.B * opacity;
            scaledColor.W = color.A * opacity;

            float dx = l.X2 - l.X1;
            float dy = l.Y2 - l.Y1;

            double  dl = Math.Sqrt((dx * dx) + (dy * dy));
            Vector2 n;
            n.X = (float)((-dy / dl) * lineWidth); 
            n.Y = (float)((dx / dl) * lineWidth);

            VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)Reserve(1);

            // p1
            vertex->XY   = l.XY1;
            vertex->RGBA = scaledColor;
            vertex->M    = COLOR_MODE;
            vertex++;

            // p2
            vertex->XY   = l.XY2;
            vertex->RGBA = scaledColor;
            vertex->M    = COLOR_MODE;
            vertex++;

            // p2'
            vertex->XY    = l.XY2 + n;
            vertex->RGBA = scaledColor;
            vertex->M    = COLOR_MODE;
            vertex++;

            // p1'
            vertex->XY   = l.XY1 + n;
            vertex->RGBA = scaledColor;
            vertex->M    = COLOR_MODE;
        }
    }
}
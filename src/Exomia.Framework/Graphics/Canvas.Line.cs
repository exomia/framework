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
using SharpDX.Direct3D11;

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
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(in Vector2 point1,
                             in Vector2 point2,
                             in Color   color,
                             float      lineWidth,
                             float      opacity,
                             float      lengthFactor = 1.0f)
        {
            DrawLine(new Line2(point1, point2), color, lineWidth, opacity, lengthFactor);
        }

        /// <summary>
        ///     Draw a line.
        /// </summary>
        /// <param name="line">         The line. </param>
        /// <param name="color">        The color. </param>
        /// <param name="lineWidth">    The width of the line. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="lengthFactor"> (Optional) The length factor. </param>
        public void DrawLine(in Line2 line,
                             in Color color,
                             float    lineWidth,
                             float    opacity,
                             float    lengthFactor = 1.0f)
        {
            float dx = line.X2 - line.X1;
            float dy = line.Y2 - line.Y1;

            double dl = Math.Sqrt((dx * dx) + (dy * dy));
            float  nx = (float)((dy / dl) * lineWidth);
            float  ny = (float)((dx / dl) * lineWidth);

            DataBox box = _context.MapSubresource(
                _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)box.DataPointer;

            // p1
            vertex->X = line.X1;
            vertex->Y = line.Y1;
            
            vertex->R = color.R * opacity;
            vertex->G = color.G * opacity;
            vertex->B = color.B * opacity;
            vertex->A = color.A * opacity;

            vertex->M = COLOR_MODE;
            vertex++;

            // p2
            vertex->X = line.X2;
            vertex->Y = line.Y2;

            vertex->R = color.R * opacity;
            vertex->G = color.G * opacity;
            vertex->B = color.B * opacity;
            vertex->A = color.A * opacity;

            vertex->M = COLOR_MODE;
            vertex++;

            // p2'
            vertex->X = line.X2 - nx;
            vertex->Y = line.Y2 + ny;

            vertex->R = color.R * opacity;
            vertex->G = color.G * opacity;
            vertex->B = color.B * opacity;
            vertex->A = color.A * opacity;

            vertex->M = COLOR_MODE;
            vertex++;

            // p1'
            vertex->X = line.X1 - nx;
            vertex->Y = line.Y1 + ny;

            vertex->R = color.R * opacity;
            vertex->G = color.G * opacity;
            vertex->B = color.B * opacity;
            vertex->A = color.A * opacity;

            vertex->M = COLOR_MODE;

            _context.UnmapSubresource(_vertexBuffer, 0);

            PrepareForRendering();
            _context.PixelShader.SetShaderResource(0, _whiteTexture.TextureView);
            _context.DrawIndexed(6, 0, 0);
        }
    }
}
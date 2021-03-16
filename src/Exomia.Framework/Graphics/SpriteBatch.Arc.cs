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
using Exomia.Framework.Mathematics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteBatch
    {
        /// <summary>
        ///     Draws a circle.
        /// </summary>
        /// <param name="center">     The center. </param>
        /// <param name="radius">     The radius. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(in Vector2 center,
                               float      radius,
                               in VkColor color,
                               float      lineWidth,
                               float      opacity,
                               int        segments,
                               float      layerDepth)
        {
            DrawArc(new Arc2(center, radius), color, lineWidth, opacity, segments, layerDepth);
        }

        /// <summary>
        ///     Draws a circle.
        /// </summary>
        /// <param name="circle">     The circle. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(in Circle2 circle,
                               in VkColor   color,
                               float      lineWidth,
                               float      opacity,
                               int        segments,
                               float      layerDepth)
        {
            DrawArc(new Arc2(circle.X, circle.Y, circle.Radius), color, lineWidth, opacity, segments, layerDepth);
        }

        /// <summary>
        ///     Draws a circle.
        /// </summary>
        /// <param name="center">     The center. </param>
        /// <param name="radius">     The radius. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawArc(in Vector2 center,
                            float      radius,
                            float      start,
                            float      end,
                            in VkColor   color,
                            float      lineWidth,
                            float      opacity,
                            int        segments,
                            float      layerDepth)
        {
            DrawArc(new Arc2(center, radius, start, end), in color, lineWidth, opacity, segments, layerDepth);
        }

        /// <summary>
        ///     Draws a circle.
        /// </summary>
        /// <param name="arc">     The arc. </param>
        /// <param name="color">      The color. </param>
        /// <param name="lineWidth">  The width of the line. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="segments">   The segments. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
        public void DrawArc(in Arc2  arc,
                            in VkColor color,
                            float    lineWidth,
                            float    opacity,
                            int      segments,
                            float    layerDepth)
        {
            Vector2[] vertex = new Vector2[segments];

            float increment = (arc.End - arc.Start) / segments;
            float theta     = arc.Start;

            for (int i = 0; i < segments; i++)
            {
                vertex[i].X =  arc.X + (arc.Radius * MathF.Cos(theta));
                vertex[i].Y =  arc.Y + (arc.Radius * MathF.Sin(theta));
                theta       += increment;
            }

            DrawPolygon(vertex, color, lineWidth, opacity, layerDepth);
        }
    }
}
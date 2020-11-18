#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Mathematics;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class Canvas
    {
        /// <summary>
        ///     Draws an arc.
        /// </summary>
        /// <param name="center">    The center. </param>
        /// <param name="radius">    The radius. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="segments">  The segments. </param>
        public void DrawArc(in Vector2 center,
                            float      radius,
                            float      start,
                            float      end,
                            in Color   color,
                            float      lineWidth,
                            float      opacity)
        {
            DrawArc(new Arc2(center, radius, start, end), in color, lineWidth, opacity);
        }

        /// <summary>
        ///     Draws an arc.
        /// </summary>
        /// <param name="arc">       The arc. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawArc(in Arc2  arc,
                            in Color color,
                            float    lineWidth,
                            float    opacity) { }

        /// <summary>
        ///     Draws a filled arc.
        /// </summary>
        /// <param name="center">    The center. </param>
        /// <param name="radius">    The radius. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="color">     The color. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillArc(in Vector2 center,
                                float      radius,
                                float      start,
                                float      end,
                                in Color   color,
                                float      opacity)
        {
            DrawFillArc(new Arc2(center, radius, start, end), in color, opacity);
        }

        /// <summary>
        ///     Draws a filled arc.
        /// </summary>
        /// <param name="arc">       The arc. </param>
        /// <param name="color">     The color. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillArc(in Arc2  arc,
                                in Color color,
                                float    opacity) { }
    }
}
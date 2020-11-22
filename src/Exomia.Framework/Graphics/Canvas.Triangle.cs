﻿#region License

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
        ///     Draws a triangle.
        /// </summary>
        /// <param name="point1">    The first point. </param>
        /// <param name="point2">    The second point. </param>
        /// <param name="point3">    The third point. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <remarks>
        ///     The points 1 to 3 have to match the <see cref="RasterizerStateDescription.IsFrontCounterClockwise" />,
        ///     so for default, with <see cref="RasterizerStates.CullBackDepthClipOff" /> is set,
        ///     the points have to use one of the following winding order:
        ///     p1           p2           p3
        ///     /\           /\           /\
        ///     /  \         /  \         /  \
        ///     /____\       /____\       /____\
        ///     p3     p2    p1     p3    p2     p1.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawTriangle(in Vector2 point1,
                                 in Vector2 point2,
                                 in Vector2 point3,
                                 in Color   color,
                                 float      lineWidth,
                                 float      rotation,
                                 in Vector2 origin,
                                 float      opacity)
        {
            DrawTriangle(new Triangle2(point1, point2, point3), color, lineWidth, rotation, in origin, opacity);
        }

        /// <summary>
        ///     Draws a triangle.
        /// </summary>
        /// <param name="triangle">  The triangle. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
        /// <remarks>
        ///     The points 1 to 3 have to match the <see cref="RasterizerStateDescription.IsFrontCounterClockwise" />,
        ///     so for default, with <see cref="RasterizerStates.CullBackDepthClipOff" /> is set,
        ///     the points have to use one of the following winding order:
        ///     p1           p2           p3
        ///     /\           /\           /\
        ///     /  \         /  \         /  \
        ///     /____\       /____\       /____\
        ///     p3     p2    p1     p3    p2     p1.
        /// </remarks>
        public void DrawTriangle(in Triangle2 triangle,
                                 in Color     color,
                                 float        lineWidth,
                                 float        rotation,
                                 in Vector2   origin,
                                 float        opacity)
        {
            Triangle2 t           = rotation == 0.0 ? triangle : Triangle2.RotateAround(triangle, rotation, origin);
            Color     scaledColor = color * opacity;

            Line2 a              = new Line2(t.X1, t.Y1, t.X2, t.Y2);
            Line2 perpendicularA = a.GetPerpendicular(lineWidth);

            Line2 b              = new Line2(t.X2, t.Y2, t.X3, t.Y3);
            Line2 perpendicularB = b.GetPerpendicular(lineWidth);

            Line2 c              = new Line2(t.X3, t.Y3, t.X1, t.Y1);
            Line2 perpendicularC = c.GetPerpendicular(lineWidth);

            if (!perpendicularA.IntersectWith(perpendicularB, out Vector2 ipAb))
            {
                throw new ArgumentException("The lines a and b are parallel to each other! Check the triangle points!");
            }

            if (!perpendicularB.IntersectWith(perpendicularC, out Vector2 ipBc))
            {
                throw new ArgumentException("The lines b and c are parallel to each other! Check the triangle points!");
            }

            if (!perpendicularC.IntersectWith(perpendicularA, out Vector2 ipCa))
            {
                throw new ArgumentException("The lines c and a are parallel to each other! Check the triangle points!");
            }

            Item* ptr = Reserve(3);
            DrawRect(ptr + 0, a, new Line2(ipCa.X, ipCa.Y, ipAb.X, ipAb.Y), in scaledColor);
            DrawRect(ptr + 1, b, new Line2(ipAb.X, ipAb.Y, ipBc.X, ipBc.Y), in scaledColor);
            DrawRect(ptr + 2, c, new Line2(ipBc.X, ipBc.Y, ipCa.X, ipCa.Y), in scaledColor);
        }

        /// <summary>
        ///     Draws a filled triangle.
        /// </summary>
        /// <param name="point1">   The first point. </param>
        /// <param name="point2">   The second point. </param>
        /// <param name="point3">   The third point. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <remarks>
        ///     The points 1 to 3 have to match the <see cref="RasterizerStateDescription.IsFrontCounterClockwise" />,
        ///     so for default, with <see cref="RasterizerStates.CullBackDepthClipOff" /> is set,
        ///     the points have to use one of the following winding order:
        ///     p1           p2           p3
        ///     /\           /\           /\
        ///     /  \         /  \         /  \
        ///     /____\       /____\       /____\
        ///     p3     p2    p1     p3    p2     p1.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawFillTriangle(in Vector2 point1,
                                     in Vector2 point2,
                                     in Vector2 point3,
                                     in Color   color,
                                     float      rotation,
                                     in Vector2 origin,
                                     float      opacity)
        {
            DrawFillTriangle(new Triangle2(point1, point2, point3), color, rotation, origin, opacity);
        }

        /// <summary>
        ///     Draws a filled triangle.
        /// </summary>
        /// <param name="triangle"> The triangle. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <remarks>
        ///     The points 1 to 3 from the <paramref name="triangle" /> have to match the
        ///     <see cref="RasterizerStateDescription.IsFrontCounterClockwise" />,
        ///     so for default, with <see cref="RasterizerStates.CullBackDepthClipOff" /> is set,
        ///     the points have to use one of the following winding order:
        ///     p1           p2           p3
        ///     /\           /\           /\
        ///     /  \         /  \         /  \
        ///     /____\       /____\       /____\
        ///     p3     p2    p1     p3    p2     p1.
        /// </remarks>
        public void DrawFillTriangle(in Triangle2 triangle,
                                     in Color     color,
                                     float        rotation,
                                     in Vector2   origin,
                                     float        opacity)
        {
            Color scaledColor = color * opacity;

            Item* ptr = Reserve(1);

            if (rotation == 0.0f)
            {
                for (int i = 0; i < 3; i++)
                {
                    VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;

                    fixed (Triangle2* t = &triangle)
                    {
                        float* tf = (float*)t;
                        vertex->X = *(tf + (i << 1));
                        vertex->Y = *(tf + (i << 1) + 1);
                    }

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    vertex->M = COLOR_MODE;
                }
            }
            else
            {
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);

                for (int i = 0; i < 3; i++)
                {
                    VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;

                    fixed (Triangle2* t = &triangle)
                    {
                        float* tf = (float*)t;
                        float  x  = *(tf + (i << 1)) - origin.X;
                        float  y  = *(tf + (i << 1) + 1) - origin.Y;
                        vertex->X = (float)(((cos * x) - (sin * y)) + origin.X);
                        vertex->Y = (float)((sin * x) + (cos * y) + origin.Y);
                    }

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    vertex->M = COLOR_MODE;
                }
            }

            // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly so just use the first vertex as the last vertex too.
            *((VertexPositionColorTextureMode*)ptr + 3) = *(VertexPositionColorTextureMode*)ptr;
        }
    }
}
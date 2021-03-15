#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using Exomia.Framework.Mathematics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        /// <summary>
        ///     Draws a polygon.
        /// </summary>
        /// <param name="vertices">  The vertices. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        public void DrawPolygon(Vector2[]  vertices,
                                in VkColor   color,
                                float      lineWidth,
                                float      rotation,
                                in Vector2 origin,
                                float      opacity)
        {
            if (vertices.Length < 2) { throw new ArgumentOutOfRangeException(nameof(vertices.Length)); }

            Vector4 scaledColor;
            scaledColor.X = color.R * opacity;
            scaledColor.Y = color.G * opacity;
            scaledColor.Z = color.B * opacity;
            scaledColor.W = color.A * opacity;

            Item* ptr = Reserve(vertices.Length);

            if (rotation != 0.0f)
            {
                Vector2[] vs  = new Vector2[vertices.Length];
                double    cos = Math.Cos(rotation);
                double    sin = Math.Sin(rotation);
                for (int i = 0; i < vertices.Length; i++)
                {
                    ref Vector2 v = ref vertices[i];
                    float       x = v.X - origin.X;
                    float       y = v.Y - origin.Y;
                    vs[i] = new Vector2(
                        (float)(((cos * x) - (sin * y)) + origin.X), (float)((sin * x) + (cos * y) + origin.Y));
                }
                vertices = vs;
            }

            Line2 previous = Line2.CreateWithPerpendicular(
                ref vertices[vertices.Length - 1], ref vertices[0], lineWidth, out Line2 perpendicularPrevious);

            Line2 current = Line2.CreateWithPerpendicular(
                ref vertices[0], ref vertices[1], lineWidth, out Line2 perpendicularCurrent);

            if (!perpendicularPrevious.IntersectWith(perpendicularCurrent, out Vector2 ipE))
            {
                ipE = new Vector2(perpendicularCurrent.X1, perpendicularCurrent.Y1);
            }

            Vector2 ip1 = ipE;
            for (int i = 1; i < vertices.Length - 1; i++)
            {
                Line2 next = Line2.CreateWithPerpendicular(
                    ref vertices[i], ref vertices[i + 1], lineWidth, out Line2 perpendicularNext);

                if (!perpendicularCurrent.IntersectWith(perpendicularNext, out Vector2 ip2))
                {
                    ip2 = new Vector2(perpendicularNext.X1, perpendicularNext.Y1);
                }

                DrawRect(ptr + i, current, new Line2(in ip1, in ip2), in scaledColor);

                current              = next;
                perpendicularCurrent = perpendicularNext;
                ip1                  = ip2;
            }

            if (!perpendicularCurrent.IntersectWith(perpendicularPrevious, out Vector2 ip3))
            {
                ip3 = new Vector2(perpendicularPrevious.X1, perpendicularPrevious.Y1);
            }

            DrawRect(ptr, current, new Line2(in ip1, in ip3), in scaledColor);
            DrawRect(ptr + (vertices.Length - 1), previous, new Line2(in ip3, in ipE), in scaledColor);
        }

        /// <summary>
        ///     Draws a filled polygon.
        /// </summary>
        /// <param name="vertices"> The vertices. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <remarks>
        ///     Attention:
        ///     - The <paramref name="vertices" /> must be declared in a clockwise orientation.
        ///     - The triangulation used to fill the polygon may not work for concave polygons at the moment!
        ///     - Complex polygons may not work at all!
        /// </remarks>
        public void DrawFillPolygon(Vector2[]  vertices,
                                    in Color   color,
                                    float      rotation,
                                    in Vector2 origin,
                                    float      opacity)
        {
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException(nameof(vertices.Length)); }

            Vector4 scaledColor;
            scaledColor.X = color.R * opacity;
            scaledColor.Y = color.G * opacity;
            scaledColor.Z = color.B * opacity;
            scaledColor.W = color.A * opacity;

            VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)Reserve(vertices.Length - 2);

            if (rotation == 0.0f)
            {
                for (int i = 1; i < vertices.Length - 1; i += 2)
                {
                    vertex->XY   = vertices[0];
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    vertex->XY   = vertices[i];
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    vertex->XY   = vertices[i + 1];
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    if (i + 2 < vertices.Length)
                    {
                        vertex->XY   = vertices[i + 2];
                        vertex->RGBA = scaledColor;
                        vertex->M    = COLOR_MODE;
                        vertex++;
                    }
                    else
                    {
                        // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly so just use the first vertex as the last vertex too.
                        *vertex = *(vertex - 3);
                    }
                }
            }
            else
            {
                Vector2* vs  = stackalloc Vector2[vertices.Length];
                double   cos = Math.Cos(rotation);
                double   sin = Math.Sin(rotation);
                for (int i = 0; i < vertices.Length; i++)
                {
                    ref Vector2 v = ref vertices[i];
                    float       x = v.X - origin.X;
                    float       y = v.Y - origin.Y;
                    *(vs + i) = new Vector2(
                        (float)(((cos * x) - (sin * y)) + origin.X), (float)((sin * x) + (cos * y) + origin.Y));
                }

                for (int i = 1; i < vertices.Length - 1; i += 2)
                {
                    vertex->XY   = *vs;
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    vertex->XY   = *(vs + i);
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    vertex->XY   = *(vs + i + 1);
                    vertex->RGBA = scaledColor;
                    vertex->M    = COLOR_MODE;
                    vertex++;

                    if (i + 2 < vertices.Length)
                    {
                        vertex->XY   = *(vs + i + 2);
                        vertex->RGBA = scaledColor;
                        vertex->M    = COLOR_MODE;
                        vertex++;
                    }
                    else
                    {
                        // INFO: currently we need 4 vertices (rectangle) and can't draw triangles directly so just use the first vertex as the last vertex too.
                        *vertex = *(vertex - 3);
                    }
                }
            }
        }
    }
}
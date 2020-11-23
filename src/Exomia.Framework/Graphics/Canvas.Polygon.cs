#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using Exomia.Framework.Mathematics;
using SharpDX;

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
                                in Color   color,
                                float      lineWidth,
                                float      rotation,
                                in Vector2 origin,
                                float      opacity)
        {
            if (vertices.Length < 2) { throw new ArgumentOutOfRangeException(nameof(vertices.Length)); }

            Color scaledColor = color * opacity;

            Item* ptr = Reserve(vertices.Length);

            if (rotation != 0.0f)
            {
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);
                for (int i = 0; i < vertices.Length; i++)
                {
                    float x = vertices[i].X - origin.X;
                    float y = vertices[i].Y - origin.Y;
                    vertices[i] = new Vector2(
                        (float)(((cos * x) - (sin * y)) + origin.X),
                        (float)((sin * x) + (cos * y) + origin.Y));
                }
            }

            Line2 previous              = new Line2(vertices[vertices.Length - 1], vertices[0]);
            Line2 perpendicularPrevious = previous.GetPerpendicular(lineWidth);

            Line2 current              = new Line2(vertices[0], vertices[1]);
            Line2 perpendicularCurrent = current.GetPerpendicular(lineWidth);

            if (!perpendicularPrevious.IntersectWith(perpendicularCurrent, out Vector2 ipE))
            {
                ipE = new Vector2(perpendicularCurrent.X1, perpendicularCurrent.Y1);
            }

            Vector2 ip1 = ipE;
            for (int i = 1; i < vertices.Length - 1; i++)
            {
                Line2 next              = new Line2(vertices[i], vertices[i + 1]);
                Line2 perpendicularNext = next.GetPerpendicular(lineWidth);

                if (!perpendicularCurrent.IntersectWith(perpendicularNext, out Vector2 ip2))
                {
                    ip2 = new Vector2(perpendicularNext.X1, perpendicularNext.Y1);
                }

                DrawRect(ptr + i, current, new Line2(ip1.X, ip1.Y, ip2.X, ip2.Y), in scaledColor);

                current              = next;
                perpendicularCurrent = perpendicularNext;
                ip1                  = ip2;
            }

            if (!perpendicularCurrent.IntersectWith(perpendicularPrevious, out Vector2 ip3))
            {
                ip3 = new Vector2(perpendicularPrevious.X1, perpendicularPrevious.Y1);
            }

            DrawRect(ptr, current, new Line2(ip1.X, ip1.Y, ip3.X, ip3.Y), in scaledColor);
            DrawRect(ptr + (vertices.Length - 1), previous, new Line2(ip3.X, ip3.Y, ipE.X, ipE.Y), in scaledColor);
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
        ///     - The <paramref name="vertices"/> must be declared in a clockwise orientation.
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

            Color scaledColor = color * opacity;

            if (rotation != 0.0f)
            {
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);
                for (int i = 0; i < vertices.Length; i++)
                {
                    float x = vertices[i].X - origin.X;
                    float y = vertices[i].Y - origin.Y;
                    vertices[i] = new Vector2(
                        (float)(((cos * x) - (sin * y)) + origin.X),
                        (float)((sin * x) + (cos * y) + origin.Y));
                }
            }

            VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)Reserve(vertices.Length - 2);

            for (int i = 1; i < vertices.Length - 1; i += 2)
            {
                vertex->X = vertices[0].X;
                vertex->Y = vertices[0].Y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;

                vertex->M = COLOR_MODE;
                vertex++;

                vertex->X = vertices[i].X;
                vertex->Y = vertices[i].Y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;

                vertex->M = COLOR_MODE;
                vertex++;

                vertex->X = vertices[i + 1].X;
                vertex->Y = vertices[i + 1].Y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;

                vertex->M = COLOR_MODE;
                vertex++;

                if (i + 2 < vertices.Length)
                {
                    vertex->X = vertices[i + 2].X;
                    vertex->Y = vertices[i + 2].Y;

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    vertex->M = COLOR_MODE;
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
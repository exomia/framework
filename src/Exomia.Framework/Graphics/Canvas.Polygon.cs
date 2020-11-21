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
        /// <param name="vertices">  he vertices. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawPolygon(Vector2[] vertices, in Color color, float lineWidth, float opacity)
        {
            if (vertices.Length < 2) { throw new ArgumentOutOfRangeException(nameof(vertices.Length)); }

            Color scaledColor = color * opacity;

            Item* ptr = Reserve(vertices.Length);

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
        /// <param name="vertices">  The vertices. </param>
        /// <param name="color">     The color. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawFillPolygon(Vector2[] vertices, in Color color, float opacity)
        {
            if (vertices.Length < 3) { throw new ArgumentOutOfRangeException(nameof(vertices.Length)); }

            Color scaledColor = color * opacity;

            // TODO: refactor; remove list; place directly into _vertexBuffer
            List<Vector2> vs = new List<Vector2>();
            for (int i = 1; i < vertices.Length - 1; i += 2)
            {
                vs.Add(vertices[0]);
                vs.Add(vertices[i]);
                vs.Add(vertices[i + 1]);
                if (i + 2 < vertices.Length)
                {
                    vs.Add(vertices[i + 2]);
                }
            }

            Item* ptr = Reserve((vs.Count + 1) / 4);

            for (int j = 0; j < vs.Count; j++)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + j;

                vertex->X = vs[j].X;
                vertex->Y = vs[j].Y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;

                vertex->M = COLOR_MODE;
            }

            if ((vs.Count & 1) == 1)
            {
                *((VertexPositionColorTextureMode*)ptr + vs.Count) =
                    *((VertexPositionColorTextureMode*)ptr + (vs.Count - 3));
            }
        }
    }
}
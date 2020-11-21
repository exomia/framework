#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Mathematics;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        private static readonly Vector2[]
            s_rectangleCornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destination"> The destination rectangle. </param>
        /// <param name="color">       The color. </param>
        /// <param name="lineWidth">   The width of the line. </param>
        /// <param name="rotation">    The rotation. </param>
        /// <param name="origin">      The origin. </param>
        /// <param name="opacity">     The opacity. </param>
        public void DrawRectangle(in RectangleF destination,
                                  in Color      color,
                                  float         lineWidth,
                                  float         rotation,
                                  in Vector2    origin,
                                  float         opacity)
        {
            Color scaledColor = color * opacity;

            Vector2 tl = new Vector2(destination.Left + lineWidth, destination.Top + lineWidth);
            Vector2 tr = new Vector2(destination.Right - lineWidth, destination.Top + lineWidth);
            Vector2 br = new Vector2(destination.Right - lineWidth, destination.Bottom - lineWidth);
            Vector2 bl = new Vector2(destination.Left + lineWidth, destination.Bottom - lineWidth);

            Item* ptr = Reserve(4);

            if (rotation == 0f)
            {
                DrawRect(
                    ptr + 0,
                    new Line2(destination.TopLeft, destination.TopRight),
                    new Line2(tl, tr),
                    in scaledColor);

                DrawRect(
                    ptr + 1,
                    new Line2(destination.TopRight, destination.BottomRight),
                    new Line2(tr, br),
                    in scaledColor);

                DrawRect(
                    ptr + 2,
                    new Line2(destination.BottomRight, destination.BottomLeft),
                    new Line2(br, bl),
                    in scaledColor);

                DrawRect(
                    ptr + 3,
                    new Line2(destination.BottomLeft, destination.TopLeft),
                    new Line2(bl, tl),
                    in scaledColor);
            }
            else
            {
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);

                float tlx1 = destination.Left - origin.X;
                float tly1 = destination.Top - origin.Y;

                float tlx2 = tl.X - origin.X;
                float tly2 = tl.Y - origin.Y;

                float trx1 = destination.Right - origin.X;
                float try1 = destination.Top - origin.Y;

                float trx2 = tr.X - origin.X;
                float try2 = tr.Y - origin.Y;

                float brx1 = destination.Right - origin.X;
                float bry1 = destination.Bottom - origin.Y;

                float brx2 = br.X - origin.X;
                float bry2 = br.Y - origin.Y;

                float blx1 = destination.Left - origin.X;
                float bly1 = destination.Bottom - origin.Y;

                float blx2 = bl.X - origin.X;
                float bly2 = bl.Y - origin.Y;

                Vector2 tl1 = new Vector2(
                    (float)((tlx1 * cos) - (tly1 * sin)) + origin.X, (float)((tlx1 * sin) + (tly1 * cos)) + origin.Y);
                Vector2 tl2 = new Vector2(
                    (float)((tlx2 * cos) - (tly2 * sin)) + origin.X, (float)((tlx2 * sin) + (tly2 * cos)) + origin.Y);

                Vector2 tr1 = new Vector2(
                    (float)((trx1 * cos) - (try1 * sin)) + origin.X, (float)((trx1 * sin) + (try1 * cos)) + origin.Y);
                Vector2 tr2 = new Vector2(
                    (float)((trx2 * cos) - (try2 * sin)) + origin.X, (float)((trx2 * sin) + (try2 * cos)) + origin.Y);

                Vector2 br1 = new Vector2(
                    (float)((brx1 * cos) - (bry1 * sin)) + origin.X, (float)((brx1 * sin) + (bry1 * cos)) + origin.Y);
                Vector2 br2 = new Vector2(
                    (float)((brx2 * cos) - (bry2 * sin)) + origin.X, (float)((brx2 * sin) + (bry2 * cos)) + origin.Y);

                Vector2 bl1 = new Vector2(
                    (float)((blx1 * cos) - (bly1 * sin)) + origin.X, (float)((blx1 * sin) + (bly1 * cos)) + origin.Y);
                Vector2 bl2 = new Vector2(
                    (float)((blx2 * cos) - (bly2 * sin)) + origin.X, (float)((blx2 * sin) + (bly2 * cos)) + origin.Y);

                DrawRect(ptr + 0, new Line2(tl1, tr1), new Line2(tl2, tr2), in scaledColor);
                DrawRect(ptr + 1, new Line2(tr1, br1), new Line2(tr2, br2), in scaledColor);
                DrawRect(ptr + 2, new Line2(br1, bl1), new Line2(br2, bl2), in scaledColor);
                DrawRect(ptr + 3, new Line2(bl1, tl1), new Line2(bl2, tl2), in scaledColor);
            }
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destination"> The destination rectangle. </param>
        /// <param name="color">       The color. </param>
        /// <param name="rotation">    The rotation. </param>
        /// <param name="origin">      The origin. </param>
        /// <param name="opacity">     The opacity. </param>
        public void DrawFillRectangle(in RectangleF destination,
                                      in Color      color,
                                      float         rotation,
                                      in Vector2    origin,
                                      float         opacity)
        {
            Color scaledColor = color * opacity;

            Item* ptr = Reserve(1);

            if (rotation == 0f)
            {
                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + j;

                    Vector2 corner = s_rectangleCornerOffsets[j];

                    vertex->X = destination.X + (corner.X * destination.Width);
                    vertex->Y = destination.Y + (corner.Y * destination.Height);

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

                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + j;

                    Vector2 corner = s_rectangleCornerOffsets[j];
                    float   posX   = (destination.X - origin.X) + (corner.X * destination.Width);
                    float   posY   = (destination.Y - origin.Y) + (corner.Y * destination.Height);

                    vertex->X = (float)((origin.X + (posX * cos)) - (posY * sin));
                    vertex->Y = (float)(origin.Y + (posX * sin) + (posY * cos));

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    vertex->M = COLOR_MODE;
                }
            }
        }

        private static void DrawRect(Item* ptr, in Line2 lineA, in Line2 lineB, in Color c)
        {
            // p1
            ptr->V1.X = lineA.X1;
            ptr->V1.Y = lineA.Y1;

            ptr->V1.R = c.R;
            ptr->V1.G = c.G;
            ptr->V1.B = c.B;
            ptr->V1.A = c.A;

            ptr->V1.M = COLOR_MODE;

            // p2
            ptr->V2.X = lineA.X2;
            ptr->V2.Y = lineA.Y2;

            ptr->V2.R = c.R;
            ptr->V2.G = c.G;
            ptr->V2.B = c.B;
            ptr->V2.A = c.A;

            ptr->V2.M = COLOR_MODE;

            // p2'
            ptr->V3.X = lineB.X2;
            ptr->V3.Y = lineB.Y2;

            ptr->V3.R = c.R;
            ptr->V3.G = c.G;
            ptr->V3.B = c.B;
            ptr->V3.A = c.A;

            ptr->V3.M = COLOR_MODE;

            // p1'
            ptr->V4.X = lineB.X1;
            ptr->V4.Y = lineB.Y1;

            ptr->V4.R = c.R;
            ptr->V4.G = c.G;
            ptr->V4.B = c.B;
            ptr->V4.A = c.A;

            ptr->V4.M = COLOR_MODE;
        }
    }
}
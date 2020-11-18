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
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        private static readonly Vector2[]
            s_rectangleCornerOffsets = { Vector2.Zero, Vector2.UnitX, Vector2.One, Vector2.UnitY };

        /// <summary>
        ///     Draw rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="lineWidth">            The width of the line. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        public void DrawRectangle(in RectangleF destinationRectangle,
                                  in Color      color,
                                  float         lineWidth,
                                  float         rotation,
                                  in Vector2    origin,
                                  float         opacity)
        {
            Color scaledColor = color * opacity;

            DataBox box = _context.MapSubresource(
                _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            VertexPositionColorTextureMode* vpctmPtr = (VertexPositionColorTextureMode*)box.DataPointer;

            Vector2 tl = new Vector2(destinationRectangle.Left + lineWidth, destinationRectangle.Top + lineWidth);
            Vector2 tr = new Vector2(destinationRectangle.Right - lineWidth, destinationRectangle.Top + lineWidth);
            Vector2 br = new Vector2(destinationRectangle.Right - lineWidth, destinationRectangle.Bottom - lineWidth);
            Vector2 bl = new Vector2(destinationRectangle.Left + lineWidth, destinationRectangle.Bottom - lineWidth);

            if (rotation == 0f)
            {
                DrawRect(
                    ref vpctmPtr,
                    new Line2(destinationRectangle.TopLeft, destinationRectangle.TopRight),
                    new Line2(tl, tr),
                    in scaledColor);

                DrawRect(
                    ref vpctmPtr,
                    new Line2(destinationRectangle.TopRight, destinationRectangle.BottomRight),
                    new Line2(tr, br),
                    in scaledColor);

                DrawRect(
                    ref vpctmPtr,
                    new Line2(destinationRectangle.BottomRight, destinationRectangle.BottomLeft),
                    new Line2(br, bl),
                    in scaledColor);

                DrawRect(
                    ref vpctmPtr,
                    new Line2(destinationRectangle.BottomLeft, destinationRectangle.TopLeft),
                    new Line2(bl, tl),
                    in scaledColor);
            }
            else
            {
                double cos = Math.Cos(rotation);
                double sin = Math.Sin(rotation);

                float tlx1 = destinationRectangle.Left - origin.X;
                float tly1 = destinationRectangle.Top - origin.Y;

                float tlx2 = tl.X - origin.X;
                float tly2 = tl.Y - origin.Y;

                float trx1 = destinationRectangle.Right - origin.X;
                float try1 = destinationRectangle.Top - origin.Y;

                float trx2 = tr.X - origin.X;
                float try2 = tr.Y - origin.Y;

                float brx1 = destinationRectangle.Right - origin.X;
                float bry1 = destinationRectangle.Bottom - origin.Y;

                float brx2 = br.X - origin.X;
                float bry2 = br.Y - origin.Y;

                float blx1 = destinationRectangle.Left - origin.X;
                float bly1 = destinationRectangle.Bottom - origin.Y;

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

                DrawRect(ref vpctmPtr, new Line2(tl1, tr1), new Line2(tl2, tr2), in scaledColor);
                DrawRect(ref vpctmPtr, new Line2(tr1, br1), new Line2(tr2, br2), in scaledColor);
                DrawRect(ref vpctmPtr, new Line2(br1, bl1), new Line2(br2, bl2), in scaledColor);
                DrawRect(ref vpctmPtr, new Line2(bl1, tl1), new Line2(bl2, tl2), in scaledColor);
            }

            _context.UnmapSubresource(_vertexBuffer, 0);

            PrepareForRendering();
            _context.PixelShader.SetShaderResource(0, _whiteTexture.TextureView);
            _context.DrawIndexed(24, 0, 0);
        }

        /// <summary>
        ///     Draw fill rectangle.
        /// </summary>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        public void DrawFillRectangle(in RectangleF destinationRectangle,
                                      in Color      color,
                                      float         rotation,
                                      in Vector2    origin,
                                      float         opacity)
        {
            Color scaledColor = color * opacity;

            DataBox box = _context.MapSubresource(
                _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            VertexPositionColorTextureMode* vpctmPtr = (VertexPositionColorTextureMode*)box.DataPointer;

            if (rotation == 0f)
            {
                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = vpctmPtr + j;

                    Vector2 corner = s_rectangleCornerOffsets[j];

                    vertex->X = destinationRectangle.X + (corner.X * destinationRectangle.Width);
                    vertex->Y = destinationRectangle.Y + (corner.Y * destinationRectangle.Height);

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
                    VertexPositionColorTextureMode* vertex = vpctmPtr + j;

                    Vector2 corner = s_rectangleCornerOffsets[j];
                    float   posX   = (destinationRectangle.X - origin.X) + (corner.X * destinationRectangle.Width);
                    float   posY   = (destinationRectangle.Y - origin.Y) + (corner.Y * destinationRectangle.Height);

                    vertex->X = (float)((origin.X + (posX * cos)) - (posY * sin));
                    vertex->Y = (float)(origin.Y + (posX * sin) + (posY * cos));

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    vertex->M = COLOR_MODE;
                }
            }

            _context.UnmapSubresource(_vertexBuffer, 0);

            PrepareForRendering();
            _context.PixelShader.SetShaderResource(0, _whiteTexture.TextureView);
            _context.DrawIndexed(6, 0, 0);
        }

        private static void DrawRect(ref VertexPositionColorTextureMode* v, in Line2 lineA, in Line2 lineB, in Color c)
        {
            // p1
            v->X = lineA.X1;
            v->Y = lineA.Y1;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = COLOR_MODE;
            v++;

            // p2
            v->X = lineA.X2;
            v->Y = lineA.Y2;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = COLOR_MODE;
            v++;

            // p2'
            v->X = lineB.X2;
            v->Y = lineB.Y2;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = COLOR_MODE;
            v++;

            // p1'
            v->X = lineB.X1;
            v->Y = lineB.Y1;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = COLOR_MODE;
            v++;
        }
    }
}
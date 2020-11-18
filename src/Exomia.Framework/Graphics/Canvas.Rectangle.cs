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
                                  float         opacity) { }

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
            DataBox box = _context.MapSubresource(
                _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            VertexPositionColorTextureMode* vpctmPtr = (VertexPositionColorTextureMode*)box.DataPointer;

            if (rotation == 0f)
            {
                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = vpctmPtr + j;

                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - origin.X) * destinationRectangle.Width;
                    float   posY   = (corner.Y - origin.Y) * destinationRectangle.Height;

                    vertex->X = destinationRectangle.X + posX;
                    vertex->Y = destinationRectangle.Y + posY;

                    vertex->R = color.R * opacity;
                    vertex->G = color.G * opacity;
                    vertex->B = color.B * opacity;
                    vertex->A = color.A * opacity;

                    vertex->M = 0.0f;
                }
            }
            else
            {
                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);

                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = vpctmPtr + j;

                    Vector2 corner = s_cornerOffsets[j];
                    float   posX   = (corner.X - origin.X) * destinationRectangle.Width;
                    float   posY   = (corner.Y - origin.Y) * destinationRectangle.Height;

                    vertex->X = (destinationRectangle.X + (posX * cos)) - (posY * sin);
                    vertex->Y = destinationRectangle.Y + (posX * sin) + (posY * cos);

                    vertex->R = color.R * opacity;
                    vertex->G = color.G * opacity;
                    vertex->B = color.B * opacity;
                    vertex->A = color.A * opacity;

                    vertex->M = 0.0f;
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

            v->M = 0.0f;
            v++;

            // p2
            v->X = lineA.X2;
            v->Y = lineA.Y2;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = 0.0f;
            v++;

            // p2'
            v->X = lineB.X2;
            v->Y = lineB.Y2;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = 0.0f;
            v++;

            // p1'
            v->X = lineB.X1;
            v->Y = lineB.Y1;

            v->R = c.R;
            v->G = c.G;
            v->B = c.B;
            v->A = c.A;

            v->M = 0.0f;
            v++;
        }
    }
}
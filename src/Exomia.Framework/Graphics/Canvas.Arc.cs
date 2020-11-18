#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Mathematics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        private static readonly Vector2[]
            s_arcCornerOffsets = { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) };
        
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                                float    opacity)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (arc.Start == arc.End) { return; }

            Color scaledColor = color * opacity;

            DataBox box = _context.MapSubresource(
                _vertexBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            VertexPositionColorTextureMode* vpctmPtr = (VertexPositionColorTextureMode*)box.DataPointer;
            
            for (int i = 0; i < 4; i++)
            {
                VertexPositionColorTextureMode* vertex = vpctmPtr + i;

                Vector2 corner = s_arcCornerOffsets[i];
                
                vertex->X = arc.X + (corner.X * arc.Radius);
                vertex->Y = arc.Y + (corner.Y * arc.Radius);
                vertex->Z = arc.X;
                vertex->W = arc.Y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;
                
                if (arc.End < 0)
                {
                    vertex->U = MathUtil.TwoPi + (arc.Start > arc.End ? arc.End : arc.Start);
                    vertex->V = MathUtil.TwoPi + (arc.Start > arc.End ? arc.Start : arc.End);
                }
                else
                {
                    vertex->U = arc.Start < arc.End ? arc.End: arc.Start;
                    vertex->V = arc.Start < arc.End ? arc.Start : arc.End;
                }

                vertex->M =  FILL_CIRCLE_ARC_MODE; 
                vertex->O = arc.Radius;
            }
            
            _context.UnmapSubresource(_vertexBuffer, 0);

            //TODO: remove
            //Vector4 pos = new Vector4(1600,0, 0, 1);

            //var m = Matrix.Transpose(_projectionMatrix);
            
            //var k = new Vector4(
            //    (pos * m.Row1).ToArray().Sum(),
            //    (pos * m.Row2).ToArray().Sum(),
            //    (pos * m.Row3).ToArray().Sum(),
            //    (pos * m.Row4).ToArray().Sum());
            //Console.WriteLine(k);

            PrepareForRendering();
            _context.PixelShader.SetShaderResource(0, _whiteTexture.TextureView);
            _context.DrawIndexed(6, 0, 0);
        }
    }
}
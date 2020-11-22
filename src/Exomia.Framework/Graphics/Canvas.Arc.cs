#region License

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
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawArc(in Vector2 center,
                            float      radius,
                            float      start,
                            float      end,
                            in Color   color,
                            float      lineWidth,
                            float      rotation,
                            in Vector2 origin,
                            float      opacity)
        {
            DrawArc(new Arc2(center, radius, start, end), in color, lineWidth, rotation, in origin, opacity);
        }

        /// <summary>
        ///     Draws an arc.
        /// </summary>
        /// <param name="arc">       The arc. </param>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> The width of the line. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        public void DrawArc(in Arc2    arc,
                            in Color   color,
                            float      lineWidth,
                            float      rotation,
                            in Vector2 origin,
                            float      opacity)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (arc.Start == arc.End) { return; }

            Color scaledColor = color * opacity;

            float r  = arc.Radius;
            float rh = (arc.Radius - lineWidth) * 0.685f;
            float o  = ((uint)(arc.Radius * 10.0f) << 16) | (uint)((arc.Radius - lineWidth) * 10.0f);

            float u = arc.Start;
            float v = arc.End;

            if (u > MathUtil.TwoPi)
            {
                float times = (float)Math.Floor(u / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }
            else if (u < -MathUtil.TwoPi)
            {
                float times = (float)Math.Floor((u + MathUtil.TwoPi) / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }

            if (v > MathUtil.TwoPi)
            {
                float times = (float)Math.Floor(v / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }
            else if (v < -MathUtil.TwoPi)
            {
                float times = (float)Math.Floor((v + MathUtil.TwoPi) / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }

            if (v < u)
            {
                float t = u;
                u = v;
                v = t;
            }

            if (u < 0 && v < 0)
            {
                u += MathUtil.TwoPi;
                v += MathUtil.TwoPi;
            }

            float x;
            float y;
            if (rotation == 0.0f)
            {
                x = arc.X;
                y = arc.Y;
            }
            else
            {
                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                float dx  = arc.X - origin.X;
                float dy  = arc.Y - origin.Y;
                x = ((cos * dx) - (sin * dy)) + origin.X;
                y = (sin * dx) + (cos * dy) + origin.Y;
            }

            Item* ptr = Reserve(4);
            DrawArcRect(
                ptr + 0,
                new Line2(x - r, y - r, x + r, y - r),
                new Line2(x - rh, y - rh, x + rh, y - rh),
                scaledColor, x, y, u, v, o);

            DrawArcRect(
                ptr + 1,
                new Line2(x + r, y - r, x + r, y + r),
                new Line2(x + rh, y - rh, x + rh, y + rh),
                scaledColor, x, y, u, v, o);

            DrawArcRect(
                ptr + 2,
                new Line2(x + r, y + r, x - r, y + r),
                new Line2(x + rh, y + rh, x - rh, y + rh),
                scaledColor, x, y, u, v, o);

            DrawArcRect(
                ptr + 3,
                new Line2(x - r, arc.Y + r, x - r, y - r),
                new Line2(x - rh, arc.Y + rh, x - rh, y - rh),
                scaledColor, x, y, u, v, o);
        }

        /// <summary>
        ///     Draws a filled arc.
        /// </summary>
        /// <param name="center">   The center. </param>
        /// <param name="radius">   The radius. </param>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawFillArc(in Vector2 center,
                                float      radius,
                                float      start,
                                float      end,
                                in Color   color,
                                float      rotation,
                                in Vector2 origin,
                                float      opacity)
        {
            DrawFillArc(new Arc2(center, radius, start, end), in color, rotation, in origin, opacity);
        }

        /// <summary>
        ///     Draws a filled arc.
        /// </summary>
        /// <param name="arc">      The arc. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        public void DrawFillArc(in Arc2    arc,
                                in Color   color,
                                float      rotation,
                                in Vector2 origin,
                                float      opacity)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (arc.Start == arc.End) { return; }

            Color scaledColor = color * opacity;

            float u = arc.Start;
            float v = arc.End;

            if (u > MathUtil.TwoPi)
            {
                float times = (float)Math.Floor(u / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }
            else if (u < -MathUtil.TwoPi)
            {
                float times = (float)Math.Floor((u + MathUtil.TwoPi) / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }

            if (v > MathUtil.TwoPi)
            {
                float times = (float)Math.Floor(v / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }
            else if (v < -MathUtil.TwoPi)
            {
                float times = (float)Math.Floor((v + MathUtil.TwoPi) / MathUtil.TwoPi);
                u -= times * MathUtil.TwoPi;
                v -= times * MathUtil.TwoPi;
            }

            if (v < u)
            {
                float t = u;
                u = v;
                v = t;
            }

            if (u < 0 && v < 0)
            {
                u += MathUtil.TwoPi;
                v += MathUtil.TwoPi;
            }

            float x;
            float y;
            if (rotation == 0.0f)
            {
                x = arc.X;
                y = arc.Y;
            }
            else
            {
                float cos = (float)Math.Cos(rotation);
                float sin = (float)Math.Sin(rotation);
                float dx  = arc.X - origin.X;
                float dy  = arc.Y - origin.Y;
                x = ((cos * dx) - (sin * dy)) + origin.X;
                y = (sin * dx) + (cos * dy) + origin.Y;
            }

            // ReSharper disable CompareOfFloatsByEqualityOperator
            float m = u == 0.0f && v == MathUtil.TwoPi ? FILL_CIRCLE_MODE : FILL_CIRCLE_ARC_MODE;

            // ReSharper enable CompareOfFloatsByEqualityOperator

            Item* ptr = Reserve(1);
            for (int i = 0; i < 4; i++)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;

                Vector2 corner = s_arcCornerOffsets[i];

                vertex->X = x + (corner.X * arc.Radius);
                vertex->Y = y + (corner.Y * arc.Radius);
                vertex->Z = x;
                vertex->W = y;

                vertex->R = scaledColor.R;
                vertex->G = scaledColor.G;
                vertex->B = scaledColor.B;
                vertex->A = scaledColor.A;

                vertex->U = u;
                vertex->V = v;

                vertex->M = m;
                vertex->O = arc.Radius;
            }
        }

        private static void DrawArcRect(Item*    ptr,
                                        in Line2 lineA,
                                        in Line2 lineB,
                                        in Color c,
                                        float    z,
                                        float    w,
                                        float    u,
                                        float    v,
                                        float    o)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            float m = u == 0.0f && v == MathUtil.TwoPi ? BORDER_CIRCLE_MODE : BORDER_CIRCLE_ARC_MODE;

            // ReSharper enable CompareOfFloatsByEqualityOperator

            for (int i = 0; i < 2; i++)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;
                fixed (Line2* t = &lineA)
                {
                    float* lf = (float*)t;
                    vertex->X = *(lf + (i << 1));
                    vertex->Y = *(lf + (i << 1) + 1);
                }

                vertex->Z = z;
                vertex->W = w;

                vertex->R = c.R;
                vertex->G = c.G;
                vertex->B = c.B;
                vertex->A = c.A;

                vertex->U = u;
                vertex->V = v;

                vertex->M = m;
                vertex->O = o;
            }

            for (int i = 1; i >= 0; i--)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + 2 + (1 - i);
                fixed (Line2* t = &lineB)
                {
                    float* lf = (float*)t;
                    vertex->X = *(lf + (i << 1));
                    vertex->Y = *(lf + (i << 1) + 1);
                }

                vertex->Z = z;
                vertex->W = w;

                vertex->R = c.R;
                vertex->G = c.G;
                vertex->B = c.B;
                vertex->A = c.A;

                vertex->U = u;
                vertex->V = v;

                vertex->M = m;
                vertex->O = o;
            }
        }
    }
}
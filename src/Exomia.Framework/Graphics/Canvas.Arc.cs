#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Mathematics;
using Exomia.Vulkan.Api.Core;

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
                            in VkColor   color,
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
                            in VkColor   color,
                            float      lineWidth,
                            float      rotation,
                            in Vector2 origin,
                            float      opacity)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (arc.Start == arc.End) { return; }

            Vector4 scaledColor;
            scaledColor.X = color.R * opacity;
            scaledColor.Y = color.G * opacity;
            scaledColor.Z = color.B * opacity;
            scaledColor.W = color.A * opacity;

            float r  = arc.Radius;
            float rh = (arc.Radius - lineWidth) * 0.685f;
            float o  = ((uint)(arc.Radius * 10.0f) << 16) | (uint)((arc.Radius - lineWidth) * 10.0f);

            float u = arc.Start;
            float v = arc.End;

            if (u > Math2.TWO_PI)
            {
                float times = MathF.Floor(u / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }
            else if (u < -Math2.TWO_PI)
            {
                float times = MathF.Floor((u + Math2.TWO_PI) / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }

            if (v > Math2.TWO_PI)
            {
                float times = MathF.Floor(v / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }
            else if (v < -Math2.TWO_PI)
            {
                float times = MathF.Floor((v + Math2.TWO_PI) / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }

            if (v < u)
            {
                float t = u;
                u = v;
                v = t;
            }

            if (u < 0 && v < 0)
            {
                u += Math2.TWO_PI;
                v += Math2.TWO_PI;
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
                float cos = MathF.Cos(rotation);
                float sin = MathF.Sin(rotation);
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
                                in VkColor   color,
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
                                in VkColor   color,
                                float      rotation,
                                in Vector2 origin,
                                float      opacity)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (arc.Start == arc.End) { return; }

            Vector4 scaledColor;
            scaledColor.X = color.R * opacity;
            scaledColor.Y = color.G * opacity;
            scaledColor.Z = color.B * opacity;
            scaledColor.W = color.A * opacity;

            float u = arc.Start;
            float v = arc.End;

            if (u > Math2.TWO_PI)
            {
                float times = MathF.Floor(u / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }
            else if (u < -Math2.TWO_PI)
            {
                float times = MathF.Floor((u + Math2.TWO_PI) / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }

            if (v > Math2.TWO_PI)
            {
                float times = MathF.Floor(v / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }
            else if (v < -Math2.TWO_PI)
            {
                float times = MathF.Floor((v + Math2.TWO_PI) / Math2.TWO_PI);
                u -= times * Math2.TWO_PI;
                v -= times * Math2.TWO_PI;
            }

            if (v < u)
            {
                float t = u;
                u = v;
                v = t;
            }

            if (u < 0 && v < 0)
            {
                u += Math2.TWO_PI;
                v += Math2.TWO_PI;
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
                float cos = MathF.Cos(rotation);
                float sin = MathF.Sin(rotation);
                float dx  = arc.X - origin.X;
                float dy  = arc.Y - origin.Y;
                x = ((cos * dx) - (sin * dy)) + origin.X;
                y = (sin * dx) + (cos * dy) + origin.Y;
            }

            // ReSharper disable CompareOfFloatsByEqualityOperator
            float m = u == 0.0f && v == Math2.TWO_PI ? FILL_CIRCLE_MODE : FILL_CIRCLE_ARC_MODE;

            // ReSharper enable CompareOfFloatsByEqualityOperator

            Item* ptr = Reserve(1);
            for (int i = 0; i < 4; i++)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;

                Vector2 corner = s_arcCornerOffsets[i];

                vertex->X    = x + (corner.X * arc.Radius);
                vertex->Y    = y + (corner.Y * arc.Radius);
                vertex->Z    = x;
                vertex->W    = y;
                vertex->RGBA = scaledColor;
                vertex->U    = u;
                vertex->V    = v;
                vertex->M    = m;
                vertex->O    = arc.Radius;
            }
        }

        private static void DrawArcRect(Item*      ptr,
                                        in Line2   lineA,
                                        in Line2   lineB,
                                        in Vector4 c,
                                        float      z,
                                        float      w,
                                        float      u,
                                        float      v,
                                        float      o)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            float m = u == 0.0f && v == Math2.TWO_PI ? BORDER_CIRCLE_MODE : BORDER_CIRCLE_ARC_MODE;

            // ReSharper enable CompareOfFloatsByEqualityOperator

            for (int i = 0; i < 2; i++)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + i;
                fixed (Line2* t = &lineA)
                {
                    Vector2* lf = (Vector2*)t;
                    vertex->XY = *(lf + i);
                }

                vertex->Z    = z;
                vertex->W    = w;
                vertex->RGBA = c;
                vertex->U    = u;
                vertex->V    = v;
                vertex->M    = m;
                vertex->O    = o;
            }

            for (int i = 1; i >= 0; i--)
            {
                VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + 2 + (1 - i);
                fixed (Line2* t = &lineB)
                {
                    Vector2* lf = (Vector2*)t;
                    vertex->XY = *(lf + i);
                }

                vertex->Z    = z;
                vertex->W    = w;
                vertex->RGBA = c;
                vertex->U    = u;
                vertex->V    = v;
                vertex->M    = m;
                vertex->O    = o;
            }
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed unsafe partial class Canvas
    {
        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">  The texture. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in Vector2 position, in Color color)
        {
            DrawTexture(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawTexture(
                texture, destinationRectangle, false,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> The source rectangle. </param>
        /// <param name="color">           The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            DrawTexture(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="sourceRectangle">      The source rectangle. </param>
        /// <param name="color">                The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Rectangle? sourceRectangle,
                         in Color      color)
        {
            DrawTexture(
                texture, destinationRectangle, false,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, TextureEffects.None);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="sourceRectangle">      The source rectangle. </param>
        /// <param name="color">                The color. </param>
        /// <param name="rotation">             The rotation. </param>
        /// <param name="origin">               The origin. </param>
        /// <param name="opacity">              The opacity. </param>
        /// <param name="effects">              The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture        texture,
                         in RectangleF  destinationRectangle,
                         in Rectangle?  sourceRectangle,
                         in Color       color,
                         float          rotation,
                         in Vector2     origin,
                         float          opacity,
                         TextureEffects effects)
        {
            DrawTexture(
                texture, destinationRectangle, false,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> The source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture        texture,
                         in Vector2     position,
                         in Rectangle?  sourceRectangle,
                         in Color       color,
                         float          rotation,
                         in Vector2     origin,
                         float          scale,
                         float          opacity,
                         TextureEffects effects)
        {
            DrawTexture(
                texture, new RectangleF(position.X, position.Y, scale, scale), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draws a texture.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> The source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture        texture,
                         in Vector2     position,
                         in Rectangle?  sourceRectangle,
                         in Color       color,
                         float          rotation,
                         in Vector2     origin,
                         in Vector2     scale,
                         float          opacity,
                         TextureEffects effects)
        {
            DrawTexture(
                texture, new RectangleF(position.X, position.Y, scale.X, scale.Y), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        private void DrawTexture(Texture        texture,
                                 in RectangleF  destination,
                                 bool           scaleDestination,
                                 in Rectangle?  sourceRectangle,
                                 in Color       color,
                                 float          rotation,
                                 in Vector2     origin,
                                 float          opacity,
                                 TextureEffects effects,
                                 float          mode = TEXTURE_MODE)

        {
            long tp = texture.TexturePointer.ToInt64();
            if (!_textures.ContainsKey(tp))
            {
                bool lockTaken = false;
                try
                {
                    _spinLock.Enter(ref lockTaken);
                    if (!_textures.ContainsKey(tp))
                    {
                        _textures.Add(tp, texture);
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        _spinLock.Exit(false);
                    }
                }
            }

            Color scaledColor = color * opacity;

            Rectangle  s = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);
            RectangleF d = destination;
            if (scaleDestination)
            {
                d.Width  *= s.Width;
                d.Height *= s.Width;
            }

            if (d.Width < 0)
            {
                d.X     += d.Width;
                d.Width =  -d.Width;
            }

            if (d.Height < 0)
            {
                d.Y      += d.Height;
                d.Height =  -d.Height;
            }

            float deltaX = 1.0f / texture.Width;
            float deltaY = 1.0f / texture.Height;

            Item* ptr = Reserve(1);
            if (rotation == 0f)
            {
                for (int j = 0; j < 4; j++)
                {
                    VertexPositionColorTextureMode* vertex = (VertexPositionColorTextureMode*)ptr + j;

                    Vector2 corner = s_rectangleCornerOffsets[j];

                    vertex->X  = d.X + ((corner.X - origin.X) * d.Width);
                    vertex->Y  = d.Y + ((corner.Y - origin.Y) * d.Height);
                    vertex->ZW = tp;

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    corner    = s_rectangleCornerOffsets[j ^ (int)effects];
                    vertex->U = (s.X + (corner.X * s.Width)) * deltaX;
                    vertex->V = (s.Y + (corner.Y * s.Height)) * deltaY;

                    vertex->M = mode;
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
                    float   posX   = (corner.X - origin.X) * d.Width;
                    float   posY   = (corner.Y - origin.Y) * d.Height;

                    vertex->X  = (float)((d.X + (posX * cos)) - (posY * sin));
                    vertex->Y  = (float)(d.Y + (posX * sin) + (posY * cos));
                    vertex->ZW = tp;

                    vertex->R = scaledColor.R;
                    vertex->G = scaledColor.G;
                    vertex->B = scaledColor.B;
                    vertex->A = scaledColor.A;

                    corner    = s_rectangleCornerOffsets[j ^ (int)effects];
                    vertex->U = (s.X + (corner.X * s.Width)) * deltaX;
                    vertex->V = (s.Y + (corner.Y * s.Height)) * deltaY;

                    vertex->M = mode;
                }
            }
        }
    }
}
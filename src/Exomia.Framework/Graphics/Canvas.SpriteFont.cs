﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using System.Text;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class Canvas
    {
        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             string        text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             string        text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">      The font. </param>
        /// <param name="text">      The text. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="position">  The position. </param>
        /// <param name="dimension"> The dimension. </param>
        /// <param name="color">     The color. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="effects">   The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             string        text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Size2F     dimension,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color, float rotation)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="start">    The start. </param>
        /// <param name="end">      The end. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="origin">   The origin. </param>
        /// <param name="opacity">  The opacity. </param>
        /// <param name="effects">  The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">      The font. </param>
        /// <param name="text">      The text. </param>
        /// <param name="start">     The start. </param>
        /// <param name="end">       The end. </param>
        /// <param name="position">  The position. </param>
        /// <param name="dimension"> The dimension. </param>
        /// <param name="color">     The color. </param>
        /// <param name="rotation">  The rotation. </param>
        /// <param name="origin">    The origin. </param>
        /// <param name="opacity">   The opacity. </param>
        /// <param name="effects">   The effects. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont    font,
                             StringBuilder text,
                             int           start,
                             int           end,
                             in Vector2    position,
                             in Size2F     dimension,
                             in Color      color,
                             float         rotation,
                             in Vector2    origin,
                             float         opacity,
                             SpriteEffects effects)
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        /// <summary>
        ///     Draw text internal.
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
        /// <param name="layerDepth">      The depth of the layer. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DrawTextInternal(Texture       texture,
                                       in Vector2    position,
                                       in Rectangle? sourceRectangle,
                                       in Color      color,
                                       float         rotation,
                                       in Vector2    origin,
                                       float         scale,
                                       float         opacity,
                                       SpriteEffects effects,
                                       float         layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle, color,
                rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawText(SpriteFont font, StringBuilder text, in Vector2 position, in Color color)
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0f);
        }
    }
}
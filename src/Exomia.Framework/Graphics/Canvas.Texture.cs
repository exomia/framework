#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class Canvas
    {
        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">  The texture. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in Vector2 position, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">              The texture. </param>
        /// <param name="destinationRectangle"> The destination rectangle. </param>
        /// <param name="color">                The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in RectangleF destinationRectangle, in Color color)
        {
            DrawSprite(
                texture, destinationRectangle, false,
                s_nullRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> The source rectangle. </param>
        /// <param name="color">           The color. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(Texture texture, in Vector2 position, in Rectangle? sourceRectangle, in Color color)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, 1f, 1f), true,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }

        /// <summary>
        ///     Draws.
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
            DrawSprite(
                texture, destinationRectangle, false,
                sourceRectangle, color, 0f, s_vector2Zero, 1.0f, SpriteEffects.None);
        }

        /// <summary>
        ///     Draws.
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
        public void Draw(Texture       texture,
                         in RectangleF destinationRectangle,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, destinationRectangle, false,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draws.
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
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         float         scale,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draws.
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
        public void Draw(Texture       texture,
                         in Vector2    position,
                         in Rectangle? sourceRectangle,
                         in Color      color,
                         float         rotation,
                         in Vector2    origin,
                         in Vector2    scale,
                         float         opacity,
                         SpriteEffects effects)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale.X, scale.Y), true,
                sourceRectangle, color, rotation, origin, opacity, effects);
        }

        /// <summary>
        ///     Draw sprite.
        /// </summary>
        /// <param name="texture">          The texture. </param>
        /// <param name="destination">      Destination for the. </param>
        /// <param name="scaleDestination"> True to scale destination. </param>
        /// <param name="sourceRectangle">  The source rectangle. </param>
        /// <param name="color">            The color. </param>
        /// <param name="rotation">         The rotation. </param>
        /// <param name="origin">           The origin. </param>
        /// <param name="opacity">          The opacity. </param>
        /// <param name="effects">          The effects. </param>
        private void DrawSprite(Texture       texture,
                                in RectangleF destination,
                                bool          scaleDestination,
                                in Rectangle? sourceRectangle,
                                in Color      color,
                                float         rotation,
                                in Vector2    origin,
                                float         opacity,
                                SpriteEffects effects) { }
    }
}
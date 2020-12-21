#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using SharpDX;

#if NETSTANDARD2_1
using System;

#endif

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteBatch
    {
        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             in Vector2         position,
                             in Color           color,
                             float              layerDepth = 1.0f)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, string text, in Vector2 position, in Color color, float layerDepth = 1.0f)
#endif
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, TextureEffects.None, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             in Vector2         position,
                             in Color           color,
                             float              rotation,
                             float              layerDepth = 1.0f)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font,
                             string     text,
                             in Vector2 position,
                             in Color   color,
                             float      rotation,
                             float      layerDepth = 1.0f)
#endif
        {
            font.Draw(
                DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, TextureEffects.None, layerDepth);
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
        /// <param name="layerDepth"> The depth of the layer. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             in Vector2         position,
                             in Color           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects,
                             float              layerDepth = 1.0f)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             in Vector2     position,
                             in Color       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects,
                             float          layerDepth = 1.0f)
#endif
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, origin, opacity, effects, layerDepth);
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
        /// <param name="layerDepth"> The depth of the layer. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             int                start,
                             int                end,
                             in Vector2         position,
                             in Color           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects,
                             float              layerDepth = 1.0f)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             int            start,
                             int            end,
                             in Vector2     position,
                             in Color       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects,
                             float          layerDepth = 1.0f)
#endif
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, color, rotation, origin, opacity, effects, layerDepth);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">       The font. </param>
        /// <param name="text">       The text. </param>
        /// <param name="start">      The start. </param>
        /// <param name="end">        The end. </param>
        /// <param name="position">   The position. </param>
        /// <param name="dimension">  The dimension. </param>
        /// <param name="color">      The color. </param>
        /// <param name="rotation">   The rotation. </param>
        /// <param name="origin">     The origin. </param>
        /// <param name="opacity">    The opacity. </param>
        /// <param name="effects">    The effects. </param>
        /// <param name="layerDepth"> The depth of the layer. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             int                start,
                             int                end,
                             in Vector2         position,
                             in Size2F          dimension,
                             in Color           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects,
                             float              layerDepth = 1.0f)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             int            start,
                             int            end,
                             in Vector2     position,
                             in Size2F      dimension,
                             in Color       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects,
                             float          layerDepth = 1.0f)
#endif
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects,
                layerDepth);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DrawTextInternal(Texture        texture,
                                       in Vector2     position,
                                       in Rectangle?  sourceRectangle,
                                       in Color       color,
                                       float          rotation,
                                       in Vector2     origin,
                                       float          scale,
                                       float          opacity,
                                       TextureEffects effects,
                                       float          layerDepth)
        {
            DrawSprite(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle,
                color, rotation, origin, opacity, effects, layerDepth);
        }
    }
}
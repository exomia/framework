#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Vulkan.Api.Core;
using Rectangle = Exomia.Framework.Core.Mathematics.Rectangle;
using RectangleF = Exomia.Framework.Core.Mathematics.RectangleF;

#if NETSTANDARD2_1
using System;

#endif

namespace Exomia.Framework.Core.Graphics
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
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, ReadOnlySpan<char> text, in Vector2 position, in VkColor color)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, string text, in Vector2 position, in VkColor color)
#endif
        {
            font.Draw(DrawTextInternal, text, position, color, 0f, Vector2.Zero, 1.0f, TextureEffects.None, 0f);
        }

        /// <summary>
        ///     Draw text.
        /// </summary>
        /// <param name="font">     The font. </param>
        /// <param name="text">     The text. </param>
        /// <param name="position"> The position. </param>
        /// <param name="color">    The color. </param>
        /// <param name="rotation"> The rotation. </param>
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             in Vector2         position,
                             in VkColor           color,
                             float              rotation)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont font, string text, in Vector2 position, in VkColor color, float rotation)
#endif
        {
            font.Draw(DrawTextInternal, text, position, color, rotation, Vector2.Zero, 1.0f, TextureEffects.None, 0f);
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
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             in Vector2         position,
                             in VkColor           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             in Vector2     position,
                             in VkColor       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects)
#endif
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
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             int                start,
                             int                end,
                             in Vector2         position,
                             in VkColor           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             int            start,
                             int            end,
                             in Vector2     position,
                             in VkColor       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects)
#endif
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
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont         font,
                             ReadOnlySpan<char> text,
                             int                start,
                             int                end,
                             in Vector2         position,
                             in Size2F          dimension,
                             in VkColor           color,
                             float              rotation,
                             in Vector2         origin,
                             float              opacity,
                             TextureEffects     effects)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawText(SpriteFont     font,
                             string         text,
                             int            start,
                             int            end,
                             in Vector2     position,
                             in Size2F      dimension,
                             in VkColor       color,
                             float          rotation,
                             in Vector2     origin,
                             float          opacity,
                             TextureEffects effects)
#endif
        {
            font.Draw(
                DrawTextInternal, text, start, end, position, dimension, color, rotation, origin, opacity, effects, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void DrawTextInternal(Texture        texture,
                                       in Vector2     position,
                                       in Rectangle?  sourceRectangle,
                                       in VkColor       color,
                                       float          rotation,
                                       in Vector2     origin,
                                       float          scale,
                                       float          opacity,
                                       TextureEffects effects,
                                       float          layerDepth)
        {
            DrawTexture(
                texture, new RectangleF(position.X, position.Y, scale, scale), true, sourceRectangle, color,
                rotation, origin, opacity, effects, FONT_TEXTURE_MODE);
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteFont
    {
        /// <summary>
        ///     Returns the size of this text.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns>
        ///     A <see cref="Vector2"/> with x and y set to width and height of <paramref name="text" />
        /// </returns>
#if NETSTANDARD2_1
        public Vector2 MeasureText(ReadOnlySpan<char> text)
#else
        public Vector2 MeasureText(string text)
#endif
        {
            return MeasureText(text, 0, text.Length);
        }

        /// <summary>
        ///     Returns the size of this text from <paramref name="start" /> to <paramref name="end" />.
        /// </summary>
        /// <param name="text">  The text. </param>
        /// <param name="start"> The start. </param>
        /// <param name="end">   The end. </param>
        /// <returns>
        ///     A <see cref="Vector2"/> with x and y set to width and height of <paramref name="text" />
        /// </returns>
#if NETSTANDARD2_1
        public Vector2 MeasureText(ReadOnlySpan<char> text, int start, int end)
#else
        public Vector2 MeasureText(string text, int start, int end)
#endif
        {
            Vector2 size = Vector2.Zero;

            if (start >= end) { return size; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start       = 0; }

            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = start; i < end; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\r':
                        {
                            key |= c;
                            continue;
                        }
                    case '\n':
                        {
                            key =  0;
                            x   =  0;
                            y   += LineSpacing;
                        }
                        break;
                    default:
                        {
                            if (!_glyphs.TryGetValue(c, out Glyph glyph))
                            {
                                if (IgnoreUnknownCharacters)
                                {
                                    continue;
                                }

                                glyph = _defaultGlyph;
                            }
                            key |= c;

                            float dx = glyph.OffsetX;
                            if (Kernings.TryGetValue(key, out Kerning kerning))
                            {
                                dx += kerning.Offset;
                            }

                            float nextX = x + glyph.XAdvance + SpacingX;

                            float h = y + LineSpacing;
                            if (nextX + dx > size.X)
                            {
                                size.X = nextX;
                            }
                            if (h > size.Y)
                            {
                                size.Y = h;
                            }

                            x = nextX;
                        }
                        break;
                }

                key <<= 16;
            }

            return size;
        }

        /// <summary>
        ///     Determines which item (if any) has been hit.
        /// </summary>
        /// <param name="text">  The text. </param>
        /// <param name="start"> The start. </param>
        /// <param name="end">   The end. </param>
        /// <param name="xPos">  The position. </param>
        /// <param name="yPos">  The position. </param>
        /// <returns>
        ///     An int.
        /// </returns>
#if NETSTANDARD2_1
        public int HitTest(ReadOnlySpan<char> text, int start, int end, float xPos, float yPos)
#else
        public int HitTest(string text, int start, int end, float xPos, float yPos)
#endif
        {
            if (start >= end) { return end; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start       = 0; }

            if (xPos < 0) { return -1; }
            if (yPos < 0) { return -1; }

            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = start; i < end; i++)
            {
                char c = text[i];
                switch (c)
                {
                    case '\r':
                        {
                            key |= c;
                            continue;
                        }
                    case '\n':
                        {
                            key =  0;
                            x   =  0;
                            y   += LineSpacing;
                        }
                        break;
                    default:
                        {
                            if (!_glyphs.TryGetValue(c, out Glyph glyph))
                            {
                                if (IgnoreUnknownCharacters)
                                {
                                    continue;
                                }
                                glyph = _defaultGlyph;
                            }
                            key |= c;

                            float dx = glyph.OffsetX;
                            if (Kernings.TryGetValue(key, out Kerning kerning))
                            {
                                dx += kerning.Offset;
                            }

                            float nextX = x + glyph.XAdvance + SpacingX;
                            float h     = y + LineSpacing;

                            if (xPos >= x && xPos <= nextX + dx && yPos <= h && yPos >= y)
                            {
                                if (xPos < (x + nextX + dx) * 0.5f) { return i; }
                                return i + 1;
                            }
                            x = nextX;
                        }
                        break;
                }
                key <<= 16;
            }

            return end;
        }
        
#if NETSTANDARD2_1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Draw(DrawFont           drawCallback,
                           ReadOnlySpan<char> text,
                           in Vector2         position,
                           in Color           color,
                           float              rotation,
                           in Vector2         origin,
                           float              opacity,
                           TextureEffects     effects,
                           float              layerDepth)
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Draw(DrawFont       drawCallback,
                           string         text,
                           in Vector2     position,
                           in VkColor       color,
                           float          rotation,
                           in Vector2     origin,
                           float          opacity,
                           TextureEffects effects,
                           float          layerDepth)
#endif
        {
        Draw(
                drawCallback, text, 0, text.Length,
                in position, in color, rotation, in origin,
                opacity, effects, layerDepth);
        }

#if NETSTANDARD2_1
        internal void Draw(DrawFont           drawCallback,
                           ReadOnlySpan<char> text,
                           int                start,
                           int                end,
                           in Vector2         position,
                           in Color           color,
                           float              rotation,
                           in Vector2         origin,
                           float              opacity,
                           TextureEffects     effects,
                           float              layerDepth)
#else
        internal void Draw(DrawFont       drawCallback,
                           string         text,
                           int            start,
                           int            end,
                           in Vector2     position,
                           in VkColor       color,
                           float          rotation,
                           in Vector2     origin,
                           float          opacity,
                           TextureEffects effects,
                           float          layerDepth)
#endif
        {
            if (end <= start || end > text.Length) { end = text.Length; }

            float x = 0;
            float y = 0;

            int key = 0;

            if (rotation == 0)
            {
                for (int i = start; i < end; i++)
                {
                    char c = text[i];
                    switch (c)
                    {
                        case '\r':
                            {
                                key |= c;
                                continue;
                            }
                        case '\n':
                            {
                                key =  0;
                                x   =  0;
                                y   += LineSpacing;
                            }
                            break;
                        default:
                            {
                                if (!_glyphs.TryGetValue(c, out Glyph glyph))
                                {
                                    if (IgnoreUnknownCharacters)
                                    {
                                        continue;
                                    }

                                    glyph = _defaultGlyph;
                                }
                                key |= c;

                                float dx = glyph.OffsetX;
                                if (Kernings.TryGetValue(key, out Kerning kerning))
                                {
                                    dx += kerning.Offset;
                                }

                                drawCallback(
                                    _texture,
                                    new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY),
                                    glyph.Subrect, color, rotation, origin,
                                    1.0f, opacity, effects, layerDepth);

                                x += glyph.XAdvance + SpacingX;
                            }
                            break;
                    }

                    key <<= 16;
                }
            }
            else
            {
                float cos = MathF.Cos(rotation);
                float sin = MathF.Sin(rotation);

                for (int i = start; i < end; i++)
                {
                    char c = text[i];
                    switch (c)
                    {
                        case '\r':
                            {
                                key |= c;
                                continue;
                            }
                        case '\n':
                            {
                                key =  0;
                                x   =  0;
                                y   += LineSpacing;
                            }
                            break;
                        default:
                            {
                                if (!_glyphs.TryGetValue(c, out Glyph glyph))
                                {
                                    if (IgnoreUnknownCharacters)
                                    {
                                        continue;
                                    }

                                    glyph = _defaultGlyph;
                                }
                                key |= c;

                                float dx = glyph.OffsetX;
                                if (Kernings.TryGetValue(key, out Kerning kerning))
                                {
                                    dx += kerning.Offset;
                                }

                                float ox = (position.X + x + dx) - origin.X;
                                float oy = (position.Y + y + glyph.OffsetY) - origin.Y;
                                drawCallback(
                                    _texture,
                                    new Vector2(
                                        ((cos * ox) - (sin * oy) + origin.X),
                                        (sin * ox) + (cos * oy) + origin.Y),
                                    glyph.Subrect,
                                    color, rotation, Vector2.Zero, 1.0f, opacity, effects, layerDepth);

                                x += glyph.XAdvance + SpacingX;
                            }
                            break;
                    }

                    key <<= 16;
                }
            }
        }

#if NETSTANDARD2_1
        internal void Draw(DrawFont           drawCallback,
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
                           float              layerDepth)
#else
        internal void Draw(DrawFont       drawCallback,
                           string         text,
                           int            start,
                           int            end,
                           in Vector2     position,
                           in Size2F      dimension,
                           in VkColor       color,
                           float          rotation,
                           in Vector2     origin,
                           float          opacity,
                           TextureEffects effects,
                           float          layerDepth)
#endif
        {
            if (end <= start || end > text.Length) { end = text.Length; }

            float x = 0;
            float y = 0;

            int key = 0;
            if (rotation == 0)
            {
                for (int i = start; i < end; i++)
                {
                    char c = text[i];
                    switch (c)
                    {
                        case '\r':
                            {
                                key |= c;
                                continue;
                            }
                        case '\n':
                            {
                                key =  0;
                                x   =  0;
                                y   += LineSpacing;
                            }
                            break;
                        default:
                            {
                                if (!_glyphs.TryGetValue(c, out Glyph glyph))
                                {
                                    if (IgnoreUnknownCharacters)
                                    {
                                        continue;
                                    }

                                    glyph = _defaultGlyph;
                                }
                                key |= c;

                                float dx = glyph.OffsetX;
                                if (Kernings.TryGetValue(key, out Kerning kerning))
                                {
                                    dx += kerning.Offset;
                                }

                                if (x + dx + glyph.Subrect.Width > dimension.Width) { return; }
                                if (y + glyph.OffsetY + glyph.Subrect.Height > dimension.Height) { return; }

                                drawCallback(
                                    _texture,
                                    new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY),
                                    glyph.Subrect, color, rotation, origin,
                                    1.0f, opacity, effects, layerDepth);

                                x += glyph.XAdvance + SpacingX;
                            }
                            break;
                    }

                    key <<= 16;
                }
            }
            else
            {
                float cos = MathF.Cos(rotation);
                float sin = MathF.Sin(rotation);

                for (int i = start; i < end; i++)
                {
                    char c = text[i];
                    switch (c)
                    {
                        case '\r':
                            {
                                key |= c;
                                continue;
                            }
                        case '\n':
                            {
                                key =  0;
                                x   =  0;
                                y   += LineSpacing;
                            }
                            break;
                        default:
                            {
                                if (!_glyphs.TryGetValue(c, out Glyph glyph))
                                {
                                    if (IgnoreUnknownCharacters)
                                    {
                                        continue;
                                    }

                                    glyph = _defaultGlyph;
                                }
                                key |= c;

                                float dx = glyph.OffsetX;
                                if (Kernings.TryGetValue(key, out Kerning kerning))
                                {
                                    dx += kerning.Offset;
                                }

                                if (x + dx + glyph.Subrect.Width > dimension.Width) { return; }
                                if (y + glyph.OffsetY + glyph.Subrect.Height > dimension.Height) { return; }

                                float ox = (position.X + x + dx) - origin.X;
                                float oy = (position.Y + y + glyph.OffsetY) - origin.Y;
                                drawCallback(
                                    _texture,
                                    new Vector2(
                                        ((cos * ox) - (sin * oy) + origin.X),
                                        (sin * ox) + (cos * oy) + origin.Y),
                                    glyph.Subrect, color, rotation, Vector2.Zero,
                                    1.0f, opacity, effects, layerDepth);

                                x += glyph.XAdvance + SpacingX;
                            }
                            break;
                    }

                    key <<= 16;
                }
            }
        }
    }
}
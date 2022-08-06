#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteFont
{
    /// <summary> Returns the size of this text. </summary>
    /// <param name="text"> The text. </param>
    /// <returns> A <see cref="Vector2" /> with x and y set to width and height of <paramref name="text" /> </returns>
    public Vector2 MeasureText(ReadOnlySpan<char> text)
    {
        return MeasureText(text, 0, text.Length);
    }

    /// <summary> Returns the size of this text from <paramref name="start" /> to <paramref name="end" />. </summary>
    /// <param name="text"> The text. </param>
    /// <param name="start"> The start. </param>
    /// <param name="end"> The end. </param>
    /// <returns> A <see cref="Vector2" /> with x and y set to width and height of <paramref name="text" /> </returns>
    public Vector2 MeasureText(ReadOnlySpan<char> text, int start, int end)
    {
        Vector2 size = Vector2.Zero;

        if (start >= end) { return size; }
        if (end   > text.Length) { end = text.Length; }
        if (start < 0) { start         = 0; }

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
                    y   += _lineHeight;
                    break;
                }
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
                    if (Kernings.TryGetValue(key, out int offset))
                    {
                        dx += offset;
                    }

                    float nextX = x + glyph.AdvanceX;

                    float h = y + _lineHeight;
                    if (nextX + dx > size.X)
                    {
                        size.X = nextX;
                    }
                    if (h > size.Y)
                    {
                        size.Y = h;
                    }

                    x = nextX;
                    break;
                }
            }

            key <<= 16;
        }

        return size;
    }
}
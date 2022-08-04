#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteFont
{
    /// <summary>Determines which item (if any) has been hit.</summary>
    /// <param name="text"> The text. </param>
    /// <param name="start"> The start. </param>
    /// <param name="end"> The end. </param>
    /// <param name="xPos"> The position. </param>
    /// <param name="yPos"> The position. </param>
    /// <returns>An int. </returns>
    public int HitTest(ReadOnlySpan<char> text, int start, int end, float xPos, float yPos)
    {
        if (start >= end) { return end; }
        if (end   > text.Length) { end = text.Length; }
        if (start < 0) { start         = 0; }

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

                    float nextX = x + glyph.XAdvance;
                    float h     = y + _lineHeight;

                    if (xPos >= x && xPos <= nextX + dx && yPos <= h && yPos >= y)
                    {
                        if (xPos < (x + nextX + dx) * 0.5f) { return i; }
                        return i + 1;
                    }
                    x = nextX;
                    break;
                }
            }
            key <<= 16;
        }

        return end;
    }
}
#region License
// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.
#endregion

namespace Exomia.Framework.Core.Graphics;

/// <content>
/// The msdf font class
/// </content>
public sealed partial class MsdfFont
{
    /// <summary> Determines which item (if any) has been hit. </summary>
    /// <param name="text"> The text. </param>
    /// <param name="fontSize"> The size of the font. </param>
    /// <param name="start"> The start. </param>
    /// <param name="end"> The end. </param>
    /// <param name="xPos"> The position. </param>
    /// <param name="yPos"> The position. </param>
    /// <returns> An int. </returns>
    public int HitTest(ReadOnlySpan<char> text, float fontSize, int start, int end, float xPos, float yPos)
    {
        if (start >= end) { return end; }
        if (end   > text.Length) { end = text.Length; }
        if (start < 0) { start         = 0; }

        if (xPos < 0) { return -1; }
        if (yPos < 0) { return -1; }

        double x = 0.0;
        double y = 0.0;

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
                    y   += fontSize;
                    break;
                }
                default:
                {
                    if (!_glyphs.TryGetValue(c, out Glyph? glyph))
                    {
                        if (IgnoreUnknownCharacters)
                        {
                            continue;
                        }
                        glyph = _defaultGlyph;
                    }
                    key |= c;

                    double dx = glyph.Advance;
                    if (Kernings.TryGetValue(key, out double offset))
                    {
                        dx += offset;
                    }

                    double nextX = x + glyph.Advance;
                    double h     = y + fontSize;

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

        return -1;
    }
}

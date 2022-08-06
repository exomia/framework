#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteFont
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Render(RenderFont         renderCallback,
                         ReadOnlySpan<char> text,
                         in Vector2         position,
                         in VkColor         color,
                         float              rotation,
                         in Vector2         origin,
                         float              opacity,
                         TextureEffects     effects,
                         float              layerDepth)
    {
        Render(
            renderCallback, text, 0, text.Length, in position, in color, rotation, in origin, opacity, effects, layerDepth);
    }

    internal void Render(RenderFont         renderCallback,
                         ReadOnlySpan<char> text,
                         int                start,
                         int                end,
                         in Vector2         position,
                         in VkColor         color,
                         float              rotation,
                         in Vector2         origin,
                         float              opacity,
                         TextureEffects     effects,
                         float              layerDepth)
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

                        renderCallback(
                            _texture,
                            new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY),
                            new Rectangle(glyph.X, glyph.Y, glyph.Width, glyph.Height),
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.AdvanceX;
                        break;
                    }
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

                        float ox = (position.X + x + dx)            - origin.X;
                        float oy = (position.Y + y + glyph.OffsetY) - origin.Y;
                        renderCallback(
                            _texture,
                            new Vector2(
                                ((cos * ox) - (sin * oy) + origin.X),
                                (sin * ox) + (cos * oy) + origin.Y),
                            new Rectangle(glyph.X, glyph.Y, glyph.Width, glyph.Height),
                            color, rotation, Vector2.Zero, 1.0f, opacity, effects, layerDepth);

                        x += glyph.AdvanceX;
                        break;
                    }
                }

                key <<= 16;
            }
        }
    }

    internal void Render(RenderFont         renderCallback,
                         ReadOnlySpan<char> text,
                         int                start,
                         int                end,
                         in Vector2         position,
                         in Vector2         dimension,
                         in VkColor         color,
                         float              rotation,
                         in Vector2         origin,
                         float              opacity,
                         TextureEffects     effects,
                         float              layerDepth)
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

                        if (x + dx            + glyph.Width  > dimension.X) { return; }
                        if (y + glyph.OffsetY + glyph.Height > dimension.Y) { return; }

                        renderCallback(
                            _texture,
                            new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY),
                            new Rectangle(glyph.X, glyph.Y, glyph.Width, glyph.Height),
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.AdvanceX;
                        break;
                    }
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

                        if (x + dx            + glyph.Width  > dimension.X) { return; }
                        if (y + glyph.OffsetY + glyph.Height > dimension.Y) { return; }

                        float ox = (position.X + x + dx)            - origin.X;
                        float oy = (position.Y + y + glyph.OffsetY) - origin.Y;
                        renderCallback(
                            _texture,
                            new Vector2(
                                ((cos * ox) - (sin * oy) + origin.X),
                                (sin * ox) + (cos * oy) + origin.Y),
                            new Rectangle(glyph.X, glyph.Y, glyph.Width, glyph.Height),
                            color, rotation, Vector2.Zero, 1.0f, opacity, effects, layerDepth);

                        x += glyph.AdvanceX;
                        break;
                    }
                }

                key <<= 16;
            }
        }
    }
}
#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    [ContentReadable(typeof(SpriteFontContentReader))]
    [ContentSerializable(typeof(SpriteFontCR), typeof(SpriteFontCW))]
    public sealed class SpriteFont : IDisposable
    {
        private Glyph _defaultGlyph;

        private Dictionary<int, Glyph> _glyphs;

        private Texture _texture;

        public bool IgnoreUnknownCharacters { get; set; }

        public string Face { get; set; }

        public int Size { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public byte[] ImageData { get; set; }

        public int LineSpacing { get; set; }

        public Glyph DefaultGlyph
        {
            get { return _defaultGlyph; }
            set { _defaultGlyph = value; }
        }

        public int DefaultCharacter { get; set; } = -1;

        public int SpacingX { get; set; }

        public int SpacingY { get; set; }

        public Texture Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Dictionary<int, Glyph> Glyphs
        {
            get { return _glyphs; }
            set
            {
                _glyphs = value;
                if (!_glyphs.TryGetValue(DefaultCharacter, out _defaultGlyph))
                {
                    if (!_glyphs.TryGetValue('?', out _defaultGlyph))
                    {
                        throw new Exception("no default glyph specified!");
                    }
                }
            }
        }

        public Dictionary<int, Kerning> Kernings { get; set; }

        public SpriteFont()
        {
            _glyphs = new Dictionary<int, Glyph>();
            Kernings = new Dictionary<int, Kerning>();
        }

        ~SpriteFont()
        {
            Dispose(false);
        }

        [ContentSerializable(typeof(SpriteFontGlyphCR), typeof(SpriteFontGlyphCW))]
        public struct Glyph
        {
            public int Character;
            public Rectangle Subrect;
            public int OffsetX;
            public int OffsetY;
            public int XAdvance;
        }

        [ContentSerializable(typeof(SpriteFontKerningCR), typeof(SpriteFontKerningCW))]
        public struct Kerning
        {
            public int First;
            public int Second;
            public int Offset;
        }

        #region String

        public Vector2 MeasureText(string text)
        {
            return MeasureText(text, 0, text.Length);
        }

        public Vector2 MeasureText(string text, int start, int end)
        {
            Vector2 size = Vector2.Zero;

            if (start >= end) { return size; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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
                        if (nextX > size.X)
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

        public int HitTest(string text, int start, int end, float xPos, float yPos)
        {
            if (start >= end) { return end; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        if (xPos >= x && xPos <= nextX && yPos <= h && yPos >= y)
                        {
                            if (xPos < (x + nextX) * 0.5f) { return i; }
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

        internal void Draw(SpriteBatch spriteBatch, string text, in Vector2 position, in Color color, float rotation,
            in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = 0; i < text.Length; i++)
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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch spriteBatch, string text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch spriteBatch, string text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        #endregion

        #region StringBuilder

        public Vector2 MeasureText(StringBuilder text)
        {
            return MeasureText(text, 0, text.Length);
        }

        public Vector2 MeasureText(StringBuilder text, int start, int end)
        {
            Vector2 size = Vector2.Zero;

            if (start >= end) { return size; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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
                        if (nextX > size.X)
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

        public int HitTest(StringBuilder text, int start, int end, float xPos, float yPos)
        {
            if (start >= end) { return end; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        if (xPos >= x && xPos <= nextX && yPos <= h && yPos >= y)
                        {
                            if (xPos < (x + nextX) * 0.5f) { return i; }
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

        internal void Draw(SpriteBatch spriteBatch, StringBuilder text, in Vector2 position, in Color color,
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = 0; i < text.Length; i++)
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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch spriteBatch, StringBuilder text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch spriteBatch, StringBuilder text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
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

                        spriteBatch.DrawFont(
                            _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _texture);
                    _glyphs.Clear();
                    Kernings.Clear();
                }

                Kernings = null;
                _glyphs = null;

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    [ContentReadable(typeof(SpriteFont2ContentReader))]
    [ContentSerializable(typeof(SpriteFontCR), typeof(SpriteFontCW))]
    public sealed class SpriteFont2 : IDisposable
    {
        private SpriteFont.Glyph _defaultGlyph;

        private Dictionary<int, SpriteFont.Glyph> _glyphs;

        private Texture2 _texture2;

        public bool IgnoreUnknownCharacters { get; set; }

        public string Face { get; set; }

        public int Size { get; set; }

        public bool Bold { get; set; }

        public bool Italic { get; set; }

        public byte[] ImageData { get; set; }

        public int LineSpacing { get; set; }

        public SpriteFont.Glyph DefaultGlyph
        {
            get { return _defaultGlyph; }
            set { _defaultGlyph = value; }
        }

        public int DefaultCharacter { get; set; } = -1;

        public int SpacingX { get; set; } = 0;

        public int SpacingY { get; set; } = 0;

        public Texture2 Texture2
        {
            get { return _texture2; }
            set { _texture2 = value; }
        }

        public Dictionary<int, SpriteFont.Glyph> Glyphs
        {
            get { return _glyphs; }
            set
            {
                _glyphs = value;
                if (!_glyphs.TryGetValue(DefaultCharacter, out _defaultGlyph))
                {
                    if (!_glyphs.TryGetValue('?', out _defaultGlyph))
                    {
                        throw new Exception("no default glyph specified!");
                    }
                }
            }
        }

        public Dictionary<int, SpriteFont.Kerning> Kernings { get; set; }

        public SpriteFont2()
        {
            _glyphs = new Dictionary<int, SpriteFont.Glyph>();
            Kernings = new Dictionary<int, SpriteFont.Kerning>();
        }

        ~SpriteFont2()
        {
            Dispose(false);
        }

        #region String

        public Vector2 MeasureText(string text)
        {
            return MeasureText(text, 0, text.Length);
        }

        public Vector2 MeasureText(string text, int start, int end)
        {
            Vector2 size = Vector2.Zero;

            if (start >= end) { return size; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        float nextX = x + glyph.XAdvance + SpacingX;

                        float h = y + LineSpacing;
                        if (nextX > size.X)
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

        public int HitTest(string text, int start, int end, float xPos, float yPos)
        {
            if (start >= end) { return end; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }
                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        float nextX = x + glyph.XAdvance + SpacingX;
                        float h = y + LineSpacing;

                        if (xPos >= x && xPos <= nextX && yPos <= h && yPos >= y)
                        {
                            if (xPos < (x + nextX) * 0.5) { return i; }
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

        internal void Draw(SpriteBatch2 spriteBatch2, string text, Vector2 position, Color color, float rotation,
            Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = 0; i < text.Length; i++)
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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch2 spriteBatch2, string text, int start, int end, Vector2 position, Color color,
            float rotation, Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch2 spriteBatch2, string text, int start, int end, Vector2 position,
            Size2F dimension, Color color, float rotation, Vector2 origin, float opacity, SpriteEffects effects,
            float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        if (x + dx + glyph.Subrect.Width > dimension.Width) { return; }
                        if (y + glyph.OffsetY + glyph.Subrect.Height > dimension.Height) { return; }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        #endregion

        #region StringBuilder

        public Vector2 MeasureText(StringBuilder text)
        {
            return MeasureText(text, 0, text.Length);
        }

        public Vector2 MeasureText(StringBuilder text, int start, int end)
        {
            Vector2 size = Vector2.Zero;

            if (start >= end) { return size; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        float nextX = x + glyph.XAdvance + SpacingX;

                        float h = y + LineSpacing;
                        if (nextX > size.X)
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

        public int HitTest(StringBuilder text, int start, int end, float xPos, float yPos)
        {
            if (start >= end) { return end; }
            if (end > text.Length) { end = text.Length; }
            if (start < 0) { start = 0; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }
                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        float nextX = x + glyph.XAdvance + SpacingX;
                        float h = y + LineSpacing;

                        if (xPos >= x && xPos <= nextX && yPos <= h && yPos >= y)
                        {
                            if (xPos < (x + nextX) * 0.5) { return i; }
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

        internal void Draw(SpriteBatch2 spriteBatch2, StringBuilder text, in Vector2 position, in Color color,
            float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            for (int i = 0; i < text.Length; i++)
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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch2 spriteBatch2, StringBuilder text, int start, int end, in Vector2 position,
            in Color color, float rotation, in Vector2 origin, float opacity, SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        internal void Draw(SpriteBatch2 spriteBatch2, StringBuilder text, int start, int end, in Vector2 position,
            in Size2F dimension, in Color color, float rotation, in Vector2 origin, float opacity,
            SpriteEffects effects, float layerDepth)
        {
            if (end <= start || end > text.Length) { end = text.Length; }

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
                        key = 0;
                        x = 0;
                        y += LineSpacing;
                    }
                        break;
                    default:
                    {
                        if (!_glyphs.TryGetValue(c, out SpriteFont.Glyph glyph))
                        {
                            if (IgnoreUnknownCharacters)
                            {
                                continue;
                            }

                            glyph = _defaultGlyph;
                        }
                        key |= c;

                        float dx = glyph.OffsetX;
                        if (Kernings.TryGetValue(key, out SpriteFont.Kerning kerning))
                        {
                            dx += kerning.Offset;
                        }

                        if (x + dx + glyph.Subrect.Width > dimension.Width) { return; }
                        if (y + glyph.OffsetY + glyph.Subrect.Height > dimension.Height) { return; }

                        spriteBatch2.DrawFont(
                            _texture2, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY), glyph.Subrect,
                            color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                        x += glyph.XAdvance + SpacingX;
                    }
                        break;
                }

                key <<= 16;
            }
        }

        #endregion

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _texture2);
                    _glyphs.Clear();
                    Kernings.Clear();
                }

                Kernings = null;
                _glyphs = null;

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    internal sealed class SpriteFontCW : AContentSerializationWriter<SpriteFont>
    {
        public override void WriteContext(ContentSerializationContext context, SpriteFont obj)
        {
            context.Set("Face", obj.Face);
            context.Set("Size", obj.Size);

            context.Set("Bold", obj.Bold);
            context.Set("Italic", obj.Italic);

            context.Set("DefaultCharacter", obj.DefaultCharacter);
            context.Set("LineSpacing", obj.LineSpacing);

            context.Set("SpacingX", obj.SpacingX);
            context.Set("SpacingY", obj.SpacingY);

            context.Set("Glyphs", obj.Glyphs);
            context.Set("Kernings", obj.Kernings);

            context.Set("ImageData", Convert.ToBase64String(obj.ImageData));
        }
    }

    internal sealed class SpriteFontGlyphCW : AContentSerializationWriter<SpriteFont.Glyph>
    {
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Glyph obj)
        {
            context.Set("Character", obj.Character);
            context.Set("Subrect", obj.Subrect);
            context.Set("OffsetX", obj.OffsetX);
            context.Set("OffsetY", obj.OffsetY);
            context.Set("XAdvance", obj.XAdvance);
        }
    }

    internal sealed class SpriteFontKerningCW : AContentSerializationWriter<SpriteFont.Kerning>
    {
        public override void WriteContext(ContentSerializationContext context, SpriteFont.Kerning obj)
        {
            context.Set("First", obj.First);
            context.Set("Second", obj.Second);
            context.Set("Offset", obj.Offset);
        }
    }

    internal sealed class SpriteFontCR : AContentSerializationReader<SpriteFont>
    {
        public override SpriteFont ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont
            {
                Face = context.Get<string>("Face"),
                Size = context.Get<int>("Size"),
                Bold = context.Get<bool>("Bold"),
                Italic = context.Get<bool>("Italic"),

                DefaultCharacter = context.Get<int>("DefaultCharacter"),
                LineSpacing = context.Get<int>("LineSpacing"),

                SpacingX = context.Get<int>("SpacingX"),
                SpacingY = context.Get<int>("SpacingY"),

                Glyphs = context.Get<Dictionary<int, SpriteFont.Glyph>>("Glyphs"),
                Kernings = context.Get<Dictionary<int, SpriteFont.Kerning>>("Kernings"),

                ImageData = Convert.FromBase64String(context.Get<string>("ImageData"))
            };
        }
    }

    internal sealed class SpriteFontGlyphCR : AContentSerializationReader<SpriteFont.Glyph>
    {
        public override SpriteFont.Glyph ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Glyph
            {
                Character = context.Get<int>("Character"),
                Subrect = context.Get<Rectangle>("Subrect"),
                OffsetX = context.Get<int>("OffsetX"),
                OffsetY = context.Get<int>("OffsetY"),
                XAdvance = context.Get<int>("XAdvance")
            };
        }
    }

    internal sealed class SpriteFontKerningCR : AContentSerializationReader<SpriteFont.Kerning>
    {
        public override SpriteFont.Kerning ReadContext(ContentSerializationContext context)
        {
            return new SpriteFont.Kerning
            {
                First = context.Get<int>("First"),
                Second = context.Get<int>("Second"),
                Offset = context.Get<int>("Offset")
            };
        }
    }
}
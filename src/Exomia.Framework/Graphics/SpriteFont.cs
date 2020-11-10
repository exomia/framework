#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite font. This class cannot be inherited.
    /// </summary>
    [ContentReadable(typeof(SpriteFontContentReader))]
    [ContentSerializable(typeof(SpriteFontContentSerializationReader), typeof(SpriteFontContentSerializationWriter))]
    public sealed class SpriteFont : IDisposable
    {
        private Glyph                  _defaultGlyph;
        private Dictionary<int, Glyph> _glyphs;
        private Texture                _texture;

        /// <summary>
        ///     Gets or sets a value indicating whether the bold.
        /// </summary>
        /// <value>
        ///     True if bold, false if not.
        /// </value>
        public bool Bold { get; set; }

        /// <summary>
        ///     Gets or sets the default character.
        /// </summary>
        /// <value>
        ///     The default character.
        /// </value>
        public int DefaultCharacter { get; set; } = -1;

        /// <summary>
        ///     Gets or sets the default glyph.
        /// </summary>
        /// <value>
        ///     The default glyph.
        /// </value>
        public Glyph DefaultGlyph
        {
            get { return _defaultGlyph; }
            set { _defaultGlyph = value; }
        }

        /// <summary>
        ///     Gets or sets the face.
        /// </summary>
        /// <value>
        ///     The face.
        /// </value>
        public string? Face { get; set; }

        /// <summary>
        ///     Gets or sets the glyphs.
        /// </summary>
        /// <value>
        ///     The glyphs.
        /// </value>
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

        /// <summary>
        ///     Gets or sets a value indicating whether the ignore unknown characters.
        /// </summary>
        /// <value>
        ///     True if ignore unknown characters, false if not.
        /// </value>
        public bool IgnoreUnknownCharacters { get; set; }

        /// <summary>
        ///     Gets or sets information describing the image.
        /// </summary>
        /// <value>
        ///     Information describing the image.
        /// </value>
        public byte[] ImageData { get; set; } = Array.Empty<byte>();

        /// <summary>
        ///     Gets or sets a value indicating whether the italic.
        /// </summary>
        /// <value>
        ///     True if italic, false if not.
        /// </value>
        public bool Italic { get; set; }

        /// <summary>
        ///     Gets or sets the kernings.
        /// </summary>
        /// <value>
        ///     The kernings.
        /// </value>
        public Dictionary<int, Kerning> Kernings { get; set; }

        /// <summary>
        ///     Gets or sets the line spacing.
        /// </summary>
        /// <value>
        ///     The line spacing.
        /// </value>
        public int LineSpacing { get; set; }

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public int Size { get; set; }

        /// <summary>
        ///     Gets or sets the spacing x coordinate.
        /// </summary>
        /// <value>
        ///     The spacing x coordinate.
        /// </value>
        public int SpacingX { get; set; }

        /// <summary>
        ///     Gets or sets the spacing y coordinate.
        /// </summary>
        /// <value>
        ///     The spacing y coordinate.
        /// </value>
        public int SpacingY { get; set; }

        /// <summary>
        ///     Gets or sets the texture.
        /// </summary>
        /// <value>
        ///     The texture.
        /// </value>
        public Texture Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteFont" /> class.
        /// </summary>
        public SpriteFont()
        {
            _glyphs  = new Dictionary<int, Glyph>();
            Kernings = new Dictionary<int, Kerning>();
            _texture = Texture.Empty;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SpriteFont" /> class.
        /// </summary>
        ~SpriteFont()
        {
            Dispose(false);
        }

        /// <summary>
        ///     A glyph.
        /// </summary>
        [ContentSerializable(typeof(SpriteFontGlyphCR), typeof(SpriteFontGlyphCW))]
        public struct Glyph
        {
            /// <summary>
            ///     The character.
            /// </summary>
            public int Character;

            /// <summary>
            ///     The subrect.
            /// </summary>
            public Rectangle Subrect;

            /// <summary>
            ///     The offset x coordinate.
            /// </summary>
            public int OffsetX;

            /// <summary>
            ///     The offset y coordinate.
            /// </summary>
            public int OffsetY;

            /// <summary>
            ///     The advance.
            /// </summary>
            public int XAdvance;
        }

        /// <summary>
        ///     A kerning.
        /// </summary>
        [ContentSerializable(typeof(SpriteFontKerningCR), typeof(SpriteFontKerningCW))]
        public struct Kerning
        {
            /// <summary>
            ///     The first.
            /// </summary>
            public int First;

            /// <summary>
            ///     The second.
            /// </summary>
            public int Second;

            /// <summary>
            ///     The offset.
            /// </summary>
            public int Offset;
        }

        /// <summary>
        ///     Draw font.
        /// </summary>
        /// <param name="texture">         The texture. </param>
        /// <param name="position">        The position. </param>
        /// <param name="sourceRectangle"> Source rectangle. </param>
        /// <param name="color">           The color. </param>
        /// <param name="rotation">        The rotation. </param>
        /// <param name="origin">          The origin. </param>
        /// <param name="scale">           The scale. </param>
        /// <param name="opacity">         The opacity. </param>
        /// <param name="effects">         The effects. </param>
        /// <param name="layerDepth">      Depth of the layer. </param>
        internal delegate void DrawFont(Texture       texture,
                                        in Vector2    position,
                                        in Rectangle? sourceRectangle,
                                        in Color      color,
                                        float         rotation,
                                        in Vector2    origin,
                                        float         scale,
                                        float         opacity,
                                        SpriteEffects effects,
                                        float         layerDepth);

        #region String

        /// <summary>
        ///     Measure text.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns>
        ///     A Vector2.
        /// </returns>
        public Vector2 MeasureText(string text)
        {
            return MeasureText(text, 0, text.Length);
        }

        /// <summary>
        ///     Measure text.
        /// </summary>
        /// <param name="text">  The text. </param>
        /// <param name="start"> The start. </param>
        /// <param name="end">   The end. </param>
        /// <returns>
        ///     A Vector2.
        /// </returns>
        public Vector2 MeasureText(string text, int start, int end)
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
        public int HitTest(string text, int start, int end, float xPos, float yPos)
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

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="position">     The position. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           string        text,
                           in Vector2    position,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            int l = text.Length;
            for (int i = 0; i < l; i++)
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
                                glyph.Subrect,
                                color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                            x += glyph.XAdvance + SpacingX;
                        }
                        break;
                }

                key <<= 16;
            }
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="start">        The start. </param>
        /// <param name="end">          The end. </param>
        /// <param name="position">     The position. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           string        text,
                           int           start,
                           int           end,
                           in Vector2    position,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
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
                                glyph.Subrect,
                                color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                            x += glyph.XAdvance + SpacingX;
                        }
                        break;
                }

                key <<= 16;
            }
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="start">        The start. </param>
        /// <param name="end">          The end. </param>
        /// <param name="position">     The position. </param>
        /// <param name="dimension">    The dimension. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           string        text,
                           int           start,
                           int           end,
                           in Vector2    position,
                           in Size2F     dimension,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
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
                                glyph.Subrect,
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

        /// <summary>
        ///     Measure text.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns>
        ///     A Vector2.
        /// </returns>
        public Vector2 MeasureText(StringBuilder text)
        {
            return MeasureText(text, 0, text.Length);
        }

        /// <summary>
        ///     Measure text.
        /// </summary>
        /// <param name="text">  The text. </param>
        /// <param name="start"> The start. </param>
        /// <param name="end">   The end. </param>
        /// <returns>
        ///     A Vector2.
        /// </returns>
        public Vector2 MeasureText(StringBuilder text, int start, int end)
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
        public int HitTest(StringBuilder text, int start, int end, float xPos, float yPos)
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

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="position">     The position. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           StringBuilder text,
                           in Vector2    position,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
        {
            float x = 0;
            float y = 0;

            int key = 0;

            int l = text.Length;
            for (int i = 0; i < l; i++)
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
                            if (Kernings.TryGetValue(key, out Kerning kerning))
                            {
                                dx += kerning.Offset;
                            }

                            drawCallback(
                                _texture, new Vector2(position.X + x + dx, position.Y + y + glyph.OffsetY),
                                glyph.Subrect,
                                color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                            x += glyph.XAdvance + SpacingX;
                            break;
                        }
                }

                key <<= 16;
            }
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="start">        The start. </param>
        /// <param name="end">          The end. </param>
        /// <param name="position">     The position. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           StringBuilder text,
                           int           start,
                           int           end,
                           in Vector2    position,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
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
                                glyph.Subrect,
                                color, rotation, origin, 1.0f, opacity, effects, layerDepth);

                            x += glyph.XAdvance + SpacingX;
                        }
                        break;
                }

                key <<= 16;
            }
        }

        /// <summary>
        ///     Draws.
        /// </summary>
        /// <param name="drawCallback"> The draw callback. </param>
        /// <param name="text">         The text. </param>
        /// <param name="start">        The start. </param>
        /// <param name="end">          The end. </param>
        /// <param name="position">     The position. </param>
        /// <param name="dimension">    The dimension. </param>
        /// <param name="color">        The color. </param>
        /// <param name="rotation">     The rotation. </param>
        /// <param name="origin">       The origin. </param>
        /// <param name="opacity">      The opacity. </param>
        /// <param name="effects">      The effects. </param>
        /// <param name="layerDepth">   Depth of the layer. </param>
        internal void Draw(DrawFont      drawCallback,
                           StringBuilder text,
                           int           start,
                           int           end,
                           in Vector2    position,
                           in Size2F     dimension,
                           in Color      color,
                           float         rotation,
                           in Vector2    origin,
                           float         opacity,
                           SpriteEffects effects,
                           float         layerDepth)
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
                                glyph.Subrect,
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

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.SpriteFont and
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
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

                Kernings.Clear();
                _glyphs.Clear();

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Numerics;
using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.ContentSerialization;
using Exomia.Framework.Core.Mathematics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     A sprite font. This class cannot be inherited.
    /// </summary>
    [ContentReadable(typeof(SpriteFontContentReader))]
    [ContentSerializable(typeof(SpriteFontContentSerializationReader), typeof(SpriteFontContentSerializationWriter))]
    public sealed partial class SpriteFont : IDisposable
    {
        internal delegate void DrawFont(Texture        texture,
                                        in Vector2     position,
                                        in Rectangle?  sourceRectangle,
                                        in VkColor       color,
                                        float          rotation,
                                        in Vector2     origin,
                                        float          scale,
                                        float          opacity,
                                        TextureEffects effects,
                                        float          layerDepth);
        
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

                Kernings.Clear();
                _glyphs.Clear();

                _disposed = true;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SpriteFont" /> class.
        /// </summary>
        ~SpriteFont()
        {
            Dispose(false);
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
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <summary> A sprite font. This class cannot be inherited. </summary>
public sealed partial class SpriteFont : IDisposable
{
    private readonly string                 _face;
    private readonly int                    _size;
    private readonly int                    _lineHeight;
    private readonly Dictionary<int, Glyph> _glyphs;
    private readonly Dictionary<int, int>   _kernings;
    private readonly Texture                _texture;
    private          int                    _defaultCharacter;
    private          Glyph                  _defaultGlyph;

    /// <summary> Gets the face. </summary>
    /// <value> The face. </value>
    public string Face
    {
        get { return _face; }
    }

    /// <summary> Gets the size. </summary>
    /// <value> The size. </value>
    public int Size
    {
        get { return _size; }
    }

    /// <summary> Gets the line height. </summary>
    /// <value> The line height. </value>
    public int LineHeight
    {
        get { return _lineHeight; }
    }

    /// <summary> Gets the kernings. </summary>
    /// <value> The kernings. </value>
    /// <remarks>
    ///     key: (first character &lt;&lt; 16) | second character <br />
    ///     value: kerning amount
    /// </remarks>
    public Dictionary<int, int> Kernings
    {
        get { return _kernings; }
    }

    /// <summary> Gets the glyphs. </summary>
    /// <value> The glyphs. </value>
    public Dictionary<int, Glyph> Glyphs
    {
        get { return _glyphs; }
    }


    /// <summary> Gets or sets the texture. </summary>
    /// <value> The texture. </value>
    public Texture Texture
    {
        get { return _texture; }
    }

    /// <summary> Gets or sets the default character. </summary>
    /// <value> The default character. </value>
    public int DefaultCharacter
    {
        get { return _defaultCharacter; }
        set
        {
            _defaultCharacter = value;
            if (!_glyphs.TryGetValue(_defaultCharacter, out _defaultGlyph))
            {
                throw new ArgumentException("Error setting default character! No glyph found!", nameof(DefaultCharacter));
            }
        }
    }

    /// <summary> Gets or sets a value indicating whether the ignore unknown characters. </summary>
    /// <value> True if ignore unknown characters, false if not. </value>
    public bool IgnoreUnknownCharacters { get; init; }

    /// <summary> Gets or sets a value indicating whether the bold. </summary>
    /// <value> True if bold, false if not. </value>
    public bool Bold { get; init; }

    /// <summary> Gets or sets a value indicating whether the italic. </summary>
    /// <value> True if italic, false if not. </value>
    public bool Italic { get; init; }

    /// <summary> Initializes a new instance of the <see cref="SpriteFont" /> class. </summary>
    /// <param name="face"> The face. </param>
    /// <param name="size"> The size. </param>
    /// <param name="lineHeight"> The line height. </param>
    /// <param name="glyphs"> The glyphs. </param>
    /// <param name="kernings"> The kernings. </param>
    /// <param name="texture"> The texture. </param>
    /// <param name="defaultCharacter"> (Optional) The default character. </param>
    public SpriteFont(string face, int size, int lineHeight, Dictionary<int, Glyph> glyphs, Dictionary<int, int> kernings, Texture texture, int defaultCharacter = -1)
    {
        _face       = face;
        _size       = size;
        _lineHeight = lineHeight;
        _texture    = texture;
        _glyphs     = glyphs;
        _kernings   = kernings;

        _defaultCharacter = defaultCharacter;

        if (!_glyphs.TryGetValue(_defaultCharacter, out _defaultGlyph))
        {
            if (!_glyphs.TryGetValue(_defaultCharacter = '?', out _defaultGlyph))
            {
                throw new ArgumentException("Error setting default character! No glyph found!", nameof(defaultCharacter));
            }
        }
    }

    #region IDisposable Support

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Glyphs.Clear();
                Kernings.Clear();
            }

            _texture.Dispose();
            _disposed = true;
        }
    }

    /// <summary> Finalizes an instance of the <see cref="SpriteFont" /> class. </summary>
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
#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <summary> A msdf font. This class cannot be inherited. </summary>
public sealed partial class MsdfFont : IDisposable
{
    private readonly string _name;
    private readonly AtlasInfo _atlas;
    private readonly MetricsData _metrics;
    private readonly Dictionary<int, Glyph> _glyphs;
    private readonly Dictionary<int, double> _kernings;
    private readonly Texture _texture;

    /// <summary> Gets the name. </summary>
    /// <value> The name. </value>
    public string Name
    {
        get { return _name; }
    }

    /// <summary> Gets the atlas information. </summary>
    /// <value> The atlas information. </value>
    public AtlasInfo Atlas
    {
        get { return _atlas; }
    }

    /// <summary> Gets the metrics data. </summary>
    /// <value> The metrics data. </value>
    public MetricsData Metrics
    {
        get { return _metrics; }
    }

    /// <summary> Gets the glyphs. </summary>
    /// <value> The glyphs. </value>
    public Dictionary<int, Glyph> Glyphs
    {
        get { return _glyphs; }
    }

    /// <summary> Gets the kernings. </summary>
    /// <value> The kernings. </value>
    /// <remarks>
    ///     key: (first character &lt;&lt; 16) | second character <br />
    ///     value: kerning amount
    /// </remarks>
    public Dictionary<int, double> Kernings
    {
        get { return _kernings; }
    }

    /// <summary> Gets or sets the texture. </summary>
    /// <value> The texture. </value>
    public Texture Texture
    {
        get { return _texture; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsdfFont"/> class
    /// </summary>
    /// <param name="name">The name</param>
    /// <param name="atlas">The atlas</param>
    /// <param name="metrics">The metrics</param>
    /// <param name="glyphs">The glyphs</param>
    /// <param name="kernings">The kernings</param>
    /// <param name="texture">The texture</param>
    public MsdfFont(
        string                  name,
        AtlasInfo               atlas,
        MetricsData             metrics,
        Dictionary<int, Glyph>  glyphs,
        Dictionary<int, double> kernings,
        Texture                 texture)
    {
        _name     = name;
        _atlas    = atlas;
        _metrics  = metrics;
        _glyphs   = glyphs;
        _kernings = kernings;
        _texture  = texture;
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

    /// <summary> Finalizes an instance of the <see cref="MsdfFont" /> class. </summary>
    ~MsdfFont()
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
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;

// ReSharper disable MemberHidesStaticFromOuterClass
namespace Exomia.Framework.Core.Content.Protocols;

/// <summary> The e1 protocol. This class cannot be inherited. </summary>
/// <remarks>
///     Protocol definition:<br />
///     - 4 bytes: e1 magic header (0x40, 0x65, 0x78, 0x31)<br />
///     - N bytes: type magic header (e.g. <see cref="Texture.MagicHeader" />, <see cref="Spritefont.MagicHeader" /><br />
///     - 2 bytes: type protocol version (e.g. <see cref="Texture.Version10" />, <see cref="Spritefont.Version10" /><br />
///     - 4 bytes: type reserved bytes for future use<br />
///     - 1 bytes: compression mode (<see cref="CompressMode" />)<br />
///     - N bytes: (compressed) data<br />
/// </remarks>
public static class E1Protocol
{
    /// <summary> Name of the e1 extension. </summary>
    public const string EXTENSION_NAME = ".e1";

    /// <summary> The type protocol version length. </summary>
    public const int TYPE_PROTOCOL_VERSION_LENGHT = 2;

    /// <summary> The e1 magic header. </summary>
    public static readonly byte[] MagicHeader = { 0x40, 0x65, 0x78, 0x31 };

    /// <summary> The texture protocol information. This class cannot be inherited. </summary>
    public static class Texture
    {
        /// <summary> The texture magic header. </summary>
        public static readonly byte[] MagicHeader = { 0x40, 0x74, 0x65, 0x78 };

        /// <summary> The texture protocol version 1.0 </summary>
        /// <remarks>
        ///     Protocol definition:<br />
        ///     - 4 bytes (int): the texture width<br />
        ///     - 4 bytes (int): the texture height<br />
        ///     - "width * height" Pixel: the texture pixel data<br />
        ///     -> Pixel<br />
        ///         - 1 byte (byte): red value<br />
        ///         - 1 byte (byte): green value<br />
        ///         - 1 byte (byte): blue value<br />
        ///         - 1 byte (byte): alpha value<br />
        /// </remarks>
        public static readonly byte[] Version10 = { 0x01, 0x00 };
    }

    /// <summary> The spritefont protocol information. This class cannot be inherited. </summary>
    public static class Spritefont
    {
        /// <summary> The spritefont magic header. </summary>
        public static readonly byte[] MagicHeader = { 0x40, 0x73, 0x66, 0x6E, 0x74 };

        /// <summary> The spritefont protocol version 1.0 </summary>
        /// <remarks>
        ///     Protocol definition:<br />
        ///     - N bytes (string): face string with prefixed length encoded as an integer 7 bits at a time<br />
        ///     - 4 bytes (int): the font size<br />
        ///     - 4 bytes (int): the font line height<br />
        ///     - 4 bytes (int): the font default character <br />
        ///     - 1 bytes (boolean): != 0 if font has to ignore unknown characters<br />
        ///     - 1 bytes (boolean): != 0 if font is bold<br />
        ///     - 1 bytes (boolean): != 0 if font is italic<br />
        ///     - 4 bytes (int): the glyph count<br />
        ///     - "glyph count" Glyph: the glyph data<br />
        ///     -> Glyph<br />
        ///         - 4 bytes (int): the character<br />
        ///         - 4 bytes (int): the x<br />
        ///         - 4 bytes (int): the y<br />
        ///         - 4 bytes (int): the width<br />
        ///         - 4 bytes (int): the height<br />
        ///         - 4 bytes (int): the x offset<br />
        ///         - 4 bytes (int): the y offset<br />
        ///         - 4 bytes (int): the x advance<br />
        ///     - 4 bytes (int): the kerning count<br />
        ///     - "kerning count" Kerning: the kerning data<br />
        ///     -> Kerning<br />
        ///         - 4 bytes (int): the first character<br />
        ///         - 4 bytes (int): the first character<br />
        ///         - 4 bytes (int): the x offset<br />
        ///         - 4 bytes (int): the texture width<br />
        ///         - 4 bytes (int): the texture height<br />
        ///     - "width * height" Pixel: the texture pixel data<br />
        ///     -> Pixel<br />
        ///         - 1 byte (byte): red value<br />
        ///         - 1 byte (byte): green value<br />
        ///         - 1 byte (byte): blue value<br />
        ///         - 1 byte (byte): alpha value<br />
        /// </remarks>
        public static readonly byte[] Version10 = { 0x01, 0x00 };
    }
}

// ReSharper enable MemberHidesStaticFromOuterClass
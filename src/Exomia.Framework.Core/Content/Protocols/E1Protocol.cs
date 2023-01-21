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
///     <code>
///     Protocol definition:<br />
///     - 4 bytes: e1 magic header (<see cref="MagicHeader" />)<br />
///     - 8 bytes: type magic header (e.g. <see cref="Texture.MagicHeader" />, <see cref="Spritefont.MagicHeader" /><br />
///     - 4 bytes: type reserved bytes for future use<br />
///     - 2 bytes: type protocol version (e.g. <see cref="Texture.Version10" />, <see cref="Spritefont.Version10" /><br />
///     - 1 bytes: compression mode (<see cref="CompressMode" />)<br />
///     - N bytes: (compressed) data<br />
///     </code>
/// </remarks>
public static class E1Protocol
{
    /// <summary> Name of the e1 extension. </summary>
    public const string EXTENSION_NAME = ".e1";

    /// <summary> The magic header length. </summary>
    public const int MAGIC_HEADER_LENGHT = 4;

    /// <summary> The type reserved bytes length. </summary>
    public const int TYPE_RESERVED_BYTES_LENGHT = 4;

    /// <summary> The type magic header length. </summary>
    public const int TYPE_MAGIC_HEADER_LENGHT = 8;

    /// <summary> The type protocol version length. </summary>
    public const int TYPE_PROTOCOL_VERSION_LENGHT = 2;

    /// <summary> The e1 magic header. </summary>
    public static readonly byte[] MagicHeader = new byte[MAGIC_HEADER_LENGHT] { 0x40, 0x65, 0x78, 0x31 };

    /// <summary> The e1 texture protocol information. This class cannot be inherited. </summary>
    public static class Texture
    {
        /// <summary> The texture magic header. </summary>
        public static readonly byte[] MagicHeader = new byte[TYPE_MAGIC_HEADER_LENGHT] { 0x40, 0x74, 0x65, 0x78, 0x00, 0x00, 0x00, 0x00 };

        /// <summary> The texture protocol version 1.0 </summary>
        /// <remarks>
        ///     <code>
        ///     Protocol definition:<br />
        ///     - 4 bytes (int): the texture width<br />
        ///     - 4 bytes (int): the texture height<br />
        ///     - "width * height" Pixel: the texture pixel data<br />
        ///       -> Pixel<br />
        ///         - 1 byte (byte): red value<br />
        ///         - 1 byte (byte): green value<br />
        ///         - 1 byte (byte): blue value<br />
        ///         - 1 byte (byte): alpha value<br />
        ///     </code>
        /// </remarks>
        public static readonly byte[] Version10 = new byte[TYPE_PROTOCOL_VERSION_LENGHT] { 0x31, 0x30 };
    }

    /// <summary> The e1 spritefont protocol information. This class cannot be inherited. </summary>
    public static class Spritefont
    {
        /// <summary> The spritefont magic header. </summary>
        public static readonly byte[] MagicHeader = new byte[TYPE_MAGIC_HEADER_LENGHT] { 0x40, 0x73, 0x66, 0x6E, 0x74, 0x00, 0x00, 0x00 };

        /// <summary> The spritefont protocol version 1.0 </summary>
        /// <remarks>
        ///     <code>
        ///     Protocol definition:<br />
        ///     - N bytes (string): the face string with prefixed length encoded as an integer 7 bits at a time<br />
        ///     - 4 bytes (int): the font size<br />
        ///     - 4 bytes (int): the font line height<br />
        ///     - 4 bytes (int): the font default character <br />
        ///     - 1 bytes (boolean): != 0 if font has to ignore unknown characters<br />
        ///     - 1 bytes (boolean): != 0 if font is bold<br />
        ///     - 1 bytes (boolean): != 0 if font is italic<br />
        ///     - 4 bytes (int): the glyph count<br />
        ///     - "glyph count" Glyph: the glyph data<br />
        ///       -> Glyph<br />
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
        ///       -> Kerning<br />
        ///         - 4 bytes (int): the first character<br />
        ///         - 4 bytes (int): the second character<br />
        ///         - 4 bytes (int): the x offset<br />
        ///     - 4 bytes (int): the texture width<br />
        ///     - 4 bytes (int): the texture height<br />
        ///     - "width * height" Pixel: the texture pixel data<br />
        ///       -> Pixel<br />
        ///         - 1 byte (byte): red value<br />
        ///         - 1 byte (byte): green value<br />
        ///         - 1 byte (byte): blue value<br />
        ///         - 1 byte (byte): alpha value<br />
        ///     </code>
        /// </remarks>
        public static readonly byte[] Version10 = new byte[TYPE_PROTOCOL_VERSION_LENGHT] { 0x31, 0x30 };
    }
    
    /// <summary> The e1 msdf font protocol information. This class cannot be inherited. </summary>
    public static class MsdfFont
    {
        /// <summary> The spritefont magic header. </summary>
        public static readonly byte[] MagicHeader = new byte[TYPE_MAGIC_HEADER_LENGHT] { 0x40, 0x6D, 0x73, 0x64, 0x66, 0x66, 0x6E, 0x74 };

        /// <summary> The msdf font protocol version 1.0 </summary>
        /// <remarks>
        ///     <code>
        ///     Protocol definition:<br />
        ///     - N bytes (string): the face string with prefixed length encoded as an integer 7 bits at a time<br />
        ///     - N bytes (string): the atlas type string with prefixed length encoded as an integer 7 bits at a time<br />
        ///     - 4 bytes (int): the atlas distance range<br />
        ///     - 4 bytes (double): the atlas size<br />
        ///     - 4 bytes (int): the atlas width<br />
        ///     - 4 bytes (int): the atlas height<br />
        ///     - N bytes (string): the atlas y origin string with prefixed length encoded as an integer 7 bits at a time<br />
        ///     - 4 bytes (int): the metrics em size<br />
        ///     - 4 bytes (double): the metrics line height<br />
        ///     - 4 bytes (double): the metrics ascender<br />
        ///     - 4 bytes (double): the metrics descender<br />
        ///     - 4 bytes (double): the metrics underline y<br />
        ///     - 4 bytes (double): the metrics underline thickness<br />
        ///     - 4 bytes (int): the glyph count<br />
        ///     - "glyph count" Glyph: the glyph data<br />
        ///       -> Glyph<br />
        ///         - 4 bytes (int): the unicode<br />
        ///         - 8 bytes (double): the advance<br />
        ///         - 1 byte (byte): != 0 if plane bounds are set<br />
        ///           -> plane bounds<br />
        ///             - 8 bytes (double): the plane top<br />
        ///             - 8 bytes (double): the plane right<br />
        ///             - 8 bytes (double): the plane bottom<br />
        ///             - 8 bytes (double): the plane left<br />
        ///         - 1 byte (byte): != 0 if atlas bounds are set<br />
        ///           -> atlas bounds<br />
        ///             - 8 bytes (double): the atlas top<br />
        ///             - 8 bytes (double): the atlas right<br />
        ///             - 8 bytes (double): the atlas bottom<br />
        ///             - 8 bytes (double): the atlas left<br />
        ///     - 4 bytes (int): the kerning count<br />
        ///     - "kerning count" Kerning: the kerning data<br />
        ///       -> Kerning<br />
        ///         - 4 bytes (int): the unicode1<br />
        ///         - 4 bytes (int): the unicode2<br />
        ///         - 8 bytes (double): the advance<br />
        ///     - 4 bytes (int): the texture width<br />
        ///     - 4 bytes (int): the texture height<br />
        ///     - "width * height" Pixel: the texture pixel data<br />
        ///       -> Pixel<br />
        ///         - 1 byte (byte): red value<br />
        ///         - 1 byte (byte): green value<br />
        ///         - 1 byte (byte): blue value<br />
        ///         - 1 byte (byte): alpha value<br />
        ///     </code>
        /// </remarks>
        public static readonly byte[] Version10 = new byte[TYPE_PROTOCOL_VERSION_LENGHT] { 0x31, 0x30 };
    }
}

// ReSharper enable MemberHidesStaticFromOuterClass
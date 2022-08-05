#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;

namespace Exomia.Framework.Core.Content.Protocols;

/// <summary> The e1 protocol. This class cannot be inherited. </summary>
/// <remarks>
///     Protocol definition:<br/>
///     - 4 bytes: e1 magic header (0x40, 0x65, 0x78, 0x31)<br/>
///     - N bytes: type magic header (e.g. <see cref="Texture.MagicHeader"/>, <see cref="Spritefont.MagicHeader"/><br/>
///     - 2 bytes: type protocol version (e.g. <see cref="Texture.Version10"/>, <see cref="Spritefont.Version10"/><br/>
///     - 4 bytes: type reserved bytes for future use<br/>
///     - 1 bytes: compression mode (0x00 = no compression, 0x01 = GZIP) <see cref="CompressMode"/><br/>
///     - N bytes: (compressed) data<br/>
/// </remarks>
public sealed class E1Protocol
{
    /// <summary> Name of the e1 extension. </summary>
    public const string EXTENSION_NAME = ".e1";

    /// <summary> The e1 magic header. </summary>
    public static readonly byte[] MagicHeader = { 0x40, 0x65, 0x78, 0x31 };
    
    /// <summary> The type protocol version length. </summary>
    public const int TYPE_PROTOCOL_VERSION_LENGHT = 2;
    
    /// <summary> The texture protocol information. This class cannot be inherited. </summary>
    public sealed class Texture
    {
        /// <summary> The texture magic header. </summary>
        public static readonly byte[] MagicHeader = { 0x40, 0x74, 0x65, 0x78 };
        
        /// <summary> The texture protocol version 1.0 </summary>
        public static readonly byte[] Version10 = { 0x01, 0x00 };
    }

    /// <summary> The spritefont protocol information. This class cannot be inherited. </summary>
    public sealed class Spritefont
    {
        /// <summary> The spritefont magic header. </summary>
        public static readonly byte[] MagicHeader = { 0x40, 0x73, 0x66, 0x6E, 0x74 };
        
        /// <summary> The spritefont protocol version 1.0 </summary>
        public static readonly byte[] Version10 = { 0x01, 0x00 };
    }
}
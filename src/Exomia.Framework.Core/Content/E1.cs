#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content;

/// <summary> A e1. </summary>
public class E1
{
    /// <summary> Name of the e1 extension. </summary>
    public const string EXTENSION_NAME = ".e1";

    /// <summary> The magic header. </summary>
    public static readonly byte[] MagicHeader = { 0x40, 0x65, 0x78, 0x31 };

    /// <summary> The texture magic header. </summary>
    public static readonly byte[] TextureMagicHeader = { 0x40, 0x74, 0x65, 0x78 };

    /// <summary> The texture magic header. </summary>
    public static readonly byte[] SpritefontMagicHeader = { 0x40, 0x73, 0x66, 0x6E, 0x74 };
}
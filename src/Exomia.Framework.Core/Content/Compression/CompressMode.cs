#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content.Compression;

/// <summary> Values that represent CompressMode. </summary>
public enum CompressMode : byte
{
    /// <summary> An enum constant representing the none option. </summary>
    None = 0x00,

    /// <summary> An enum constant representing the gzip option. </summary>
    Gzip = 0x01
}
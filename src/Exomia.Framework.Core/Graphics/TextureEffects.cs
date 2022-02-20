#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <summary>
///     Bitfield of flags for specifying TextureEffects.
/// </summary>
[Flags]
public enum TextureEffects
{
    /// <summary>
    ///     A binary constant representing the none flag.
    /// </summary>
    None = 0b00,

    /// <summary>
    ///     A binary constant representing the flip horizontally flag.
    /// </summary>
    FlipHorizontally = 0b01,

    /// <summary>
    ///     A binary constant representing the flip vertically flag.
    /// </summary>
    FlipVertically = 0b10,

    /// <summary>
    ///     A binary constant representing the flip both flag.
    /// </summary>
    FlipBoth = FlipHorizontally | FlipVertically
}
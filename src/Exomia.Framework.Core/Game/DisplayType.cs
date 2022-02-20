#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Game;

/// <summary>
///     Values that represent DisplayType.
/// </summary>
public enum DisplayType
{
    /// <summary>
    ///     An enum constant representing the fullscreen option.
    /// </summary>
    Fullscreen = 1 << 0,

    /// <summary>
    ///     An enum constant representing the window option.
    /// </summary>
    Window = 1 << 1,

    /// <summary>
    ///     An enum constant representing the fullscreen window option.
    /// </summary>
    FullscreenWindow = 1 << 2
}
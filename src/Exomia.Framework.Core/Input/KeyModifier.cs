#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Input;

/// <summary>
///     Values that represent KeyModifier.
/// </summary>
[Flags]
public enum KeyModifier
{
    /// <summary>
    ///     An enum constant representing the shift option.
    /// </summary>
    Shift = 1,

    /// <summary>
    ///     An enum constant representing the Alternate option.
    /// </summary>
    Alt = 2,

    /// <summary>
    ///     An enum constant representing the control option.
    /// </summary>
    Control = 4
}
﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Windows.Application.Desktop;

/// <summary> Values that represent FormWindowState. </summary>
public enum FormWindowState
{
    /// <summary>
    ///     An enum constant representing the normal option.
    /// </summary>
    Normal,

    /// <summary>
    ///     An enum constant representing the minimized option.
    /// </summary>
    Minimized,

    /// <summary>
    ///     An enum constant representing the maximized option.
    /// </summary>
    Maximized
}

/// <summary> Values that represent FormBorderStyle. </summary>
public enum FormBorderStyle
{
    /// <summary>
    ///     No frame.
    /// </summary>
    None,

    /// <summary>
    ///     A frame whose size can not be changed.
    /// </summary>
    Fixed,

    /// <summary>
    ///     A frame whose size can be changed.
    /// </summary>
    Sizable
}
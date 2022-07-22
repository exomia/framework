#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using Exomia.Framework.Core;
using Exomia.Framework.Core.Application;

namespace Exomia.Framework.Windows.Application.Desktop;

/// <summary> Interface for win32 render form. </summary>
public interface IWin32RenderForm : IRenderForm
{
    /// <summary> Occurs when the mouse leaves the client area. </summary>
    event EventHandler<IWin32RenderForm, IntPtr>? MouseLeave;

    /// <summary> Occurs when the mouse enters the client area. </summary>
    event EventHandler<IWin32RenderForm, IntPtr>? MouseEnter;

    /// <summary> Gets or sets the state of the window. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <exception cref="Win32Exception">              Thrown when a win32 error condition occurs. </exception>
    /// <value> The window state. </value>
    FormWindowState WindowState { get; set; }

    /// <summary> Gets or sets the state of the window. </summary>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <exception cref="Win32Exception">              Thrown when a win32 error condition occurs. </exception>
    /// <value> The window state. </value>
    FormBorderStyle BorderStyle { get; set; }
}
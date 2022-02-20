#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct WndClassEx
{
    [MarshalAs(UnmanagedType.U4)]
    public int cbSize;

    [MarshalAs(UnmanagedType.U4)]
    public ClassStyles style;

    public WndProc lpfnWndProc;
    public int     cbClsExtra;
    public int     cbWndExtra;
    public IntPtr  hInstance;
    public IntPtr  hIcon;
    public IntPtr  hCursor;
    public IntPtr  hbrBackground;

    [MarshalAs(UnmanagedType.LPStr)]
    public string lpszMenuName;

    [MarshalAs(UnmanagedType.LPStr)]
    public string lpszClassName;

    public IntPtr hIconSm;
}
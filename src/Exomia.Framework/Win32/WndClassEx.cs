﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Win32
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    struct WndClassEx
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
}
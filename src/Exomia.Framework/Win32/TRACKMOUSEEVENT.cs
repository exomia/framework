#region License

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
    [StructLayout(LayoutKind.Sequential)]
    struct TRACKMOUSEEVENT
    {
        public int cbSize;
        [MarshalAs(UnmanagedType.U4)]
        public uint dwFlags;
        public IntPtr hWnd;
        public uint dwHoverTime;

        public TRACKMOUSEEVENT(uint dwFlags, IntPtr hWnd, uint dwHoverTime)
        {
            this.cbSize      = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
            this.dwFlags     = dwFlags;
            this.hWnd        = hWnd;
            this.dwHoverTime = dwHoverTime;
        }
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public IntPtr  hWnd;
        public uint    msg;
        public IntPtr  wParam;
        public IntPtr  lParam;
        public uint    time;
        public Vector2 pt;
    }
}
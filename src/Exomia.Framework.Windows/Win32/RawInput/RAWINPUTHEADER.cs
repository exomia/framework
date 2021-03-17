#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32.RawInput
{
    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTHEADER
    {
        /// <summary> Type of device the input is coming from. </summary>
        public int Type;

        /// <summary> Size of the packet of data. </summary>
        public int Size;

        /// <summary> Handle to the device sending the data. </summary>
        public IntPtr Device;

        /// <summary> wParam from the window message. </summary>
        public IntPtr wParam;
    }
}
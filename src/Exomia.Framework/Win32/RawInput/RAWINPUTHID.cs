#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Win32.RawInput
{
    /// <summary>
    ///     Value type for raw input from a HID.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    struct RAWINPUTHID
    {
        /// <summary>Size of the HID data in bytes.</summary>
        public int Size;

        /// <summary>Number of HID in Data.</summary>
        public int Count;

        /// <summary>Data for the HID.</summary>
        public IntPtr Data;
    }
}
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
namespace Exomia.Framework.Windows.Win32.RawInput
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RAWINPUTHID
    {
        /// <summary> Size of the HID data in bytes. </summary>
        public int Size;

        /// <summary> Number of HID in Data. </summary>
        public int Count;

        /// <summary> Data for the HID. </summary>
        public IntPtr Data;
    }
}
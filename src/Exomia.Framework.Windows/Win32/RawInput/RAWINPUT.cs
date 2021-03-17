#region License

// Copyright (c) 2018-2020, exomia
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
    struct RAWINPUT
    {
        /// <summary> The header. </summary>
        public RAWINPUTHEADER Header;

        /// <summary> The data. </summary>
        public Union Data;

        /// <summary> An union. </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            /// <summary> Mouse raw input data. </summary>
            [FieldOffset(0)]
            public RAWINPUTMOUSE Mouse;

            /// <summary> Keyboard raw input data. </summary>
            [FieldOffset(0)]
            public RAWINPUTKEYBOARD Keyboard;

            /// <summary> HID raw input data. </summary>
            [FieldOffset(0)]
            public RAWINPUTHID HID;
        }
    }
}
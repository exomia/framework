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
    [StructLayout(LayoutKind.Explicit)]
    internal struct RAWINPUTMOUSE
    {
        /// <summary> The mouse state. </summary>
        [FieldOffset(0)]
        public RawMouseFlags Flags;

        /// <summary> Flags for the event. </summary>
        [FieldOffset(4)]
        public RawMouseButtons ButtonFlags;

        /// <summary> If the mouse wheel is moved, this will contain the delta amount. </summary>
        [FieldOffset(6)]
        public ushort ButtonData;

        /// <summary> Raw button data. </summary>
        [FieldOffset(8)]
        public uint RawButtons;

        /// <summary> The motion in the X direction. This is signed relative motion or absolute motion, depending on the value of usFlags. </summary>
        [FieldOffset(12)]
        public int LastX;

        /// <summary> The motion in the Y direction. This is signed relative motion or absolute motion, depending on the value of usFlags. </summary>
        [FieldOffset(16)]
        public int LastY;

        /// <summary> The device-specific additional information for the event. </summary>
        [FieldOffset(20)]
        public uint ExtraInformation;
    }
}
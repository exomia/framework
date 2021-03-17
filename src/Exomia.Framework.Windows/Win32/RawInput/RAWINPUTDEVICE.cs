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
    struct RAWINPUTDEVICE
    {
        /// <summary> Top level collection Usage page for the raw input device. </summary>
        public HIDUsagePage UsagePage;

        /// <summary> Top level collection Usage for the raw input device. </summary>
        public HIDUsage Usage;

        /// <summary> Mode flag that specifies how to interpret the information provided by UsagePage and Usage. </summary>
        public RawInputDeviceFlags Flags;

        /// <summary> Handle to the target device. If NULL, it follows the keyboard focus. </summary>
        public IntPtr WindowHandle;
    }
}
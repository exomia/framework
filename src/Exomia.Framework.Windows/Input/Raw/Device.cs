﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using System.Runtime.InteropServices;
using Exomia.Framework.Windows.Win32;
using Exomia.Framework.Windows.Win32.RawInput;

namespace Exomia.Framework.Windows.Input.Raw;

static class Device
{
    public static void RegisterDevice(HIDUsagePage        hidUsagePage,
                                      HIDUsage            hidUsage,
                                      RawInputDeviceFlags rawInputDeviceFlags,
                                      IntPtr              hWndTarget)
    {
        RAWINPUTDEVICE[] rawInputDevices = new RAWINPUTDEVICE[1];
        rawInputDevices[0].UsagePage    = hidUsagePage;
        rawInputDevices[0].Usage        = hidUsage;
        rawInputDevices[0].Flags        = rawInputDeviceFlags;
        rawInputDevices[0].WindowHandle = hWndTarget;

        if (!User32.RegisterRawInputDevices(rawInputDevices, 1, Marshal.SizeOf<RAWINPUTDEVICE>()))
        {
            throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.RegisterRawInputDevices)} failed!");
        }
    }
}
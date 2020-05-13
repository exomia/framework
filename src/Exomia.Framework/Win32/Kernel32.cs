#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Win32
{
    static class Kernel32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", EntryPoint = "SetEvent")]
        internal static extern bool SetEvent(IntPtr hEvent);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", EntryPoint = "GetLastError")]
        internal static extern int GetLastError();

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
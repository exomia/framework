#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Native
{
    /// <summary>
    ///     A kernel 32.
    /// </summary>
    static class Kernel32
    {
        /// <summary>
        ///     Sets an event.
        /// </summary>
        /// <param name="hEvent"> The event. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", EntryPoint = "SetEvent")]
        internal static extern bool SetEvent(IntPtr hEvent);
    }
}
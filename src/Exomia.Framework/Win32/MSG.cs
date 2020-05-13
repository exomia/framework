#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using SharpDX;

namespace Exomia.Framework.Win32
{
    [StructLayout(LayoutKind.Sequential)]
    struct MSG
    {
        public IntPtr hWnd;
        public uint   msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint   time;
        public Point  pt;
    }

    /// <summary>
    ///     A message.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        /// <summary>
        ///     The window handle.
        /// </summary>
        public IntPtr hWnd;

        /// <summary>
        ///     The message.
        /// </summary>
        public uint msg;

        /// <summary>
        ///     The w parameter.
        /// </summary>
        public IntPtr wParam;

        /// <summary>
        ///     The l parameter.
        /// </summary>
        public IntPtr lParam;
    }
}
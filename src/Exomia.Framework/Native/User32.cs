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
using SharpDX;

namespace Exomia.Framework.Native
{
    /// <summary>
    ///     A user 32.
    /// </summary>
    static class User32
    {
        /// <summary>
        ///     Dispatch message.
        /// </summary>
        /// <param name="lpMsg"> [in,out] The message. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int DispatchMessage(ref MSG lpMsg);

        /// <summary>
        ///     Peek message.
        /// </summary>
        /// <param name="lpMsg">         [in,out] The message. </param>
        /// <param name="hWnd">          The window. </param>
        /// <param name="wMsgFilterMin"> The message filter minimum. </param>
        /// <param name="wMsgFilterMax"> The message filter maximum. </param>
        /// <param name="wRemoveMsg">    Message describing the remove. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage(
            out MSG lpMsg,
            IntPtr  hWnd,
            uint    wMsgFilterMin,
            uint    wMsgFilterMax,
            uint    wRemoveMsg);

        /// <summary>
        ///     Translate message.
        /// </summary>
        /// <param name="lpMsg"> [in,out] The message. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int TranslateMessage(ref MSG lpMsg);

        /// <summary>
        ///     A message.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            /// <summary>
            ///     The window.
            /// </summary>
            public IntPtr hWnd;

            /// <summary>
            ///     The message.
            /// </summary>
            public int message;

            /// <summary>
            ///     The parameter.
            /// </summary>
            public IntPtr wParam;

            /// <summary>
            ///     The parameter.
            /// </summary>
            public IntPtr lParam;

            /// <summary>
            ///     The time.
            /// </summary>
            public uint time;

            /// <summary>
            ///     The point.
            /// </summary>
            public Point pt;
        }
    }
}
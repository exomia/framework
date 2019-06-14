#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
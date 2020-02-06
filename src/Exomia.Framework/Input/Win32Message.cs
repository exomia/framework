#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     A window 32 message.
    /// </summary>
    public static class Win32Message
    {
        /// <summary>
        ///     The wm keydown.
        /// </summary>
        public const int WM_KEYDOWN = 0x0100;

        /// <summary>
        ///     The wm keyup.
        /// </summary>
        public const int WM_KEYUP = 0x0101;

        /// <summary>
        ///     The wm syskeydown.
        /// </summary>
        public const int WM_SYSKEYDOWN = 0x0104;

        /// <summary>
        ///     The wm syskeyup.
        /// </summary>
        public const int WM_SYSKEYUP = 0x0105;

        /// <summary>
        ///     The wm character.
        /// </summary>
        public const int WM_CHAR = 0x0102;

        /// <summary>
        ///     The wm unichar.
        /// </summary>
        public const int WM_UNICHAR = 0x0109;

        /// <summary>
        ///     The wm mousemove.
        /// </summary>
        public const int WM_MOUSEMOVE = 0x0200;

        /// <summary>
        ///     The wm mousewheel.
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;

        /// <summary>
        ///     The wm lbuttondown.
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x0201;

        /// <summary>
        ///     The wm lbuttonup.
        /// </summary>
        public const int WM_LBUTTONUP = 0x0202;

        /// <summary>
        ///     The wm mbuttondown.
        /// </summary>
        public const int WM_MBUTTONDOWN = 0x0207;

        /// <summary>
        ///     The wm mbuttonup.
        /// </summary>
        public const int WM_MBUTTONUP = 0x0208;

        /// <summary>
        ///     The wm rbuttondown.
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x0204;

        /// <summary>
        ///     The wm rbuttonup.
        /// </summary>
        public const int WM_RBUTTONUP = 0x0205;

        /// <summary>
        ///     The wm xbuttondown.
        /// </summary>
        public const int WM_XBUTTONDOWN = 0x020B;

        /// <summary>
        ///     The wm xbuttonup.
        /// </summary>
        public const int WM_XBUTTONUP = 0x020C;

        /// <summary>
        ///     The wm lbuttondblclk.
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x0203;

        /// <summary>
        ///     The wm mbuttondblclk.
        /// </summary>
        public const int WM_MBUTTONDBLCLK = 0x0209;

        /// <summary>
        ///     The wm rbuttondblclk.
        /// </summary>
        public const int WM_RBUTTONDBLCLK = 0x0206;

        /// <summary>
        ///     The wm xbuttondblclk.
        /// </summary>
        public const int WM_XBUTTONDBLCLK = 0x020D;
    }
}
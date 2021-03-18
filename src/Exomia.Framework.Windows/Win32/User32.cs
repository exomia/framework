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
using Exomia.Framework.Windows.Win32.RawInput;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32
{
    static class User32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "GetRawInputData", SetLastError = false)]
        internal static extern unsafe uint GetRawInputData(IntPtr  hRawInput,
                                                           uint    riCmd,
                                                           byte*   pData,
                                                           ref int pcbSize,
                                                           int     cbSizeHeader);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "DispatchMessage", SetLastError = true)]
        internal static extern int DispatchMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "PeekMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage(out MSG lpMsg,
                                                IntPtr  hWnd,
                                                uint    wMsgFilterMin,
                                                uint    wMsgFilterMax,
                                                uint    wRemoveMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "TranslateMessage", SetLastError = true)]
        internal static extern int TranslateMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "ClientToScreen", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "ClipCursor", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClipCursor(ref RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "CreateWindowEx", SetLastError = true)]
        internal static extern IntPtr CreateWindowEx(uint                                    dwExStyle,
                                                     [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
                                                     [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
                                                     uint                                    dwStyle,
                                                     int                                     x,
                                                     int                                     y,
                                                     int                                     nWidth,
                                                     int                                     nHeight,
                                                     IntPtr                                  hWndParent,
                                                     IntPtr                                  hMenu,
                                                     IntPtr                                  hInstance,
                                                     IntPtr                                  lpParam);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "RegisterClassEx", SetLastError = false)]
        internal static extern ushort RegisterClassEx([In] ref WndClassEx lpWndClass);

        [DllImport(ExternDll.User32, EntryPoint = "UnregisterClass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "DefWindowProc", SetLastError = false)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "PostQuitMessage", SetLastError = false)]
        internal static extern void PostQuitMessage(int nExitCode);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "LoadCursor", SetLastError = false)]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "UpdateWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "ShowWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "DestroyWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr            hWnd,
                                                 IntPtr            hWndInsertAfter,
                                                 int               x,
                                                 int               y,
                                                 int               cx,
                                                 int               cy,
                                                 SetWindowPosFlags uFlags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustWindowRectEx(ref RECT lpRect,
                                                       uint     dwStyle,
                                                       bool     bMenu,
                                                       uint     dwExStyle);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "SetFocus", SetLastError = true)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "SetWindowText", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "GetClientRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "GetSystemMetrics")]
        internal static extern int GetSystemMetrics(int smIndex);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "TrackMouseEvent", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "ShowCursor")]
        internal static extern int ShowCursor(bool bShow);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(ExternDll.User32, EntryPoint = "RegisterRawInputDevices", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]
                                                            RAWINPUTDEVICE[] pRawInputDevices,
                                                            int uiNumDevices,
                                                            int cbSize);
    }
}
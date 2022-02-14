#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using System.Security;
using Exomia.Framework.Windows.Win32.RawInput;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32
{
    internal static class User32
    {
        private const string USER32 = "user32.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "GetRawInputData", SetLastError = false)]
        internal static extern unsafe uint GetRawInputData(IntPtr  hRawInput,
                                                           uint    riCmd,
                                                           byte*   pData,
                                                           ref int pcbSize,
                                                           int     cbSizeHeader);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "DispatchMessage", SetLastError = true)]
        internal static extern int DispatchMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "PeekMessage", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool PeekMessage(out MSG lpMsg,
                                                IntPtr  hWnd,
                                                uint    wMsgFilterMin,
                                                uint    wMsgFilterMax,
                                                uint    wRemoveMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "TranslateMessage", SetLastError = true)]
        internal static extern int TranslateMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "ClientToScreen", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "ClipCursor", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ClipCursor(ref RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "CreateWindowEx", SetLastError = true)]
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
        [DllImport(USER32, EntryPoint = "RegisterClassEx", SetLastError = false)]
        internal static extern ushort RegisterClassEx([In] ref WndClassEx lpWndClass);

        [DllImport(USER32, EntryPoint = "UnregisterClass")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UnregisterClass(string lpClassName, IntPtr hInstance);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "DefWindowProc", SetLastError = false)]
        internal static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "PostQuitMessage", SetLastError = false)]
        internal static extern void PostQuitMessage(int nExitCode);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "LoadCursor", SetLastError = false)]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "UpdateWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "ShowWindow", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "DestroyWindow", SetLastError = false)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "SetWindowPos", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowPos(IntPtr            hWnd,
                                                 IntPtr            hWndInsertAfter,
                                                 int               x,
                                                 int               y,
                                                 int               cx,
                                                 int               cy,
                                                 SetWindowPosFlags uFlags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "AdjustWindowRectEx", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AdjustWindowRectEx(ref RECT lpRect,
                                                       uint     dwStyle,
                                                       bool     bMenu,
                                                       uint     dwExStyle);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "SetForegroundWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "SetFocus", SetLastError = true)]
        internal static extern IntPtr SetFocus(IntPtr hWnd);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "SetWindowText", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetWindowText(IntPtr hWnd, string lpString);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "SetWindowLongPtr")]
        internal static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "GetClientRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "GetSystemMetrics")]
        internal static extern int GetSystemMetrics(int smIndex);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "GetWindowRect")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "TrackMouseEvent", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "ShowCursor")]
        internal static extern int ShowCursor(bool bShow);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "RegisterRawInputDevices", SetLastError = true)]
        [return: MarshalAs(                                            UnmanagedType.Bool)]
        internal static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)] RAWINPUTDEVICE[] pRawInputDevices,
                                                            int                                                                     uiNumDevices,
                                                            int                                                                     cbSize);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(USER32, EntryPoint = "MonitorFromWindow", SetLastError = true)]
        internal static extern IntPtr MonitorFromWindow(IntPtr hwnd, MonitorFlags dwFlags);
    }
}
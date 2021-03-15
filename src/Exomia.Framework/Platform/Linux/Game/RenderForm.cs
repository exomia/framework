#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Exomia.Framework.Input;
using Exomia.Framework.Mathematics;
using Exomia.Framework.Platform.Windows.Input.Raw;
using Exomia.Framework.Platform.Windows.Win32;
using Exomia.Framework.Platform.Windows.Win32.RawInput;

namespace Exomia.Framework.Platform.Linux.Game
{
    /// <summary>
    ///     The RenderForm.
    /// </summary>
    public sealed partial class RenderForm : IDisposable
    {
        private const string LP_CLASS_NAME = "Exomia.Framework.RenderForm";

        private const uint COLOR_WINDOW = 5;
        private const int  IDC_ARROW    = 32512;

        private static readonly int s_size_of_rawinputheader = Marshal.SizeOf<RAWINPUTHEADER>();

        private readonly WndClassEx _wndClassEx;

        private int         _state;
        private KeyModifier _keyModifier = 0;
        private IntPtr      _hWnd;

        private string          _windowTitle;
        private VectorI2        _size;
        private FormWindowState _windowState = FormWindowState.Normal;
        private FormBorderStyle _borderStyle = FormBorderStyle.Fixed;

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        /// <value>
        ///     The size.
        /// </value>
        public VectorI2 Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    Resize(value.X, value.Y);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the window title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                if (_hWnd != IntPtr.Zero)
                {
                    User32.SetWindowText(_hWnd, _windowTitle);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the state of the window.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <exception cref="Win32Exception">              Thrown when a Window 32 error condition occurs. </exception>
        /// <value>
        ///     The window state.
        /// </value>
        public FormWindowState WindowState
        {
            get { return _windowState; }
            set
            {
                _windowState = value;
                if (_hWnd != IntPtr.Zero)
                {
                    ShowWindowCommands showWindowCommands = _windowState switch
                    {
                        FormWindowState.Normal => ShowWindowCommands.Normal,
                        FormWindowState.Minimized => ShowWindowCommands.Minimize,
                        FormWindowState.Maximized => ShowWindowCommands.Maximize,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    if (!User32.ShowWindow(_hWnd, (int)showWindowCommands))
                    {
                        throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.ShowWindow)} failed!");
                    }
                }
            }
        }

        /// <summary>
        ///     Gets or sets the state of the window.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <exception cref="Win32Exception">              Thrown when a Window 32 error condition occurs. </exception>
        /// <value>
        ///     The window state.
        /// </value>
        public FormBorderStyle BorderStyle
        {
            get { return _borderStyle; }
            set
            {
                _borderStyle = value;
                if (_hWnd != IntPtr.Zero)
                {
                    uint windowStyles = _borderStyle switch
                    {
                        FormBorderStyle.None => WS.OVERLAPPED,
                        FormBorderStyle.Fixed => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                                 WS.MAXIMIZEBOX,
                        FormBorderStyle.Sizable => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                                   WS.MAXIMIZEBOX | WS.SIZEFRAME,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    windowStyles |= _windowState switch
                    {
                        FormWindowState.Normal => 0,
                        FormWindowState.Minimized => WS.MINIMIZE,
                        FormWindowState.Maximized => WS.MAXIMIZE,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    uint windowStylesEx = WSEX.LEFT | WSEX.LTRREADING | WSEX.WINDOWEDGE | WSEX.APPWINDOW;
                    if (_borderStyle == FormBorderStyle.None)
                    {
                        windowStylesEx &= ~WSEX.WINDOWEDGE;
                    }

                    User32.SetWindowLongPtr(_hWnd, WLF.GWL_STYLE, (IntPtr)windowStyles);
                    User32.SetWindowLongPtr(_hWnd, WLF.GWL_EXSTYLE, (IntPtr)windowStylesEx);

                    if (!User32.SetWindowPos(
                        _hWnd,
                        IntPtr.Zero,
                        0, 0, 0, 0,
                        SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove |
                        SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.IgnoreResize |
                        SetWindowPosFlags.FrameChanged))
                    {
                        throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
                    }
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RenderForm" /> class.
        /// </summary>
        /// <param name="windowTitle"> (Optional) The window title. </param>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        public RenderForm(string windowTitle = "RenderForm")
        {
            _windowTitle       = windowTitle;
            _rawKeyPipe        = new Pipe<RawKeyEventHandler>();
            _keyUpPipe         = new Pipe<KeyEventHandler>();
            _keyDownPipe       = new Pipe<KeyEventHandler>();
            _keyPressPipe      = new Pipe<KeyPressEventHandler>();
            _mouseMovePipe     = new Pipe<MouseEventHandler>();
            _mouseUpPipe       = new Pipe<MouseEventHandler>();
            _mouseDownPipe     = new Pipe<MouseEventHandler>();
            _mouseClickPipe    = new Pipe<MouseEventHandler>();
            _mouseWheelPipe    = new Pipe<MouseEventHandler>();
            _mouseRawInputPipe = new Pipe<MouseEventHandler>();

            _wndClassEx = new WndClassEx
            {
                cbSize = Marshal.SizeOf(typeof(WndClassEx)),
                style = ClassStyles.HorizontalRedraw | ClassStyles.VerticalRedraw |
                        ClassStyles.DoubleClicks | ClassStyles.OwnDC,
                hbrBackground = (IntPtr)COLOR_WINDOW + 1, //null,
                cbClsExtra    = 0,
                cbWndExtra    = 0,
                hInstance     = Kernel32.GetModuleHandle(null!),
                hIcon         = Shell32.ExtractIcon(IntPtr.Zero, Assembly.GetExecutingAssembly().Location, 0),
                hCursor       = User32.LoadCursor(IntPtr.Zero, IDC_ARROW),
                lpszMenuName  = null!,
                lpszClassName = LP_CLASS_NAME,
                lpfnWndProc   = WndProc,
                hIconSm       = IntPtr.Zero
            };

            ushort regResult = User32.RegisterClassEx(ref _wndClassEx);
            if (regResult == 0)
            {
                throw new Win32Exception(
                    Kernel32.GetLastError(), $"{nameof(User32.RegisterClassEx)} failed with code {regResult}!");
            }
        }

        /// <summary>
        ///     Shows the window.
        /// </summary>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        public void Show()
        {
            User32.ShowWindow(_hWnd, (int)ShowWindowCommands.Normal);

            if (!User32.UpdateWindow(_hWnd))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.UpdateWindow)} failed!");
            }
            if (!User32.SetForegroundWindow(_hWnd))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetForegroundWindow)} failed!");
            }
            User32.SetFocus(_hWnd);
        }

        /// <summary>
        ///     Resizes the window.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        public void Resize(int width, int height)
        {
            RECT windowRect;
            windowRect.LeftTop.X     = 0;
            windowRect.LeftTop.Y     = 0;
            windowRect.RightBottom.X = width;
            windowRect.RightBottom.Y = height;

            uint windowStyles = _borderStyle switch
            {
                FormBorderStyle.None => 0,
                FormBorderStyle.Fixed => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                         WS.MAXIMIZEBOX,
                FormBorderStyle.Sizable => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                           WS.MAXIMIZEBOX | WS.SIZEFRAME,
                _ => throw new ArgumentOutOfRangeException()
            };

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            windowStyles |= _windowState switch
            {
                FormWindowState.Normal => 0,
                FormWindowState.Maximized => WS.MAXIMIZE,
                _ => throw new ArgumentOutOfRangeException()
            };

            uint windowStylesEx = WSEX.LEFT | WSEX.LTRREADING | WSEX.WINDOWEDGE | WSEX.APPWINDOW;
            if (_borderStyle == FormBorderStyle.None)
            {
                windowStylesEx &= ~WSEX.WINDOWEDGE;
            }

            if (!User32.AdjustWindowRectEx(ref windowRect, windowStyles, false, windowStylesEx))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.AdjustWindowRectEx)} failed!");
            }

            if (!User32.SetWindowPos(
                _hWnd,
                IntPtr.Zero,
                0, 0,
                windowRect.RightBottom.X - windowRect.LeftTop.X,
                windowRect.RightBottom.Y - windowRect.LeftTop.Y,
                SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove |
                SetWindowPosFlags.IgnoreZOrder))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
            }
        }

        /// <summary>
        ///     Creates a window.
        /// </summary>
        /// <param name="w">            The width. </param>
        /// <param name="h">            The height. </param>
        /// <returns>
        ///     The new window.
        /// </returns>
        /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
        internal IntPtr CreateWindow(int w, int h)
        {
            RECT windowRect;
            windowRect.LeftTop.X     = 0;
            windowRect.LeftTop.Y     = 0;
            windowRect.RightBottom.X = w;
            windowRect.RightBottom.Y = h;

            uint windowStyles = _borderStyle switch
            {
                FormBorderStyle.None => WS.POPUP,
                FormBorderStyle.Fixed => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                         WS.MAXIMIZEBOX,
                FormBorderStyle.Sizable => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                                           WS.MAXIMIZEBOX | WS.SIZEFRAME,
                _ => throw new ArgumentOutOfRangeException()
            };

            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            windowStyles |= _windowState switch
            {
                FormWindowState.Normal => 0,
                FormWindowState.Maximized => WS.MAXIMIZE,
                _ => throw new ArgumentOutOfRangeException()
            };
            uint windowStylesEx = WSEX.LEFT | WSEX.LTRREADING | WSEX.WINDOWEDGE | WSEX.APPWINDOW;
            if (_borderStyle == FormBorderStyle.None)
            {
                windowStylesEx &= ~WSEX.WINDOWEDGE;
            }

            if (!User32.AdjustWindowRectEx(ref windowRect, windowStyles, false, windowStylesEx))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.AdjustWindowRectEx)} failed!");
            }

            const int CW_USEDEFAULT = unchecked((int)0x80000000);
            if ((_hWnd =
                User32.CreateWindowEx(
                    windowStylesEx,
                    _wndClassEx.lpszClassName,
                    WindowTitle,
                    windowStyles,
                    CW_USEDEFAULT,
                    0,
                    windowRect.RightBottom.X - windowRect.LeftTop.X,
                    windowRect.RightBottom.Y - windowRect.LeftTop.Y,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    _wndClassEx.hInstance,
                    IntPtr.Zero)) == IntPtr.Zero)
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.CreateWindowEx)} failed!");
            }

            if (_borderStyle == FormBorderStyle.None)
            {
                User32.SetWindowLongPtr(_hWnd, WLF.GWL_STYLE, (IntPtr)0);
                if (!User32.SetWindowPos(
                    _hWnd,
                    IntPtr.Zero,
                    0, 0, 0, 0,
                    SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove |
                    SetWindowPosFlags.IgnoreZOrder | SetWindowPosFlags.IgnoreResize |
                    SetWindowPosFlags.FrameChanged))
                {
                    throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
                }
            }

            Device.RegisterDevice(HIDUsagePage.Generic, HIDUsage.Mouse, RawInputDeviceFlags.None, _hWnd);

            return _hWnd;
        }

        #region IDisposable Support

        private bool _disposed;

#pragma warning disable IDE0060

        // ReSharper disable once UnusedParameter.Local
        private void Dispose(bool disposing)
#pragma warning restore IDE0060
        {
            if (!_disposed)
            {
                if (_hWnd != IntPtr.Zero)
                {
                    User32.DestroyWindow(_hWnd);
                }

                User32.UnregisterClass(_wndClassEx.lpszClassName, _wndClassEx.hInstance);

                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~RenderForm()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
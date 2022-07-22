#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Windows.Input;
using Exomia.Framework.Windows.Input.Raw;
using Exomia.Framework.Windows.Win32;
using Exomia.Framework.Windows.Win32.RawInput;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.Windows.Game.Desktop;

/// <summary> The RenderForm. </summary>
internal sealed partial class RenderForm : IWin32RenderForm
{
    private const string LP_CLASS_NAME = "Exomia.Framework.RenderForm";

    private const uint COLOR_WINDOW = 5;
    private const int  IDC_ARROW    = 32512;

    private static readonly int s_sizeOfRawInputHeader = Marshal.SizeOf<RAWINPUTHEADER>();

    private readonly WndClassEx _wndClassEx;
    private readonly bool       _clipCursor;
    private readonly bool       _isMouseVisible;

    private int         _state;
    private KeyModifier _keyModifier = 0;

    private string          _windowTitle;
    private FormWindowState _windowState;
    private FormBorderStyle _borderStyle;

    /// <inheritdoc />
    public int Width { get; private set; }

    /// <inheritdoc />
    public int Height { get; private set; }

    /// <inheritdoc />
    public string Title
    {
        get { return _windowTitle; }
        set
        {
            _windowTitle = value;
            if (HWnd != IntPtr.Zero)
            {
                User32.SetWindowText(HWnd, _windowTitle);
            }
        }
    }

    /// <inheritdoc />
    public FormWindowState WindowState
    {
        get { return _windowState; }
        set
        {
            _windowState = value;
            if (HWnd != IntPtr.Zero)
            {
                ShowWindowCommands showWindowCommands = _windowState switch
                {
                    FormWindowState.Normal    => ShowWindowCommands.Normal,
                    FormWindowState.Minimized => ShowWindowCommands.Minimize,
                    FormWindowState.Maximized => ShowWindowCommands.Maximize,
                    _                         => throw new ArgumentOutOfRangeException()
                };

                if (!User32.ShowWindow(HWnd, (int)showWindowCommands))
                {
                    throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.ShowWindow)} failed!");
                }
            }
        }
    }

    /// <inheritdoc />
    public FormBorderStyle BorderStyle
    {
        get { return _borderStyle; }
        set
        {
            _borderStyle = value;
            if (HWnd != IntPtr.Zero)
            {
                uint windowStyles = _borderStyle switch
                {
                    FormBorderStyle.None => WS.OVERLAPPED,
                    FormBorderStyle.Fixed => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                        WS.MAXIMIZEBOX,
                    FormBorderStyle.Sizable => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                        WS.MAXIMIZEBOX                    | WS.SIZEFRAME,
                    _ => throw new ArgumentOutOfRangeException()
                };

                windowStyles |= _windowState switch
                {
                    FormWindowState.Normal    => 0,
                    FormWindowState.Minimized => WS.MINIMIZE,
                    FormWindowState.Maximized => WS.MAXIMIZE,
                    _                         => throw new ArgumentOutOfRangeException()
                };

                uint windowStylesEx = WSEX.LEFT | WSEX.LTRREADING | WSEX.WINDOWEDGE | WSEX.APPWINDOW;
                if (_borderStyle == FormBorderStyle.None)
                {
                    windowStylesEx &= ~WSEX.WINDOWEDGE;
                }

                User32.SetWindowLongPtr(HWnd, WLF.GWL_STYLE,   (IntPtr)windowStyles);
                User32.SetWindowLongPtr(HWnd, WLF.GWL_EXSTYLE, (IntPtr)windowStylesEx);

                if (!User32.SetWindowPos(
                        HWnd,
                        IntPtr.Zero,
                        0, 0, 0, 0,
                        SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove   |
                        SetWindowPosFlags.IgnoreZOrder  | SetWindowPosFlags.IgnoreResize |
                        SetWindowPosFlags.FrameChanged))
                {
                    throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
                }
            }
        }
    }

    internal IntPtr HWnd { get; private set; }

    /// <summary> Initializes a new instance of the <see cref="RenderForm" /> class. </summary>
    /// <param name="configuration"> The configuration. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="Win32Exception">        Thrown when a Window 32 error condition occurs. </exception>
    public RenderForm(IOptions<RenderFormConfiguration> configuration)
    {
        if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }

        _windowTitle = configuration.Value.Title;
        _windowState = configuration.Value.DisplayType == DisplayType.FullscreenWindow
            ? FormWindowState.Maximized
            : FormWindowState.Normal;
        _borderStyle = configuration.Value.DisplayType == DisplayType.Window
            ? FormBorderStyle.Fixed
            : FormBorderStyle.None;
        _clipCursor     = configuration.Value.ClipCursor;
        _isMouseVisible = configuration.Value.IsMouseVisible;

        Width  = (int)configuration.Value.Width;
        Height = (int)configuration.Value.Height;

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
                ClassStyles.DoubleClicks         | ClassStyles.OwnDC,
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

    /// <summary> Shows the window. </summary>
    /// <exception cref="Win32Exception"> Thrown when a Window 32 error condition occurs. </exception>
    public void Show()
    {
        User32.ShowWindow(HWnd, (int)ShowWindowCommands.Show);

        if (!User32.UpdateWindow(HWnd))
        {
            throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.UpdateWindow)} failed!");
        }
        if (!User32.SetForegroundWindow(HWnd))
        {
            throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetForegroundWindow)} failed!");
        }
        User32.SetFocus(HWnd);

        if (_clipCursor)
        {
            RECT rect = new(Width, Height);
            if (User32.ClientToScreen(HWnd, ref rect.LeftTop) &&
                User32.ClientToScreen(HWnd, ref rect.RightBottom))
            {
                User32.ClipCursor(ref rect);
            }
        }
    }

    /// <summary> Resizes the window. </summary>
    /// <param name="width">  The width. </param>
    /// <param name="height"> The height. </param>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <exception cref="Win32Exception">              Thrown when a Window 32 error condition occurs. </exception>
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
                WS.MAXIMIZEBOX                    | WS.SIZEFRAME,
            _ => throw new ArgumentOutOfRangeException()
        };

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        windowStyles |= _windowState switch
        {
            FormWindowState.Normal    => 0,
            FormWindowState.Maximized => WS.MAXIMIZE,
            _                         => throw new ArgumentOutOfRangeException()
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
                HWnd,
                IntPtr.Zero,
                0, 0,
                windowRect.RightBottom.X - windowRect.LeftTop.X,
                windowRect.RightBottom.Y - windowRect.LeftTop.Y,
                SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreZOrder))
        {
            throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
        }
    }

    /// <summary> Creates the window. </summary>
    /// <returns> The new window. </returns>
    /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
    /// <exception cref="Win32Exception">              Thrown when a Window 32 error condition occurs. </exception>
    public (IntPtr, IntPtr) CreateWindow()
    {
        if (HWnd != IntPtr.Zero) { return (_wndClassEx.hInstance, HWnd); }

        RECT windowRect;
        windowRect.LeftTop.X     = 0;
        windowRect.LeftTop.Y     = 0;
        windowRect.RightBottom.X = Width;
        windowRect.RightBottom.Y = Height;

        uint windowStyles = _borderStyle switch
        {
            FormBorderStyle.None => WS.POPUP,
            FormBorderStyle.Fixed => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                WS.MAXIMIZEBOX,
            FormBorderStyle.Sizable => WS.CAPTION | WS.SYSMENU | WS.OVERLAPPED | WS.MINIMIZEBOX |
                WS.MAXIMIZEBOX                    | WS.SIZEFRAME,
            _ => throw new ArgumentOutOfRangeException()
        };

        // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
        windowStyles |= _windowState switch
        {
            FormWindowState.Normal    => 0,
            FormWindowState.Maximized => WS.MAXIMIZE,
            _                         => throw new ArgumentOutOfRangeException(nameof(_windowState))
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

        // ReSharper disable once InconsistentNaming
        const int CW_USEDEFAULT = unchecked((int)0x80000000);
        if ((HWnd =
                User32.CreateWindowEx(
                    windowStylesEx,
                    _wndClassEx.lpszClassName,
                    Title,
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
            User32.SetWindowLongPtr(HWnd, WLF.GWL_STYLE, (IntPtr)0);
            if (!User32.SetWindowPos(
                    HWnd,
                    IntPtr.Zero,
                    0, 0, 0, 0,
                    SetWindowPosFlags.DoNotActivate | SetWindowPosFlags.IgnoreMove   |
                    SetWindowPosFlags.IgnoreZOrder  | SetWindowPosFlags.IgnoreResize |
                    SetWindowPosFlags.FrameChanged))
            {
                throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.SetWindowPos)} failed!");
            }
        }

        //TODO: Fullscreen & FullscreenWindow
        //if (_configuration.DisplayType == DisplayType.FullscreenWindow) { }
        //if (_configuration.DisplayType == DisplayType.Fullscreen) { }

        if (!User32.GetClientRect(HWnd, out RECT rcRect))
        {
            throw new Win32Exception(Kernel32.GetLastError(), $"{nameof(User32.GetClientRect)} failed!");
        }

        Width  = (rcRect.RightBottom.X - rcRect.LeftTop.X);
        Height = (rcRect.RightBottom.Y - rcRect.LeftTop.Y);

        Device.RegisterDevice(HIDUsagePage.Generic, HIDUsage.Mouse, RawInputDeviceFlags.None, HWnd);
        return (_wndClassEx.hInstance, HWnd);
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
            if (HWnd != IntPtr.Zero)
            {
                User32.DestroyWindow(HWnd);
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
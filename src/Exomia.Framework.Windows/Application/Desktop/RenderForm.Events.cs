#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#nullable enable

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Exomia.Framework.Core;
using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Windows.Win32;
using Exomia.Framework.Windows.Win32.RawInput;
using EventHandler = Exomia.Framework.Core.EventHandler;

namespace Exomia.Framework.Windows.Application.Desktop;

sealed partial class RenderForm
{
    private const uint RID_ERROR               = unchecked((uint)-1);
    private const int  RID_INPUT               = 0x10000003;
    private const int  RID_HEADER              = 0x10000005;
    private const int  RID_INPUT_TYPE_MOUSE    = 0;
    private const int  RID_INPUT_TYPE_KEYBOARD = 1;
    private const int  RID_INPUT_TYPE_HID      = 2;
    private const int  RID_INPUT_TYPE_OTHER    = 3;

    private const int MOUSE_LE_STATE = 1;

    /// <summary> Occurs when the mouse leaves the client area. </summary>
    public event EventHandler<IWin32RenderForm, IntPtr>? MouseLeave;

    /// <summary> Occurs when the mouse enters the client area. </summary>
    public event EventHandler<IWin32RenderForm, IntPtr>? MouseEnter;

    /// <summary> Occurs when the window is about to close. </summary>
    public event RefEventHandler<bool>? Closing;

    /// <summary> Occurs when the window is closed. </summary>
    public event EventHandler? Closed;

    /// <summary> Occurs when the window was resized. </summary>
    public event Core.EventHandler<IRenderForm>? Resized;

    private unsafe IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {
        Message m;
        m.hWnd   = hWnd;
        m.msg    = msg;
        m.wParam = wParam;
        m.lParam = lParam;

        switch (msg)
        {
            case WM.INPUT:
            {
                int sizeOfRawInputData = 0;
                User32.GetRawInputData(
                    lParam, RID_INPUT, null, ref sizeOfRawInputData, s_sizeOfRawInputHeader);

                if (sizeOfRawInputData == 0) { return IntPtr.Zero; }

                byte* rawInputDataPtr = stackalloc byte[sizeOfRawInputData];
                if (User32.GetRawInputData(
                        lParam, RID_INPUT, rawInputDataPtr, ref sizeOfRawInputData, s_sizeOfRawInputHeader) !=
                    RID_ERROR)
                {
                    RAWINPUT* rawInput = (RAWINPUT*)rawInputDataPtr;
                    switch (rawInput->Header.Type)
                    {
                        case RID_INPUT_TYPE_MOUSE:
                            RawMouseInput(in rawInput->Data.Mouse);
                            break;

                        // not supported/needed atm.
                        case RID_INPUT_TYPE_KEYBOARD:
                        case RID_INPUT_TYPE_HID:
                        case RID_INPUT_TYPE_OTHER:
                            break;
                    }
                }
                return IntPtr.Zero;
            }
            case WM.MOUSELEAVE:
            {
                _state &= ~MOUSE_LE_STATE;

                if (!_isMouseVisible)
                {
                    // ReSharper disable once EmptyEmbeddedStatement
                    while (User32.ShowCursor(true) < 0) ;
                }

                MouseLeave?.Invoke(this, HWnd);
                return IntPtr.Zero;
            }
            case WM.SIZE:
            {
                int width  = LowWord(lParam);
                int height = HighWord(lParam);
                if (width != Width || height != Height)
                {
                    Width  = width;
                    Height = height;
                    Resized?.Invoke(this);
                }
                return IntPtr.Zero;
            }
            case WM.CLOSE:
            {
                Task.Run(() =>
                {
                    bool cancel = false;
                    Closing?.Invoke(ref cancel);
                    if (!cancel)
                    {
                        Closed?.Invoke();
                        User32.DestroyWindow(HWnd);
                    }
                }).ConfigureAwait(false);
                return IntPtr.Zero;
            }
            case WM.DESTROY:
            {
                MouseLeave?.Invoke(this, HWnd);
                User32.PostQuitMessage(0);
                return IntPtr.Zero;
            }
            case WM.KEYDOWN:
            case WM.KEYUP:
            case WM.CHAR:
            case WM.UNICHAR:
            case WM.SYSKEYDOWN:
            case WM.SYSKEYUP:
            {
                RawKeyMessage(ref m);
                return IntPtr.Zero;
            }
            case WM.LBUTTONDOWN:
            {
                RawMouseDown(ref m, MouseButtons.Left);
                return IntPtr.Zero;
            }
            case WM.MBUTTONDOWN:
            {
                RawMouseDown(ref m, MouseButtons.Middle);
                return IntPtr.Zero;
            }
            case WM.RBUTTONDOWN:
            {
                RawMouseDown(ref m, MouseButtons.Right);
                return IntPtr.Zero;
            }
            case WM.XBUTTONDOWN:
            {
                RawMouseDown(
                    ref m, HighWord(m.wParam) == 1
                        ? MouseButtons.XButton1
                        : MouseButtons.XButton2);
                return IntPtr.Zero;
            }
            case WM.LBUTTONUP:
            {
                RawMouseUp(ref m, MouseButtons.Left);
                return IntPtr.Zero;
            }
            case WM.MBUTTONUP:
            {
                RawMouseUp(ref m, MouseButtons.Middle);
                return IntPtr.Zero;
            }
            case WM.RBUTTONUP:
            {
                RawMouseUp(ref m, MouseButtons.Right);
                return IntPtr.Zero;
            }
            case WM.XBUTTONUP:
            {
                RawMouseUp(
                    ref m, HighWord(m.wParam) == 1
                        ? MouseButtons.XButton1
                        : MouseButtons.XButton2);
                return IntPtr.Zero;
            }
            case WM.MOUSEMOVE:
            {
                if ((_state & MOUSE_LE_STATE) != MOUSE_LE_STATE)
                {
                    _state |= MOUSE_LE_STATE;

                    if (!_isMouseVisible)
                    {
                        // ReSharper disable once EmptyEmbeddedStatement
                        while (User32.ShowCursor(false) >= 0) ;
                    }
                    if (_clipCursor)
                    {
                        RECT rect = new RECT(Width, Height);
                        if (User32.ClientToScreen(hWnd, ref rect.LeftTop) &&
                            User32.ClientToScreen(hWnd, ref rect.RightBottom))
                        {
                            User32.ClipCursor(ref rect);
                        }
                    }

                    MouseEnter?.Invoke(this, HWnd);

                    TRACKMOUSEEVENT trackMouseEvent = new TRACKMOUSEEVENT(TME.LEAVE, HWnd, 0);
                    if (!User32.TrackMouseEvent(ref trackMouseEvent))
                    {
                        throw new Win32Exception(
                            Kernel32.GetLastError(), $"{nameof(User32.TrackMouseEvent)} failed!");
                    }
                }

                MouseEventArgs args = new MouseEventArgs(
                    LowWord(m.lParam), HighWord(m.lParam), (MouseButtons)LowWord(m.wParam), 0, 0);
                for (int i = 0; i < _mouseMovePipe.Count; i++)
                {
                    if (_mouseMovePipe[i].Invoke(args) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }

                return IntPtr.Zero;
            }
            case WM.MOUSEWHEEL:
            {
                MouseEventArgs args = new MouseEventArgs(
                    LowWord(m.lParam), HighWord(m.lParam), (MouseButtons)LowWord(m.wParam), 0,
                    HighWord(m.wParam));
                for (int i = 0; i < _mouseWheelPipe.Count; i++)
                {
                    if (_mouseWheelPipe[i].Invoke(args) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }
                return IntPtr.Zero;
            }
            case WM.LBUTTONDBLCLK:
            case WM.MBUTTONDBLCLK:
            case WM.RBUTTONDBLCLK:
            case WM.XBUTTONDBLCLK:
            {
                _state |= 0xC000000;
                MouseEventArgs args = new MouseEventArgs(
                    LowWord(m.lParam), HighWord(m.lParam), (MouseButtons)LowWord(m.wParam), 2, 0);
                for (int i = 0; i < _mouseClickPipe.Count; i++)
                {
                    if (_mouseClickPipe[i].Invoke(args) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }
                return IntPtr.Zero;
            }
        }
        return User32.DefWindowProc(hWnd, msg, wParam, lParam);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RawKeyMessage(ref Message m)
    {
        for (int i = 0; i < _rawKeyPipe.Count; i++)
        {
            if (_rawKeyPipe[i].Invoke(m) == EventAction.StopPropagation)
            {
                break;
            }
        }

        int vKey = (int)m.wParam.ToInt64();

        switch (m.msg)
        {
            case WM.SYSKEYDOWN:
            case WM.KEYDOWN:
            {
                switch (vKey)
                {
                    case Key.ShiftKey:
                        _keyModifier |= KeyModifier.Shift;
                        break;
                    case Key.ControlKey:
                        _keyModifier |= KeyModifier.Control;
                        break;
                    case Key.Menu:
                        _keyModifier |= KeyModifier.Alt;
                        break;
                }
                for (int i = 0; i < _keyDownPipe.Count; i++)
                {
                    if (_keyDownPipe[i].Invoke(vKey, _keyModifier) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }
                break;
            }
            case WM.SYSKEYUP:
            case WM.KEYUP:
            {
                switch (vKey)
                {
                    case Key.ShiftKey:
                        _keyModifier &= ~KeyModifier.Shift;
                        break;
                    case Key.ControlKey:
                        _keyModifier &= ~KeyModifier.Control;
                        break;
                    case Key.Menu:
                        _keyModifier &= ~KeyModifier.Alt;
                        break;
                }
                for (int i = 0; i < _keyUpPipe.Count; i++)
                {
                    if (_keyUpPipe[i].Invoke(vKey, _keyModifier) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }
                break;
            }
            case WM.UNICHAR:
            case WM.CHAR:
            {
                for (int i = 0; i < _keyPressPipe.Count; i++)
                {
                    if (_keyPressPipe[i].Invoke((char)vKey) == EventAction.StopPropagation)
                    {
                        break;
                    }
                }
                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RawMouseDown(ref Message m, MouseButtons buttons)
    {
        _state |= 0x8000000;

        MouseEventArgs args = new MouseEventArgs(
            LowWord(m.lParam), HighWord(m.lParam), buttons, 1, 0);
        for (int i = 0; i < _mouseDownPipe.Count; i++)
        {
            if (_mouseDownPipe[i].Invoke(args) == EventAction.StopPropagation)
            {
                break;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RawMouseUp(ref Message m, MouseButtons buttons)
    {
        int x = LowWord(m.lParam);
        int y = HighWord(m.lParam);

        if ((_state & 0x8000000) == 0x8000000)
        {
            MouseEventArgs argsClick = new MouseEventArgs(
                x, y, buttons, (_state & 0x4000000) == 0x4000000
                    ? 2
                    : 1, 0);
            for (int i = 0; i < _mouseClickPipe.Count; i++)
            {
                if (_mouseClickPipe[i].Invoke(argsClick) == EventAction.StopPropagation)
                {
                    break;
                }
            }
        }

        _state &= ~0xC000000;

        MouseEventArgs argsUp = new MouseEventArgs(x, y, buttons, 1, 0);
        for (int i = 0; i < _mouseUpPipe.Count; i++)
        {
            if (_mouseUpPipe[i].Invoke(argsUp) == EventAction.StopPropagation)
            {
                break;
            }
        }
    }

    private void RawMouseInput(in RAWINPUTMOUSE e)
    {
        if (_mouseRawInputPipe.Count > 0)
        {
            MouseButtons buttons = MouseButtons.None;
            int          clicks  = 0;
            if ((e.ButtonFlags & RawMouseButtons.LeftDown) == RawMouseButtons.LeftDown)
            {
                buttons |= MouseButtons.Left;
                clicks  =  1;
            }
            if ((e.ButtonFlags & RawMouseButtons.RightDown) == RawMouseButtons.RightDown)
            {
                buttons |= MouseButtons.Right;
                clicks  =  1;
            }
            if ((e.ButtonFlags & RawMouseButtons.MiddleDown) == RawMouseButtons.MiddleDown)
            {
                buttons |= MouseButtons.Middle;
                clicks  =  1;
            }
            if ((e.ButtonFlags & RawMouseButtons.Button4Down) == RawMouseButtons.Button4Down)
            {
                buttons |= MouseButtons.XButton1;
                clicks  =  1;
            }
            if ((e.ButtonFlags & RawMouseButtons.Button5Down) == RawMouseButtons.Button5Down)
            {
                buttons |= MouseButtons.XButton2;
                clicks  =  1;
            }

            MouseEventArgs args = new MouseEventArgs(
                e.LastX, e.LastY, buttons, clicks, e.ButtonData);
            for (int i = 0; i < _mouseRawInputPipe.Count; i++)
            {
                if (_mouseRawInputPipe[i].Invoke(args) == EventAction.StopPropagation)
                {
                    break;
                }
            }
        }
    }
}
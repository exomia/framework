#nullable enable

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Exomia.Framework.Input;
using Exomia.Framework.Win32;

namespace Exomia.Framework.Game
{
    sealed partial class RenderForm
    {
        private const int MOUSE_LE_STATE = 1;

        /// <summary>
        ///     Occurs when the mouse leaves the client area.
        /// </summary>
        public event EventHandler<IntPtr>? MouseLeave;

        /// <summary>
        ///     Occurs when the mouse enters the client area.
        /// </summary>
        public event EventHandler<IntPtr>? MouseEnter;

        /// <summary>
        ///     Occurs when the form is closing.
        /// </summary>
        public event RefEventHandler<IntPtr, bool>? FormClosing;


        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            Message m;
            m.hWnd = hWnd;
            m.msg = msg;
            m.wParam = wParam;
            m.lParam = lParam;

            switch (msg)
            {
                case WM.MOUSELEAVE:
                    _state &= ~MOUSE_LE_STATE;
                    MouseLeave?.Invoke(_hWnd);
                    break;
                case WM.SIZE:
                    _size.X = LowWord(lParam);
                    _size.Y = HighWord(lParam);
                    break;
                case WM.CLOSE:
                    bool close = true;
                    FormClosing?.Invoke(_hWnd, ref close);
                    if (close)
                    {
                        User32.DestroyWindow(_hWnd);
                        User32.PostQuitMessage(0);
                    }
                    return IntPtr.Zero;
                case WM.DESTROY:
                    User32.DestroyWindow(_hWnd);
                    User32.PostQuitMessage(0);
                    MouseLeave?.Invoke(_hWnd);
                    return IntPtr.Zero;
                case WM.KEYDOWN:
                case WM.KEYUP:
                case WM.CHAR:
                case WM.UNICHAR:
                case WM.SYSKEYDOWN:
                case WM.SYSKEYUP:
                    RawKeyMessage(ref m);
                    return IntPtr.Zero;
                case WM.LBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Left);
                    return IntPtr.Zero;
                case WM.MBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Middle);
                    return IntPtr.Zero;
                case WM.RBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Right);
                    return IntPtr.Zero;
                case WM.XBUTTONDOWN:
                    RawMouseDown(
                        ref m, HighWord(m.wParam) == 1
                            ? Input.MouseButtons.XButton1
                            : Input.MouseButtons.XButton2);
                    return IntPtr.Zero;
                case WM.LBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Left);
                    return IntPtr.Zero;
                case WM.MBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Middle);
                    return IntPtr.Zero;
                case WM.RBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Right);
                    return IntPtr.Zero;
                case WM.XBUTTONUP:
                    RawMouseUp(
                        ref m, HighWord(m.wParam) == 1
                            ? Input.MouseButtons.XButton1
                            : Input.MouseButtons.XButton2);
                    return IntPtr.Zero;
                case WM.MOUSEMOVE:
                    {
                        if ((_state & MOUSE_LE_STATE) != MOUSE_LE_STATE)
                        {
                            _state |= MOUSE_LE_STATE;
                            MouseEnter?.Invoke(_hWnd);
                            
                            TRACKMOUSEEVENT trackMouseEvent = new TRACKMOUSEEVENT(TME.LEAVE, _hWnd, 0);
                            if (!User32.TrackMouseEvent(ref trackMouseEvent))
                            {
                                throw new Win32Exception(
                                    Kernel32.GetLastError(), $"{nameof(User32.TrackMouseEvent)} failed!");
                            }
                        }

                        int x = LowWord(m.lParam);
                        int y = HighWord(m.lParam);
                        MouseButtons mouseButtons = (MouseButtons)LowWord(m.wParam);
                        for (int i = 0; i < _mouseMovePipe.Count; i++)
                        {
                            if (_mouseMovePipe[i].Invoke(new MouseEventArgs(x, y, mouseButtons, 0, 0))) { break; }
                        }
                        return IntPtr.Zero;
                    }
                case WM.MOUSEWHEEL:
                    {
                        int x = LowWord(m.lParam);
                        int y = HighWord(m.lParam);
                        MouseButtons mouseButtons = (MouseButtons)LowWord(m.wParam);
                        int wheelDelta = HighWord(m.wParam);
                        for (int i = 0; i < _mouseWheelPipe.Count; i++)
                        {
                            if (_mouseWheelPipe[i].Invoke(new MouseEventArgs(x, y, mouseButtons, 2, wheelDelta)))
                            {
                                break;
                            }
                        }
                    }
                    return IntPtr.Zero;
                case WM.LBUTTONDBLCLK:
                case WM.MBUTTONDBLCLK:
                case WM.RBUTTONDBLCLK:
                case WM.XBUTTONDBLCLK:
                    {
                        _state |= 0xC000000;
                        int x = LowWord(m.lParam);
                        int y = HighWord(m.lParam);
                        MouseButtons mouseButtons = (MouseButtons)LowWord(m.wParam);
                        for (int i = 0; i < _mouseClickPipe.Count; i++)
                        {
                            if (_mouseClickPipe[i].Invoke(new MouseEventArgs(x, y, mouseButtons, 2, 0))) { break; }
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
                if (_rawKeyPipe[i].Invoke(m))
                {
                    break;
                }
            }

            int vKey = (int)m.wParam.ToInt64();

            switch (m.msg)
            {
                case WM.SYSKEYDOWN:
                case WM.KEYDOWN:
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
                        if (_keyDownPipe[i].Invoke(vKey, _keyModifier))
                        {
                            break;
                        }
                    }
                    break;
                case WM.SYSKEYUP:
                case WM.KEYUP:
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
                        if (_keyUpPipe[i].Invoke(vKey, _keyModifier))
                        {
                            break;
                        }
                    }
                    break;
                case WM.UNICHAR:
                case WM.CHAR:
                    for (int i = 0; i < _keyPressPipe.Count; i++)
                    {
                        if (_keyPressPipe[i].Invoke((char)vKey))
                        {
                            break;
                        }
                    }
                    break;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RawMouseDown(ref Message m, MouseButtons buttons)
        {
            _state |= 0x8000000;
            int low = LowWord(m.lParam);
            int high = HighWord(m.lParam);
            for (int i = 0; i < _mouseDownPipe.Count; i++)
            {
                if (_mouseDownPipe[i].Invoke(new MouseEventArgs(low, high, buttons, 1, 0)))
                {
                    break;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RawMouseUp(ref Message m, MouseButtons buttons)
        {
            int low = LowWord(m.lParam);
            int high = HighWord(m.lParam);
            if ((_state & 0x8000000) == 0x8000000)
            {
                int clicks = (_state & 0x4000000) == 0x4000000 ? 2 : 1;
                for (int i = 0; i < _mouseClickPipe.Count; i++)
                {
                    if (_mouseClickPipe[i].Invoke(new MouseEventArgs(low, high, buttons, clicks, 0)))
                    {
                        break;
                    }
                }
            }
            _state &= ~0xC000000;
            for (int i = 0; i < _mouseUpPipe.Count; i++)
            {
                if (_mouseUpPipe[i].Invoke(new MouseEventArgs(low, high, buttons, 1, 0))) { break; }
            }
        }

        //private void DeviceOnMouseInput(object sender, MouseInputEventArgs e)
        //{
        //    MouseButtons buttons = Input.MouseButtons.None;
        //    int          clicks  = 0;
        //    if ((e.ButtonFlags & MouseButtonFlags.LeftButtonDown) == MouseButtonFlags.LeftButtonDown)
        //    {
        //        buttons |= Input.MouseButtons.Left;
        //        clicks  =  1;
        //    }
        //    if ((e.ButtonFlags & MouseButtonFlags.RightButtonDown) == MouseButtonFlags.RightButtonDown)
        //    {
        //        buttons |= Input.MouseButtons.Right;
        //        clicks  =  1;
        //    }
        //    if ((e.ButtonFlags & MouseButtonFlags.MiddleButtonDown) == MouseButtonFlags.MiddleButtonDown)
        //    {
        //        buttons |= Input.MouseButtons.Middle;
        //        clicks  =  1;
        //    }
        //    if ((e.ButtonFlags & MouseButtonFlags.Button4Down) == MouseButtonFlags.Button4Down)
        //    {
        //        buttons |= Input.MouseButtons.XButton1;
        //        clicks  =  1;
        //    }
        //    if ((e.ButtonFlags & MouseButtonFlags.Button5Down) == MouseButtonFlags.Button5Down)
        //    {
        //        buttons |= Input.MouseButtons.XButton2;
        //        clicks  =  1;
        //    }
        //    for (int i = 0; i < _mouseRawInputPipe.Count; i++)
        //    {
        //        if (_mouseRawInputPipe[i].Invoke(new MouseEventArgs(e.X, e.Y, buttons, clicks, e.WheelDelta)))
        //        {
        //            break;
        //        }
        //    }
        //}
    }
}

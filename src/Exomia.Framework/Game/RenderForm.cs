#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Exomia.Framework.Input;
using MouseEventHandler = Exomia.Framework.Input.MouseEventHandler;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     The RenderForm.
    /// </summary>
    public class RenderForm : SharpDX.Windows.RenderForm, IRawInputDevice
    {
        /// <summary>
        ///     Occurs when Raw Input Device. Raw Mouse Down.
        /// </summary>
        event MouseEventHandler? IRawInputDevice.RawMouseDown
        {
            add { _rawMouseDown += value; }
            remove
            {
                if (_rawMouseDown != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawMouseDown -= value;
                }
            }
        }

        /// <summary>
        ///     Occurs when Raw Input Device. Raw Mouse Up.
        /// </summary>
        event MouseEventHandler? IRawInputDevice.RawMouseUp
        {
            add { _rawMouseUp += value; }
            remove
            {
                if (_rawMouseUp != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawMouseUp -= value;
                }
            }
        }

        /// <summary>
        ///     Occurs when Raw Input Device. Raw Mouse click.
        /// </summary>
        event MouseEventHandler? IRawInputDevice.RawMouseClick
        {
            add { _rawMouseClick += value; }
            remove
            {
                if (_rawMouseClick != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawMouseClick -= value;
                }
            }
        }

        /// <summary>
        ///     Occurs when Raw Input Device. Raw Mouse Move.
        /// </summary>
        event MouseEventHandler? IRawInputDevice.RawMouseMove
        {
            add { _rawMouseMove += value; }
            remove
            {
                if (_rawMouseMove != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawMouseMove -= value;
                }
            }
        }

        /// <summary>
        ///     Occurs when Raw Input Device. Raw Mouse Wheel.
        /// </summary>
        event MouseEventHandler? IRawInputDevice.RawMouseWheel
        {
            add { _rawMouseWheel += value; }
            remove
            {
                if (_rawMouseWheel != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawMouseWheel -= value;
                }
            }
        }

        /// <summary>
        ///     Occurs when raw Input Device. Raw Key.
        /// </summary>
        event RefEventHandler<Message>? IRawInputDevice.RawKeyEvent
        {
            add { _rawKeyEvent += value; }
            remove
            {
                if (_rawKeyEvent != null)
                {
                    // ReSharper disable once DelegateSubtraction
                    _rawKeyEvent -= value;
                }
            }
        }

        /// <summary>
        ///     Gets the raw mouse wheel.
        /// </summary>
        private MouseEventHandler? _rawMouseDown;

        /// <summary>
        ///     The raw mouse up.
        /// </summary>
        private MouseEventHandler? _rawMouseUp;

        /// <summary>
        ///     The raw mouse click.
        /// </summary>
        private MouseEventHandler? _rawMouseClick;

        /// <summary>
        ///     The raw mouse move.
        /// </summary>
        private MouseEventHandler? _rawMouseMove;

        /// <summary>
        ///     The raw mouse wheel.
        /// </summary>
        private MouseEventHandler? _rawMouseWheel;

        /// <summary>
        ///     The raw key event.
        /// </summary>
        private RefEventHandler<Message>? _rawKeyEvent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RenderForm" /> class.
        /// </summary>
        /// <param name="text"> The text. </param>
        public RenderForm(string text)
            : base(text) { }

        /// <summary>
        ///     Low word.
        /// </summary>
        /// <param name="number"> Number of. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowWord(IntPtr number)
        {
            return (int)number.ToInt64() & 0x0000FFFF;
        }

        /// <summary>
        ///     High word.
        /// </summary>
        /// <param name="number"> Number of. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HighWord(IntPtr number)
        {
            return (int)number.ToInt64() >> 16;
        }

        private int _state = 0;

        /// <summary>
        ///     Gets a state.
        /// </summary>
        /// <param name="flag"> The flag. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool GetState(int flag)
        {
            return (_state & flag) == flag;
        }

        /// <summary>
        ///     Sets a state.
        /// </summary>
        /// <param name="flag">  The flag. </param>
        /// <param name="value"> True to value. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetState(int flag, bool value)
        {
            _state = value ? _state | flag : _state & ~flag;
        }
        
        /// <summary>
        ///     Raw mouse up.
        /// </summary>
        /// <param name="m">       [in,out] The ref Message to process. </param>
        /// <param name="buttons"> The buttons. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RawMouseUp(ref Message m, Input.MouseButtons buttons)
        {
            buttons |= (Input.MouseButtons)LowWord(m.WParam);
            int          low     = LowWord(m.LParam);
            int          high    = HighWord(m.LParam);
            if (GetState(0x8000000))
            {
                _rawMouseClick?.Invoke(low, high, buttons, !GetState(0x4000000) ? 1 : 2, 0);
            }
            SetState(0x4000000, false);
            SetState(0x8000000, false);
            _rawMouseUp?.Invoke(low, high, buttons, 1, 0);
        }
        
        /// <inheritdoc/>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32Message.WM_KEYDOWN:
                case Win32Message.WM_KEYUP:
                case Win32Message.WM_CHAR:
                case Win32Message.WM_UNICHAR:
                case Win32Message.WM_SYSKEYDOWN:
                case Win32Message.WM_SYSKEYUP:
                    _rawKeyEvent?.Invoke(ref m);
                    break;
                case Win32Message.WM_LBUTTONDOWN:
                case Win32Message.WM_MBUTTONDOWN:
                case Win32Message.WM_RBUTTONDOWN:
                case Win32Message.WM_XBUTTONDOWN:
                    SetState(0x8000000, true);
                    _rawMouseDown?.Invoke(LowWord(m.LParam), HighWord(m.LParam), (Input.MouseButtons)LowWord(m.WParam), 1, 0);
                    break;
                case Win32Message.WM_LBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Left);
                    break;
                case Win32Message.WM_MBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Middle);
                    break;
                case Win32Message.WM_RBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.Right);
                    break;
                case Win32Message.WM_XBUTTONUP:
                    RawMouseUp(ref m, Input.MouseButtons.None);
                    break;
                case Win32Message.WM_MOUSEMOVE:
                    _rawMouseMove?.Invoke(LowWord(m.LParam), HighWord(m.LParam), (Input.MouseButtons)LowWord(m.WParam), 0, 0);
                    break;
                case Win32Message.WM_MOUSEWHEEL:
                    _rawMouseWheel?.Invoke(
                        LowWord(m.LParam), HighWord(m.LParam), (Input.MouseButtons)LowWord(m.WParam), 1, HighWord(m.WParam));
                    break;
                case Win32Message.WM_LBUTTONDBLCLK:
                case Win32Message.WM_MBUTTONDBLCLK:
                case Win32Message.WM_RBUTTONDBLCLK:
                case Win32Message.WM_XBUTTONDBLCLK:
                    SetState(0xC000000, true);
                    _rawMouseDown?.Invoke(LowWord(m.LParam), HighWord(m.LParam), (Input.MouseButtons)LowWord(m.WParam), 2, 0);
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
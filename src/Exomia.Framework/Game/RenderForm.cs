﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Exomia.Framework.Input;
using KeyEventHandler = Exomia.Framework.Input.KeyEventHandler;
using KeyPressEventHandler = Exomia.Framework.Input.KeyPressEventHandler;
using MouseButtons = Exomia.Framework.Input.MouseButtons;
using MouseEventHandler = Exomia.Framework.Input.MouseEventHandler;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     The RenderForm.
    /// </summary>
    public class RenderForm : SharpDX.Windows.RenderForm, IInputDevice
    {
        private readonly Pipe<RawKeyEventHandler>   _rawKeyPipe;
        private readonly Pipe<KeyEventHandler>      _keyUpPipe, _keyDownPipe;
        private readonly Pipe<KeyPressEventHandler> _keyPressPipe;

        private readonly Pipe<MouseEventHandler> _mouseMovePipe,
                                                 _mouseUpPipe,
                                                 _mouseDownPipe,
                                                 _mouseClickPipe,
                                                 _mouseWheelPipe;

        private int         _state;
        private KeyModifier _keyModifier = 0;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RenderForm" /> class.
        /// </summary>
        /// <param name="text"> The text. </param>
        public RenderForm(string text)
            : base(text)
        {
            _rawKeyPipe     = new Pipe<RawKeyEventHandler>();
            _keyUpPipe      = new Pipe<KeyEventHandler>();
            _keyDownPipe    = new Pipe<KeyEventHandler>();
            _keyPressPipe   = new Pipe<KeyPressEventHandler>();
            _mouseMovePipe  = new Pipe<MouseEventHandler>();
            _mouseUpPipe    = new Pipe<MouseEventHandler>();
            _mouseDownPipe  = new Pipe<MouseEventHandler>();
            _mouseClickPipe = new Pipe<MouseEventHandler>();
            _mouseWheelPipe = new Pipe<MouseEventHandler>();
        }

        /// <inheritdoc />
        public void RegisterRawKeyEvent(RawKeyEventHandler handler, int position = -1)
        {
            _rawKeyPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterKeyUp(KeyEventHandler handler, int position = -1)
        {
            _keyUpPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterKeyPress(KeyPressEventHandler handler, int position = -1)
        {
            _keyPressPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterKeyDown(KeyEventHandler handler, int position = -1)
        {
            _keyDownPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterMouseDown(MouseEventHandler handler, int position = -1)
        {
            _mouseDownPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterMouseUp(MouseEventHandler handler, int position = -1)
        {
            _mouseUpPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterMouseClick(MouseEventHandler handler, int position = -1)
        {
            _mouseClickPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterMouseMove(MouseEventHandler handler, int position = -1)
        {
            _mouseMovePipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void RegisterMouseWheel(MouseEventHandler handler, int position = -1)
        {
            _mouseWheelPipe.Register(handler, position);
        }

        /// <inheritdoc />
        public void UnregisterRawKeyEvent(RawKeyEventHandler handler)
        {
            _rawKeyPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterKeyUp(KeyEventHandler handler)
        {
            _keyUpPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterKeyPress(KeyPressEventHandler handler)
        {
            _keyPressPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterKeyDown(KeyEventHandler handler)
        {
            _keyDownPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterMouseDown(MouseEventHandler handler)
        {
            _mouseDownPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterMouseUp(MouseEventHandler handler)
        {
            _mouseUpPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterMouseClick(MouseEventHandler handler)
        {
            _mouseClickPipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterMouseMove(MouseEventHandler handler)
        {
            _mouseMovePipe.Unregister(handler);
        }

        /// <inheritdoc />
        public void UnregisterMouseWheel(MouseEventHandler handler)
        {
            _mouseWheelPipe.Unregister(handler);
        }

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

        /// <inheritdoc />
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
                    RawKeyMessage(ref m);
                    break;
                case Win32Message.WM_LBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Left);
                    break;
                case Win32Message.WM_MBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Middle);
                    break;
                case Win32Message.WM_RBUTTONDOWN:
                    RawMouseDown(ref m, Input.MouseButtons.Right);
                    break;
                case Win32Message.WM_XBUTTONDOWN:
                    RawMouseDown(
                        ref m, HighWord(m.WParam) == 1
                            ? Input.MouseButtons.XButton1
                            : Input.MouseButtons.XButton2);
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
                    RawMouseUp(
                        ref m, HighWord(m.WParam) == 1
                            ? Input.MouseButtons.XButton1
                            : Input.MouseButtons.XButton2);
                    break;
                case Win32Message.WM_MOUSEMOVE:
                    for (int i = 0; i < _mouseMovePipe.Count; i++)
                    {
                        if (_mouseMovePipe[i]
                            .Invoke(LowWord(m.LParam), HighWord(m.LParam), (MouseButtons)LowWord(m.WParam), 0, 0))
                        {
                            break;
                        }
                    }
                    break;
                case Win32Message.WM_MOUSEWHEEL:
                    for (int i = 0; i < _mouseWheelPipe.Count; i++)
                    {
                        if (_mouseWheelPipe[i]
                            .Invoke(
                                LowWord(m.LParam), HighWord(m.LParam), (MouseButtons)LowWord(m.WParam), 1,
                                HighWord(m.WParam)))
                        {
                            break;
                        }
                    }
                    break;
                case Win32Message.WM_LBUTTONDBLCLK:
                case Win32Message.WM_MBUTTONDBLCLK:
                case Win32Message.WM_RBUTTONDBLCLK:
                case Win32Message.WM_XBUTTONDBLCLK:
                    _state |= 0xC000000;
                    for (int i = 0; i < _mouseClickPipe.Count; i++)
                    {
                        if (_mouseClickPipe[i]
                            .Invoke(LowWord(m.LParam), HighWord(m.LParam), (MouseButtons)LowWord(m.WParam), 2, 0))
                        {
                            break;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        ///     Raw key message.
        /// </summary>
        /// <param name="m"> [in,out] The ref Message to process. </param>
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

            int vKey = (int)m.WParam.ToInt64();

            switch (m.Msg)
            {
                case Win32Message.WM_SYSKEYDOWN:
                case Win32Message.WM_KEYDOWN:
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
                case Win32Message.WM_SYSKEYUP:
                case Win32Message.WM_KEYUP:
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
                case Win32Message.WM_UNICHAR:
                case Win32Message.WM_CHAR:
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

        /// <summary>
        ///     Raw mouse up.
        /// </summary>
        /// <param name="m">       [in,out] The ref Message to process. </param>
        /// <param name="buttons"> The buttons. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RawMouseDown(ref Message m, MouseButtons buttons)
        {
            _state |= 0x8000000;
            for (int i = 0; i < _mouseWheelPipe.Count; i++)
            {
                if (_mouseDownPipe[i].Invoke(LowWord(m.LParam), HighWord(m.LParam), buttons, 1, 0))
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Raw mouse up.
        /// </summary>
        /// <param name="m">       [in,out] The ref Message to process. </param>
        /// <param name="buttons"> The buttons. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RawMouseUp(ref Message m, MouseButtons buttons)
        {
            int low  = LowWord(m.LParam);
            int high = HighWord(m.LParam);
            if ((_state & 0x8000000) == 0x8000000)
            {
                for (int i = 0; i < _mouseClickPipe.Count; i++)
                {
                    if (_mouseClickPipe[i].Invoke(low, high, buttons, (_state & 0x4000000) == 0x4000000 ? 2 : 1, 0))
                    {
                        break;
                    }
                }
            }
            _state &= ~0xC000000;
            for (int i = 0; i < _mouseUpPipe.Count; i++)
            {
                if (_mouseUpPipe[i].Invoke(low, high, buttons, 1, 0)) { break; }
            }
        }

        /// <summary>
        ///     A pipe.
        /// </summary>
        /// <typeparam name="TDelegate"> Type of the delegate. </typeparam>
        private sealed class Pipe<TDelegate>
            where TDelegate : Delegate
        {
            /// <summary>
            ///     The list.
            /// </summary>
            private readonly List<TDelegate> _list;

            /// <summary>
            ///     Gets the number of the registered delegates.
            /// </summary>
            /// <value>
            ///     The count.
            /// </value>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return _list.Count; }
            }

            /// <summary>
            ///     Indexer to get items within this collection using array index syntax.
            /// </summary>
            /// <param name="index"> Zero-based index of the entry to access. </param>
            /// <returns>
            ///     The indexed item.
            /// </returns>
            public TDelegate this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return _list[index]; }
            }

            /// <summary>
            ///     Initializes a new instance of the &lt;see cref="Pipe&lt;TDelegate&gt;"/&gt; class.
            /// </summary>
            public Pipe()
            {
                _list = new List<TDelegate>(8);
            }

            /// <summary>
            ///     Registers this object.
            /// </summary>
            /// <param name="handler">  The handler. </param>
            /// <param name="position"> (Optional) The position. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Register(in TDelegate handler, int position = -1)
            {
                if (position == -1) { _list.Add(handler); }
                else { _list.Insert(position >= 0 ? position : _list.Count + position, handler); }
            }

            /// <summary>
            ///     Deregisters this object.
            /// </summary>
            /// <param name="handler"> The handler. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Unregister(in TDelegate handler)
            {
                _list.Remove(handler);
            }

            /// <summary>
            ///     Deregisters this object.
            /// </summary>
            /// <param name="position"> The position. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Unregister(int position)
            {
                _list.RemoveAt(position);
            }
        }
    }
}
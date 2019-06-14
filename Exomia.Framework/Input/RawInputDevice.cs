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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Exomia.Framework.Game;
using SharpDX.Multimedia;
using SharpDX.RawInput;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     A raw input device. This class cannot be inherited.
    /// </summary>
    public sealed class RawInputDevice : IRawInputDevice, IDisposable
    {
        /// <summary>
        ///     Occurs when Key Down.
        /// </summary>
        public event RKeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when Key Press.
        /// </summary>
        public event RKeyEventHandler KeyPress;

        /// <summary>
        ///     Occurs when Key Up.
        /// </summary>
        public event RKeyEventHandler KeyUp;

        /// <summary>
        ///     Occurs when Mouse Down.
        /// </summary>
        public event RMouseEventHandler MouseDown;

        /// <summary>
        ///     Occurs when Mouse Move.
        /// </summary>
        public event RMouseEventHandler MouseMove;

        /// <summary>
        ///     Occurs when Mouse Up.
        /// </summary>
        public event RMouseEventHandler MouseUp;

        /// <summary>
        ///     The pressed keys.
        /// </summary>
        private readonly HashSet<int> _pressedKeys;

        /// <summary>
        ///     The mouse position.
        /// </summary>
        private Point _mousePosition = Point.Empty;

        /// <summary>
        ///     Buffer for mouse wheel data data.
        /// </summary>
        private int _mouseWheelDataBuffer;

        /// <summary>
        ///     The panel.
        /// </summary>
        private Panel _panel;

        /// <summary>
        ///     The pressed mouse buttons.
        /// </summary>
        private RMouseButtons _pressedMouseButtons = 0;

        /// <summary>
        ///     The window.
        /// </summary>
        private IGameWindow _window;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RawInputDevice" /> class.
        /// </summary>
        public RawInputDevice()
        {
            _pressedKeys = new HashSet<int>();

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
            Device.KeyboardInput += Device_KeyboardInput;

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);
            Device.MouseInput += Device_MouseInput;
        }

        /// <inheritdoc />
        public void EndUpdate()
        {
            WheelData             = _mouseWheelDataBuffer;
            _mouseWheelDataBuffer = 0;
        }

        /// <inheritdoc />
        public void Initialize(IGameWindow window)
        {
            _window = window;
            if (_window is IWinFormsGameWindow formsWindow)
            {
                formsWindow.RenderForm.MouseMove += RenderForm_MouseMove;
            }
        }

        /// <inheritdoc />
        public void Initialize(Panel panel)
        {
            _panel           =  panel;
            _panel.MouseMove += RenderForm_MouseMove;
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for mouse move events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void RenderForm_MouseMove(object sender, MouseEventArgs e)
        {
            _mousePosition = e.Location;
            MouseMove?.Invoke(e.X, e.Y, _pressedMouseButtons, 1, e.Delta);
        }

        //TODO: CHANGE TO FORM INPUT INSTEAD OF RAW INPUT

        #region Device MouseInput

        /// <summary>
        ///     Event handler. Called by Device for mouse input events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse input event information. </param>
        private void Device_MouseInput(object sender, MouseInputEventArgs e)
        {
            if ((e.ButtonFlags & MouseButtonFlags.MouseWheel) == MouseButtonFlags.MouseWheel)
            {
                _mouseWheelDataBuffer += e.WheelDelta > 0 ? -1 : 1;
            }

            if ((e.ButtonFlags & MouseButtonFlags.LeftButtonDown) == MouseButtonFlags.LeftButtonDown)
            {
                _pressedMouseButtons |= RMouseButtons.Left;
                MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Left, 1, e.WheelDelta);
            }
            else if ((e.ButtonFlags & MouseButtonFlags.LeftButtonUp) == MouseButtonFlags.LeftButtonUp)
            {
                _pressedMouseButtons &= ~RMouseButtons.Left;
                MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Left, 1, e.WheelDelta);
            }

            if ((e.ButtonFlags & MouseButtonFlags.RightButtonDown) == MouseButtonFlags.RightButtonDown)
            {
                _pressedMouseButtons |= RMouseButtons.Right;
                MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Right, 1, e.WheelDelta);
            }
            else if ((e.ButtonFlags & MouseButtonFlags.RightButtonUp) == MouseButtonFlags.RightButtonUp)
            {
                _pressedMouseButtons &= ~RMouseButtons.Right;
                MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Right, 1, e.WheelDelta);
            }

            if ((e.ButtonFlags & MouseButtonFlags.MiddleButtonDown) == MouseButtonFlags.MiddleButtonDown)
            {
                _pressedMouseButtons |= RMouseButtons.Middle;
                MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Middle, 1, e.WheelDelta);
            }
            else if ((e.ButtonFlags & MouseButtonFlags.MiddleButtonUp) == MouseButtonFlags.MiddleButtonUp)
            {
                _pressedMouseButtons &= ~RMouseButtons.Middle;
                MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Left, 1, e.WheelDelta);
            }

            if ((e.ButtonFlags & MouseButtonFlags.Button4Down) == MouseButtonFlags.Button4Down)
            {
                _pressedMouseButtons |= RMouseButtons.Button4;
                MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Button4, 1, e.WheelDelta);
            }
            else if ((e.ButtonFlags & MouseButtonFlags.Button4Up) == MouseButtonFlags.Button4Up)
            {
                _pressedMouseButtons &= ~RMouseButtons.Button4;
                MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Button4, 1, e.WheelDelta);
            }

            if ((e.ButtonFlags & MouseButtonFlags.Button5Down) == MouseButtonFlags.Button5Down)
            {
                _pressedMouseButtons |= RMouseButtons.Button5;
                MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Button5, 1, e.WheelDelta);
            }
            else if ((e.ButtonFlags & MouseButtonFlags.Button5Up) == MouseButtonFlags.Button5Up)
            {
                _pressedMouseButtons &= ~RMouseButtons.Button5;
                MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, RMouseButtons.Button5, 1, e.WheelDelta);
            }
        }

        /// <inheritdoc />
        public bool IsMouseButtonDown(RMouseButtons button)
        {
            //return _pressedMouseButtons.Contains(button);
            return (_pressedMouseButtons & button) == button;
        }

        /// <inheritdoc />
        public bool IsMouseButtonDown(params RMouseButtons[] buttons)
        {
            int l = buttons.Length;
            for (int i = 0; i < l; i++)
            {
                //if (_pressedMouseButtons.Contains(buttons[i])) { return true; }
                if (IsMouseButtonDown(buttons[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsKeyUp(RMouseButtons button)
        {
            return !IsMouseButtonDown(button);
        }

        /// <inheritdoc />
        public bool IsKeyUp(params RMouseButtons[] buttons)
        {
            int l = buttons.Length;
            for (int i = 0; i < l; i++)
            {
                if (!IsMouseButtonDown(buttons[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        public int WheelData { get; private set; }

        #endregion

        #region Device KeyboardInput

        /// <summary>
        ///     Event handler. Called by Device for keyboard input events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Keyboard input event information. </param>
        private void Device_KeyboardInput(object sender, KeyboardInputEventArgs e)
        {
            RSpecialKeys sKeys = RSpecialKeys.None;
            if (IsKeyDown(Key.ShiftKey))
            {
                sKeys |= RSpecialKeys.Shift;
            }
            if (IsKeyDown(Key.Alt))
            {
                sKeys |= RSpecialKeys.Alt;
            }
            if (IsKeyDown(Key.ControlKey))
            {
                sKeys |= RSpecialKeys.Control;
            }
            if (e.State == KeyState.KeyDown)
            {
                if (_pressedKeys.Add((int)e.Key))
                {
                    KeyDown?.Invoke((int)e.Key, e.State, e.ExtraInformation, sKeys);
                }
            }
            if (e.State == KeyState.KeyUp)
            {
                if (_pressedKeys.Remove((int)e.Key))
                {
                    KeyPress?.Invoke((int)e.Key, e.State, e.ExtraInformation, sKeys);
                }
                KeyUp?.Invoke((int)e.Key, e.State, e.ExtraInformation, sKeys);
            }
        }

        /// <inheritdoc />
        public bool IsKeyDown(int key)
        {
            return _pressedKeys.Contains(key);
        }

        /// <inheritdoc />
        public bool IsKeyDown(params int[] key)
        {
            int l = key.Length;
            for (int i = 0; i < l; i++)
            {
                if (_pressedKeys.Contains(key[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsKeyUp(int key)
        {
            return !_pressedKeys.Contains(key);
        }

        /// <inheritdoc />
        public bool IsKeyUp(params int[] key)
        {
            int l = key.Length;
            for (int i = 0; i < l; i++)
            {
                if (!_pressedKeys.Contains(key[i])) { return true; }
            }
            return false;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     True to disposed value.
        /// </summary>
        private bool _disposedValue;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Input.RawInputDevice and
        ///     optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing) { }

                Device.KeyboardInput -= Device_KeyboardInput;
                Device.MouseInput    -= Device_MouseInput;

                if (_window != null)
                {
                    if (_window is IWinFormsGameWindow formsWindow)
                    {
                        formsWindow.RenderForm.MouseMove -= RenderForm_MouseMove;
                    }
                }
                if (_panel != null)
                {
                    _panel.MouseMove -= RenderForm_MouseMove;
                }

                _disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
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
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Exomia.Framework.Game;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     A window forms input device. This class cannot be inherited.
    /// </summary>
    public sealed class WinFormsInputDevice : IInputDevice, IDisposable
    {
        /// <summary>
        ///     Occurs when Key Down.
        /// </summary>
        /// <inheritdoc />
        public event KeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when Key Press.
        /// </summary>
        /// <inheritdoc />
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     Occurs when Key Up.
        /// </summary>
        /// <inheritdoc />
        public event KeyEventHandler KeyUp;

        /// <summary>
        ///     Occurs when Mouse Click.
        /// </summary>
        /// <inheritdoc />
        public event MouseEventHandler MouseClick;

        /// <summary>
        ///     Occurs when Mouse Down.
        /// </summary>
        /// <inheritdoc />
        public event MouseEventHandler MouseDown;

        /// <summary>
        ///     Occurs when Mouse Move.
        /// </summary>
        /// <inheritdoc />
        public event MouseEventHandler MouseMove;

        /// <summary>
        ///     Occurs when Mouse Up.
        /// </summary>
        /// <inheritdoc />
        public event MouseEventHandler MouseUp;

        /// <summary>
        ///     Occurs when Mouse Wheel.
        /// </summary>
        /// <inheritdoc />
        public event MouseEventHandler MouseWheel;

        /// <summary>
        ///     The pressed keys.
        /// </summary>
        private readonly HashSet<int> _pressedKeys = new HashSet<int>();

        /// <summary>
        ///     The window.
        /// </summary>
        private readonly IWinFormsGameWindow _window;

        /// <summary>
        ///     The mouse position.
        /// </summary>
        private Point _mousePosition = Point.Empty;

        /// <summary>
        ///     The pressed mouse buttons.
        /// </summary>
        private MouseButtons _pressedMouseButtons = MouseButtons.None;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WinFormsInputDevice" /> class.
        /// </summary>
        /// <param name="game"> The game. </param>
        public WinFormsInputDevice(Game.Game game)
            : this(game.GameWindow as IWinFormsGameWindow) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WinFormsInputDevice" /> class.
        /// </summary>
        /// <param name="window"> The win forms game window. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public WinFormsInputDevice(IWinFormsGameWindow window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));

            _window.RenderForm.MouseMove  += Renderform_MouseMove;
            _window.RenderForm.MouseClick += RenderForm_MouseClick;
            _window.RenderForm.MouseDown  += RenderForm_MouseDown;
            _window.RenderForm.MouseUp    += RenderForm_MouseUp;
            _window.RenderForm.MouseWheel += RenderForm_MouseWheel;

            _window.RenderForm.KeyDown  += RenderForm_KeyDown;
            _window.RenderForm.KeyUp    += RenderForm_KeyUp;
            _window.RenderForm.KeyPress += RenderForm_KeyPress;
        }

        /// <summary>
        ///     WinFormsInputDevice destructor.
        /// </summary>
        ~WinFormsInputDevice()
        {
            Dispose(false);
        }

        #region Device MouseInput

        /// <summary>
        ///     TODO: CHECK LATER PERFORMANCE LIMIT MOUSE MOVE?
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void Renderform_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePosition != e.Location)
            {
                _mousePosition = e.Location;
                MouseMove?.Invoke(e.X, e.Y, _pressedMouseButtons, e.Clicks, e.Delta);
            }
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for mouse wheel events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void RenderForm_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheel?.Invoke(_mousePosition.X, _mousePosition.Y, _pressedMouseButtons, e.Clicks, e.Delta);
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for mouse up events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void RenderForm_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    _pressedMouseButtons &= ~MouseButtons.Left;
                    MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Left, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    _pressedMouseButtons &= ~MouseButtons.Middle;
                    MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Middle, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    _pressedMouseButtons &= ~MouseButtons.Right;
                    MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Right, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton1:
                    _pressedMouseButtons &= ~MouseButtons.Button4;
                    MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button4, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton2:
                    _pressedMouseButtons &= ~MouseButtons.Button5;
                    MouseUp?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button5, e.Clicks, e.Delta);
                    break;
            }
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for mouse down events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void RenderForm_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    _pressedMouseButtons |= MouseButtons.Left;
                    MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Left, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    _pressedMouseButtons |= MouseButtons.Middle;
                    MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Middle, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    _pressedMouseButtons |= MouseButtons.Right;
                    MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Right, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton1:
                    _pressedMouseButtons |= MouseButtons.Button4;
                    MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button4, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton2:
                    _pressedMouseButtons |= MouseButtons.Button5;
                    MouseDown?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button5, e.Clicks, e.Delta);
                    break;
            }
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for mouse click events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Mouse event information. </param>
        private void RenderForm_MouseClick(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    MouseClick?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Left, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Middle:
                    MouseClick?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Middle, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.Right:
                    MouseClick?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Right, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton1:
                    MouseClick?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button4, e.Clicks, e.Delta);
                    break;
                case System.Windows.Forms.MouseButtons.XButton2:
                    MouseClick?.Invoke(_mousePosition.X, _mousePosition.Y, MouseButtons.Button5, e.Clicks, e.Delta);
                    break;
            }
        }

        #endregion

        #region Device KeyboardInput

        /// <summary>
        ///     Event handler. Called by RenderForm for key down events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Key event information. </param>
        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Add(e.KeyValue))
            {
                KeyDown?.Invoke(e.KeyValue, e.Shift, e.Alt, e.Control);
            }
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for key press events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Key press event information. </param>
        private void RenderForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(e.KeyChar);
        }

        /// <summary>
        ///     Event handler. Called by RenderForm for key up events.
        /// </summary>
        /// <param name="sender"> Source of the event. </param>
        /// <param name="e">      Key event information. </param>
        private void RenderForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Remove(e.KeyValue))
            {
                KeyUp?.Invoke(e.KeyValue, e.Shift, e.Alt, e.Control);
            }
        }

        #endregion

        #region MouseHelper

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseButtonDown(MouseButtons button)
        {
            return (_pressedMouseButtons & button) == button;
        }

        /// <inheritdoc />
        public bool IsMouseButtonDown(params MouseButtons[] buttons)
        {
            int l = buttons.Length;
            for (int i = 0; i < l; i++)
            {
                if (IsMouseButtonDown(buttons[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseButtonUp(MouseButtons button)
        {
            return (_pressedMouseButtons & button) != button;
        }

        /// <inheritdoc />
        public bool IsMouseButtonUp(params MouseButtons[] buttons)
        {
            int l = buttons.Length;
            for (int i = 0; i < l; i++)
            {
                if (IsMouseButtonUp(buttons[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        public void SetMousePosition(int x, int y)
        {
            if (_window?.RenderForm != null)
            {
                Cursor.Position = _window.RenderForm.PointToScreen(new Point(x, y));
            }
        }

        #endregion

        #region KeyboardHelper

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyDown(int keyValue)
        {
            return _pressedKeys.Contains(keyValue);
        }

        /// <inheritdoc />
        public bool IsKeyDown(params int[] keyValues)
        {
            int l = keyValues.Length;
            for (int i = 0; i < l; i++)
            {
                if (_pressedKeys.Contains(keyValues[i])) { return true; }
            }
            return false;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyUp(int keyValues)
        {
            return !_pressedKeys.Contains(keyValues);
        }

        /// <inheritdoc />
        public bool IsKeyUp(params int[] keyValues)
        {
            int l = keyValues.Length;
            for (int i = 0; i < l; i++)
            {
                if (!_pressedKeys.Contains(keyValues[i])) { return true; }
            }
            return false;
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Input.WinFormsInputDevice
        ///     and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_window != null)
                    {
                        _window.RenderForm.MouseMove  -= Renderform_MouseMove;
                        _window.RenderForm.MouseClick -= RenderForm_MouseClick;
                        _window.RenderForm.MouseDown  -= RenderForm_MouseDown;
                        _window.RenderForm.MouseUp    -= RenderForm_MouseUp;
                        _window.RenderForm.MouseWheel -= RenderForm_MouseWheel;

                        _window.RenderForm.KeyDown  -= RenderForm_KeyDown;
                        _window.RenderForm.KeyUp    -= RenderForm_KeyUp;
                        _window.RenderForm.KeyPress -= RenderForm_KeyPress;
                    }
                }

                _disposed = true;
            }
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
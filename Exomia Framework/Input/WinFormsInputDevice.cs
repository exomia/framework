#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
    /// <inheritdoc cref="IInputDevice" />
    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    ///     WinFormsInputDevice class
    /// </summary>
    public sealed class WinFormsInputDevice : IInputDevice, IDisposable
    {
        #region Variables

        private readonly HashSet<int> _pressedKeys = new HashSet<int>();

        private readonly IWinFormsGameWindow _window;

        private Point _mousePosition = Point.Empty;

        private MouseButtons _pressedMouseButtons = MouseButtons.None;

        #endregion

        #region Constructors

        /// <summary>
        ///     WinFormsInputDevice constuctor
        /// </summary>
        /// <param name="window">IWinFormsGameWindow</param>
        public WinFormsInputDevice(IWinFormsGameWindow window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));

            _window.RenderForm.MouseMove += Renderform_MouseMove;
            _window.RenderForm.MouseClick += RenderForm_MouseClick;
            _window.RenderForm.MouseDown += RenderForm_MouseDown;
            _window.RenderForm.MouseUp += RenderForm_MouseUp;
            _window.RenderForm.MouseWheel += RenderForm_MouseWheel;

            _window.RenderForm.KeyDown += RenderForm_KeyDown;
            _window.RenderForm.KeyUp += RenderForm_KeyUp;
            _window.RenderForm.KeyPress += RenderForm_KeyPress;
        }

        /// <summary>
        ///     WinFormsInputDevice destructor
        /// </summary>
        ~WinFormsInputDevice()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public event KeyEventHandler KeyDown;

        /// <inheritdoc />
        public event KeyEventHandler KeyUp;

        /// <inheritdoc />
        public event KeyPressEventHandler KeyPress;

        /// <inheritdoc />
        public event MouseEventHandler MouseMove;

        /// <inheritdoc />
        public event MouseEventHandler MouseDown;

        /// <inheritdoc />
        public event MouseEventHandler MouseUp;

        /// <inheritdoc />
        public event MouseEventHandler MouseClick;

        /// <inheritdoc />
        public event MouseEventHandler MouseWheel;

        #endregion

        #region Device MouseInput

        //TODO: CHECK LATER PERFORMANCE LIMIT MOUSE MOVE?
        private void Renderform_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mousePosition != e.Location)
            {
                _mousePosition = e.Location;
                MouseMove?.Invoke(e.X, e.Y, _pressedMouseButtons, e.Clicks, e.Delta);
            }
        }

        private void RenderForm_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseWheel?.Invoke(_mousePosition.X, _mousePosition.Y, _pressedMouseButtons, e.Clicks, e.Delta);
        }

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

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Add(e.KeyValue))
            {
                KeyDown?.Invoke(e.KeyValue, e.Shift, e.Alt, e.Control);
            }
        }

        private void RenderForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPress?.Invoke(e.KeyChar);
        }

        private void RenderForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Remove(e.KeyValue))
            {
                KeyUp?.Invoke(e.KeyValue, e.Shift, e.Alt, e.Control);
            }
        }

        #endregion

        #region MouseHelper

        /// <summary>
        ///     <see cref="IInputDevice.IsMouseButtonDown(MouseButtons)" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseButtonDown(MouseButtons button)
        {
            return (_pressedMouseButtons & button) == button;
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsMouseButtonDown(MouseButtons[])" />
        /// </summary>
        public bool IsMouseButtonDown(params MouseButtons[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (IsMouseButtonDown(buttons[i])) { return true; }
            }
            return false;
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsMouseButtonUp(MouseButtons)" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsMouseButtonUp(MouseButtons button)
        {
            return !((_pressedMouseButtons & button) == button);
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsMouseButtonUp(MouseButtons[])" />
        /// </summary>
        public bool IsMouseButtonUp(params MouseButtons[] buttons)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (IsMouseButtonUp(buttons[i])) { return true; }
            }
            return false;
        }

        /// <summary>
        ///     <see cref="IInputDevice.SetMousePosition(int, int)" />
        /// </summary>
        public void SetMousePosition(int x, int y)
        {
            if (_window != null && _window.RenderForm != null)
            {
                Cursor.Position = _window.RenderForm.PointToScreen(new Point(x, y));
            }
        }

        #endregion

        #region KeyboardHelper

        /// <summary>
        ///     <see cref="IInputDevice.IsKeyDown(int)" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyDown(int keyValue)
        {
            return _pressedKeys.Contains(keyValue);
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsKeyDown(int[])" />
        /// </summary>
        public bool IsKeyDown(params int[] keyValues)
        {
            for (int i = 0; i < keyValues.Length; i++)
            {
                if (_pressedKeys.Contains(keyValues[i])) { return true; }
            }
            return false;
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsKeyUp(int)" />
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKeyUp(int keyValues)
        {
            return !_pressedKeys.Contains(keyValues);
        }

        /// <summary>
        ///     <see cref="IInputDevice.IsKeyUp(int[])" />
        /// </summary>
        public bool IsKeyUp(params int[] keyValues)
        {
            for (int i = 0; i < keyValues.Length; i++)
            {
                if (!_pressedKeys.Contains(keyValues[i])) { return true; }
            }
            return false;
        }

        #endregion

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_window != null)
                    {
                        _window.RenderForm.MouseMove -= Renderform_MouseMove;
                        _window.RenderForm.MouseClick -= RenderForm_MouseClick;
                        _window.RenderForm.MouseDown -= RenderForm_MouseDown;
                        _window.RenderForm.MouseUp -= RenderForm_MouseUp;
                        _window.RenderForm.MouseWheel -= RenderForm_MouseWheel;

                        _window.RenderForm.KeyDown -= RenderForm_KeyDown;
                        _window.RenderForm.KeyUp -= RenderForm_KeyUp;
                        _window.RenderForm.KeyPress -= RenderForm_KeyPress;
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        ///     Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
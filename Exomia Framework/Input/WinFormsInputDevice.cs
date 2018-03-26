using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Exomia.Framework.Game;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     WinFormsInputDevice class
    /// </summary>
    public sealed class WinFormsInputDevice : IInputDevice, IDisposable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        /// <summary>
        ///     <see cref="IInputDevice.KeyDown" />
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        ///     <see cref="IInputDevice.KeyUp" />
        /// </summary>
        public event KeyEventHandler KeyUp;

        /// <summary>
        ///     <see cref="IInputDevice.KeyPress" />
        /// </summary>
        public event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     <see cref="IInputDevice.MouseMove" />
        /// </summary>
        public event MouseEventHandler MouseMove;

        /// <summary>
        ///     <see cref="IInputDevice.MouseDown" />
        /// </summary>
        public event MouseEventHandler MouseDown;

        /// <summary>
        ///     <see cref="IInputDevice.MouseUp" />
        /// </summary>
        public event MouseEventHandler MouseUp;

        /// <summary>
        ///     <see cref="IInputDevice.MouseClick" />
        /// </summary>
        public event MouseEventHandler MouseClick;

        /// <summary>
        ///     <see cref="IInputDevice.MouseWheel" />
        /// </summary>
        public event MouseEventHandler MouseWheel;

        private readonly IWinFormsGameWindow _window;

        private MouseButtons _pressedMouseButtons = MouseButtons.None;

        private Point _mousePosition = Point.Empty;

        private readonly HashSet<int> _pressedKeys = new HashSet<int>();

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Constructors

        #region Statics

        #endregion

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

        #region Statics

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
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;
using Exomia.Framework.Platform.Windows.Win32;

namespace Exomia.Framework.Platform.Linux.Game.Desktop
{
    /// <summary>
    ///     Form for viewing the window forms game. This class cannot be inherited.
    /// </summary>
    sealed class GameWindow : IGameWindow
    {
        /// <inheritdoc />
        public event RefEventHandler<bool> FormClosing
        {
            add { _renderForm.FormClosing    += value; }
            remove { _renderForm.FormClosing -= value; }
        }

        /// <inheritdoc />
        public event EventHandler FormClosed
        {
            add { _renderForm.FormClosed    += value; }
            remove { _renderForm.FormClosed -= value; }
        }

        private readonly RenderForm _renderForm;
        private          bool       _isInitialized;

        /// <inheritdoc />
        public int Width
        {
            get { return _renderForm.Size.X; }
        }

        /// <inheritdoc />
        public int Height
        {
            get { return _renderForm.Size.Y; }
        }

        /// <inheritdoc />
        public string Title
        {
            get { return _renderForm.WindowTitle; }
            set { _renderForm.WindowTitle = value; }
        }

        /// <summary>
        ///     Gets the render form.
        /// </summary>
        /// <value>
        ///     The render form.
        /// </value>
        public RenderForm RenderForm
        {
            get { return _renderForm; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameWindow" /> class.
        /// </summary>
        /// <param name="title"> The title. </param>
        public GameWindow(string title)
        {
            _renderForm = new RenderForm(title);
        }

        /// <inheritdoc />
        public void Resize(int width, int height)
        {
            _renderForm.Resize(width, height);
        }

        /// <inheritdoc />
        public void Initialize(ref GameGraphicsParameters parameters)
        {
            if (_isInitialized) { return; }

            _renderForm.WindowState = parameters.DisplayType == DisplayType.FullscreenWindow
                ? RenderForm.FormWindowState.Maximized
                : RenderForm.FormWindowState.Normal;

            _renderForm.BorderStyle = parameters.DisplayType == DisplayType.Window
                ? RenderForm.FormBorderStyle.Fixed
                : RenderForm.FormBorderStyle.None;

            parameters.Handle = _renderForm.CreateWindow(parameters.Width, parameters.Height);

            if (parameters.DisplayType == DisplayType.FullscreenWindow)
            {
                User32.GetClientRect(parameters.Handle, out RECT rcRect);
                parameters.Width  = rcRect.RightBottom.X;
                parameters.Height = rcRect.RightBottom.Y;
            }

            bool isMouseVisible = parameters.IsMouseVisible;
            bool clipCursor     = parameters.ClipCursor;

            _renderForm.MouseEnter += hWnd =>
            {
                if (!isMouseVisible)
                {
                    User32.ShowCursor(false);
                }
                if (clipCursor)
                {
                    RECT rect = new RECT(Width, Height);
                    if (User32.ClientToScreen(hWnd, ref rect.LeftTop) &&
                        User32.ClientToScreen(hWnd, ref rect.RightBottom))
                    {
                        User32.ClipCursor(ref rect);
                    }
                }
            };

            _renderForm.MouseLeave += hWnd =>
            {
                if (!isMouseVisible)
                {
                    User32.ShowCursor(true);
                }
            };

            if (clipCursor)
            {
                RECT rect = new RECT(Width, Height);
                if (User32.ClientToScreen(parameters.Handle, ref rect.LeftTop) &&
                    User32.ClientToScreen(parameters.Handle, ref rect.RightBottom))
                {
                    User32.ClipCursor(ref rect);
                }
            }

            _isInitialized = true;
        }

        /// <inheritdoc />
        public void Show()
        {
            _renderForm.Show();
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
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
                    _renderForm.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="GameWindow" /> class.
        /// </summary>
        ~GameWindow()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
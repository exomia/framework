#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Win32;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Form for viewing the window forms game. This class cannot be inherited.
    /// </summary>
    sealed class WinFormsGameWindow : IWinFormsGameWindow, IGameWindowInitialize
    {
        private readonly RenderForm _renderForm;
        private          bool       _isInitialized;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width
        {
            get { return _renderForm.Size.X; }
        }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height
        {
            get { return _renderForm.Size.Y; }
        }

        /// <summary>
        ///     Occurs when the form is about to close.
        /// </summary>
        public event RefEventHandler<bool> FormClosing
        {
            add { _renderForm.FormClosing += value; }
            remove { _renderForm.FormClosing -= value; }
        }

        /// <summary>
        ///     Gets a value indicating whether this object is initialized.
        /// </summary>
        /// <value>
        ///     True if this object is initialized, false if not.
        /// </value>
        bool IGameWindowInitialize.IsInitialized
        {
            get { return _isInitialized; }
        }

        /// <inheritdoc />
        public RenderForm RenderForm
        {
            get { return _renderForm; }
        }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        public string Title
        {
            get { return _renderForm.WindowTitle; }
            set { _renderForm.WindowTitle = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WinFormsGameWindow" /> class.
        /// </summary>
        /// <param name="title"> The title. </param>
        public WinFormsGameWindow(string title)
        {
            _renderForm = new RenderForm(title);
        }

        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public void Resize(int width, int height)
        {
            _renderForm.Resize(width, height);
        }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void IGameWindowInitialize.Initialize(ref GameGraphicsParameters parameters)
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

        void IGameWindowInitialize.Show()
        {
            _renderForm.Show();
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
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
        ///     Finalizes an instance of the <see cref="WinFormsGameWindow" /> class.
        /// </summary>
        ~WinFormsGameWindow()
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
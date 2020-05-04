#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Exomia.Framework.Native;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Form for viewing the window forms game. This class cannot be inherited.
    /// </summary>
    public sealed class WinFormsGameWindow : IWinFormsGameWindow, IGameWindowInitialize
    {
        private readonly RenderForm _renderForm;
        private          bool       _isInitialized;

        /// <summary>
        ///     Gets the width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; private set; }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; private set; }

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
            get { return _renderForm.Text; }
            set { _renderForm.Text = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WinFormsGameWindow" /> class.
        /// </summary>
        /// <param name="title"> The title. </param>
        public WinFormsGameWindow(string title)
        {
            _renderForm = new RenderForm(title)
            {
                StartPosition = FormStartPosition.Manual, 
                Location = Point.Empty, 
                AutoScaleMode = AutoScaleMode.None
            };
        }
        
        /// <summary>
        ///     Resizes.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public void Resize(int width, int height)
        {
            if (_renderForm.InvokeRequired)
            {
                _renderForm.Invoke(
                    (MethodInvoker)delegate
                    {
                        Resize(width, height);
                    });
                return;
            }
            _renderForm.ClientSize = new Size(Width = width, Height = height);
        }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        void IGameWindowInitialize.Initialize(ref GameGraphicsParameters parameters)
        {
            if (_isInitialized) { return; }

            _renderForm.WindowState = parameters.DisplayType == DisplayType.FullscreenWindow
                ? FormWindowState.Maximized
                : FormWindowState.Normal;

            _renderForm.IsFullscreen = parameters.DisplayType == DisplayType.Fullscreen;
            _renderForm.FormBorderStyle = parameters.DisplayType == DisplayType.Window
                ? FormBorderStyle.Fixed3D
                : FormBorderStyle.None;

            _renderForm.ForceCreateHandle();

            while (!_renderForm.IsHandleCreated)
            {
                Thread.Sleep(1);
                Application.DoEvents();
            }

            parameters.Handle = _renderForm.Handle;

            if (parameters.DisplayType == DisplayType.FullscreenWindow)
            {
                parameters.Width  = Width  = _renderForm.ClientSize.Width;
                parameters.Height = Height = _renderForm.ClientSize.Height;
            }
            else
            {
                Resize(parameters.Width, parameters.Height);
            }

            bool isMouseVisible = parameters.IsMouseVisible;
            bool clipCursor     = parameters.ClipCursor;

            _renderForm.MouseEnter += (sender, e) =>
            {
                if (!isMouseVisible)
                {
                    Cursor.Hide();
                }
                if (clipCursor)
                {
                    User32.RECT rect = new User32.RECT(Width, Height);
                    if (User32.ClientToScreen(_renderForm.Handle, ref rect.LeftTop) &&
                        User32.ClientToScreen(_renderForm.Handle, ref rect.RightBottom))
                    {
                        User32.ClipCursor(ref rect);
                    }
                }
            };

            _renderForm.MouseLeave += (sender, e) =>
            {
                if (!isMouseVisible)
                {
                    Cursor.Show();
                }
            };

            if (clipCursor)
            {
                User32.RECT rect = new User32.RECT(Width, Height);
                if (User32.ClientToScreen(_renderForm.Handle, ref rect.LeftTop) &&
                    User32.ClientToScreen(_renderForm.Handle, ref rect.RightBottom))
                {
                    User32.ClipCursor(ref rect);
                }
            }

            _isInitialized = true;
        }

        void IGameWindowInitialize.Show()
        {
            _renderForm.Show();
            _renderForm.Activate();
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
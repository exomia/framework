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
using SharpDX;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Form for viewing the window forms game. This class cannot be inherited.
    /// </summary>
    public sealed class WinFormsGameWindow : IWinFormsGameWindow, IGameWindowInitialize
    {
        /// <summary>
        ///     The render form.
        /// </summary>
        private RenderForm _renderForm;

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
        public bool IsInitialized { get; private set; }

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
            set
            {
                if (_renderForm != null)
                {
                    _renderForm.Text = value;
                }
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WinFormsGameWindow" /> class.
        /// </summary>
        /// <param name="title"> The title. </param>
        public WinFormsGameWindow(string title)
        {
            _renderForm = new RenderForm(title) { FormBorderStyle = FormBorderStyle.FixedSingle };
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="WinFormsGameWindow" /> class.
        /// </summary>
        ~WinFormsGameWindow()
        {
            Dispose(false);
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
        public void Initialize(ref GameGraphicsParameters parameters)
        {
            if (IsInitialized) { return; }

            _renderForm.ClientSize = new Size(Width = parameters.Width, Height = parameters.Height);
            _renderForm.Show();
            _renderForm.Activate();

            if (!parameters.IsMouseVisible)
            {
                _renderForm.MouseEnter += (sender, e) => { Cursor.Hide(); };
                _renderForm.MouseLeave += (sender, e) => { Cursor.Show(); };
            }

            while (!_renderForm.IsHandleCreated)
            {
                Thread.Sleep(16);
                Application.DoEvents();
            }

            parameters.Handle = _renderForm.Handle;

            IsInitialized = true;
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
                    /* USER CODE */
                    Utilities.Dispose(ref _renderForm);
                }
                _disposed = true;
            }
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
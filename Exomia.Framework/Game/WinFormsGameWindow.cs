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
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;

namespace Exomia.Framework.Game
{
    /// <summary>
    ///     Form for viewing the window forms game. This class cannot be inherited.
    /// </summary>
    public sealed class WinFormsGameWindow : IWinFormsGameWindow
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
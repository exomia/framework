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

#pragma warning disable 1591

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;

namespace Exomia.Framework.Game
{
    public sealed class WinFormsGameWindow : IWinFormsGameWindow
    {
        private RenderForm _renderForm;

        public WinFormsGameWindow(string title)
        {
            _renderForm = new RenderForm(title) { FormBorderStyle = FormBorderStyle.FixedSingle };
        }

        ~WinFormsGameWindow()
        {
            Dispose(false);
        }

        public bool IsInitialized { get; private set; }

        public int Width { get; private set; }

        public int Height { get; private set; }

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

        public RenderForm RenderForm
        {
            get { return _renderForm; }
        }

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

        public void Resize(int width, int height)
        {
            if (_renderForm.InvokeRequired)
            {
                _renderForm.Invoke(
                    new MethodInvoker(
                        () =>
                        {
                            Resize(width, height);
                        }));
                return;
            }
            _renderForm.ClientSize = new Size(Width = width, Height = height);
        }

        #region IDisposable Support

        private bool _disposed;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
#pragma warning disable 1591

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Windows;

namespace Exomia.Framework.Game
{
    public sealed class WinFormsGameWindow : IWinFormsGameWindow, IDisposable
    {
        #region Variables

        #region Statics

        #endregion

        private RenderForm _renderForm;

        #endregion

        #region Constants

        #endregion

        #region Properties

        #region Statics

        #endregion

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

        #endregion

        #region Constructors

        #region Statics

        #endregion

        public WinFormsGameWindow(string title)
        {
            _renderForm = new RenderForm(title)
                { FormBorderStyle = FormBorderStyle.FixedSingle };
        }

        ~WinFormsGameWindow()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        #region Statics

        #endregion

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

        #endregion

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
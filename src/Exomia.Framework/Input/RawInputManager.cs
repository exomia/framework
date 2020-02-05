using System;
using System.Windows.Forms;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Manager for raw inputs. This class cannot be inherited.
    /// </summary>
    sealed class RawInputManager : IDisposable
    {
        /// <summary>
        ///     The key modifier.
        /// </summary>
        private KeyModifier _keyModifier = 0;

        /// <summary>
        ///     The raw input device.
        /// </summary>
        private IRawInputDevice _rawInputDevice;

        /// <summary>
        ///     True if this object is initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RawInputManager"/> class.
        /// </summary>
        public RawInputManager()
        {
            _rawInputDevice = null!;
        }

        /// <summary>
        ///     Gets or sets the raw input handler.
        /// </summary>
        /// <value>
        ///     The raw input handler.
        /// </value>
        public IRawInputHandler? RawInputHandler { get; set; }
        
        public void Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                _rawInputDevice = registry.GetService<IRawInputDevice>() ??
                                  throw new NullReferenceException($"No {nameof(IRawInputDevice)} found.");
                
                _rawInputDevice.RawKeyEvent   += RawInputDeviceOnRawKeyEvent;
                _rawInputDevice.RawMouseDown  += RawInputDeviceOnRawMouseDown;
                _rawInputDevice.RawMouseUp    += RawInputDeviceOnRawMouseUp;
                _rawInputDevice.RawMouseClick += RawInputDeviceOnRawMouseClick;
                _rawInputDevice.RawMouseMove  += RawInputDeviceOnRawMouseMove;
                _rawInputDevice.RawMouseWheel += RawInputDeviceOnRawMouseWheel;
                
                _isInitialized = true;
            }
        }

        /// <summary>
        ///     Raw input device on raw mouse wheel.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            RawInputHandler?.MouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse move.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            RawInputHandler?.MouseMove(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse up.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            RawInputHandler?.MouseUp(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse down.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            RawInputHandler?.MouseDown(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse click.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            RawInputHandler?.MouseClick(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw key event.
        /// </summary>
        /// <param name="e"> [in,out] The ref Message to process. </param>
        private void RawInputDeviceOnRawKeyEvent(ref Message e)
        {
            RawInputHandler?.KeyEvent(ref e);
            if (RawInputHandler is IInputHandler inputHandler)
            {
                int vKey = (int)e.WParam.ToInt64();

                switch (e.Msg)
                {
                    case Win32Message.WM_KEYDOWN:
                        switch (vKey)
                        {
                            case Key.ShiftKey:
                                _keyModifier |= KeyModifier.Shift;
                                break;
                            case Key.ControlKey:
                                _keyModifier |= KeyModifier.Control;
                                break;
                        }
                        inputHandler.KeyDown(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_KEYUP:
                        switch (vKey)
                        {
                            case Key.ShiftKey:
                                _keyModifier &= ~KeyModifier.Shift;
                                break;
                            case Key.ControlKey:
                                _keyModifier &= ~KeyModifier.Control;
                                break;
                        }
                        inputHandler.KeyUp(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_SYSKEYDOWN:
                        if (vKey == Key.Menu)
                        {
                            _keyModifier |= KeyModifier.Alt;
                        }
                        inputHandler.KeyDown(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_SYSKEYUP:
                        if (vKey == Key.Menu)
                        {
                            _keyModifier &= ~KeyModifier.Alt;
                        }
                        inputHandler.KeyUp(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_UNICHAR:
                    case Win32Message.WM_CHAR:
                        inputHandler.KeyPress((char)vKey);
                        break;
                }
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Input.RawInputManager and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">  to release both managed and unmanaged resources;  to release only unmanaged resources. </param>
        void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _rawInputDevice!.RawKeyEvent  -= RawInputDeviceOnRawKeyEvent;
                    _rawInputDevice.RawMouseDown  -= RawInputDeviceOnRawMouseDown;
                    _rawInputDevice.RawMouseUp    -= RawInputDeviceOnRawMouseUp;
                    _rawInputDevice.RawMouseMove  -= RawInputDeviceOnRawMouseMove;
                    _rawInputDevice.RawMouseWheel -= RawInputDeviceOnRawMouseWheel;
                }
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        ~RawInputManager()
        {
            Dispose(false);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
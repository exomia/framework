#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.UI.Controls;
using SharpDX;

namespace Exomia.Framework.UI
{
    /// <summary>
    ///     A ui manager. This class cannot be inherited.
    /// </summary>
    public sealed class UiManager : IComponent, IInitializable, IDrawable, IDisposable, IInputHandler
    {
        internal const int INITIAL_LIST_SIZE = 8;

        /// <summary>
        ///     Occurs when the <see cref="DrawOrder" /> property changes.
        /// </summary>
        public event EventHandler? DrawOrderChanged;

        /// <summary>
        ///     Occurs when the <see cref="Visible" /> property changes.
        /// </summary>
        public event EventHandler? VisibleChanged;
        
        private bool _isInitialized;

        private readonly DisposeCollector _collector;
        private          int              _drawOrder;
        private          bool             _visible;

        private Control[] _controls;
        private Control[] _currentlyControls;
        private int       _controlCount;
        private int       _currentlyControlCount;

        private Canvas _canvas = null!;

        private bool _isDirty;

        private IKeyListener? _keyListener;
        private Control?      _focusedControl;
        private Control?      _enteredControl;

        /// <inheritdoc />
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke();
                }
            }
        }

        /// <summary>
        ///     Gets or sets the position the <see cref="IInputHandler"/> should be using while registering the callbacks.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         e.g. <see cref="IInputDevice.RegisterRawKeyEvent" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         e.g. <see cref="IInputDevice.RegisterRawKeyEvent" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        public int InputHandlerInsertPosition { get; set; } = 0;

        /// <summary>
        ///     Gets the input handler.
        /// </summary>
        /// <value>
        ///     The input handler.
        /// </value>
        public IInputHandler InputHandler
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return this; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UiManager" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public UiManager(string name)
        {
            Name               = name ?? throw new ArgumentNullException(nameof(name));
            _collector         = new DisposeCollector();
            _controls          = new Control[INITIAL_LIST_SIZE];
            _currentlyControls = new Control[INITIAL_LIST_SIZE];
        }

        void IInputHandler.RegisterInput(IInputDevice device)
        {
            device.RegisterMouseMove(MouseMove, InputHandlerInsertPosition);
            device.RegisterMouseDown(MouseDown, InputHandlerInsertPosition);
            device.RegisterMouseUp(MouseUp, InputHandlerInsertPosition);

            device.RegisterKeyDown(KeyDown, InputHandlerInsertPosition);
            device.RegisterKeyUp(KeyUp, InputHandlerInsertPosition);
            device.RegisterKeyPress(KeyPress, InputHandlerInsertPosition);
        }

        void IInputHandler.UnregisterInput(IInputDevice device)
        {
            device.UnregisterMouseMove(MouseMove);
            device.UnregisterMouseDown(MouseDown);
            device.UnregisterMouseUp(MouseUp);

            device.UnregisterKeyDown(KeyDown);
            device.UnregisterKeyUp(KeyUp);
            device.UnregisterKeyPress(KeyPress);
        }
        
        bool IDrawable.BeginDraw()
        {
            return _visible;
        }
        
        void IDrawable.Draw(GameTime gameTime)
        {
            if (_isDirty)
            {
                lock (_controls)
                {
                    if (_controlCount >= _currentlyControls.Length)
                    {
                        Array.Resize(ref _currentlyControls, _currentlyControls.Length * 2);
                    }
                    else if (_controlCount < _currentlyControlCount)
                    {
                        Array.Clear(_currentlyControls, _controlCount, _currentlyControlCount - _controlCount);
                    }
                    Array.Copy(_controls, _currentlyControls, _currentlyControlCount = _controlCount);
                }
                _isDirty = false;
            }

            _canvas.Begin();

            for (int i = _currentlyControlCount - 1; i >= 0; i--)
            {
                Control control = _currentlyControls[i];
                if (control.BeginDraw())
                {
                    control.Draw(gameTime.DeltaTimeS, _canvas);
                    control.EndDraw();
                }
            }

            _canvas.End();
        }
        
        void IDrawable.EndDraw() { }

        /// <summary>
        ///     Adds the <paramref name="control" /> to this ui manger.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="control"> The control to add. </param>
        /// <returns>The <paramref name="control"/></returns>
        public T Add<T>(T control) where T : Control
        {
            if (control.GetUiManager() != null || control._parent != null)
            {
                throw new InvalidOperationException(
                    $"The control can't be added to the {nameof(UiManager)} instance! It's already part of an other manager or container!");
            }

            control.SetUiManager(this);
            lock (_controls)
            {
                if (_controlCount >= _controls.Length)
                {
                    Array.Resize(ref _controls, _controls.Length * 2);
                }
                _controls[control._uiListIndex = _controlCount++] = control;
            }

            _isDirty = true;

            return _collector.Collect(control);
        }

        /// <summary>
        ///     Removes the given <paramref name="control" /> from this ui manager.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="control"> The control to remove. </param>
        /// <param name="dispose"> True to dispose the control after removing. </param>
        /// <returns>The <paramref name="control"/></returns>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public T Remove<T>(T control, bool dispose = false) where T : Control
        {
            if (control._parent != null)
            {
                throw new InvalidOperationException(
                    $"The control doesn't belongs to this {nameof(UiManager)} instance.");
            }

            RemoveAt(control._uiListIndex);

            _collector.Remove(control);
            
            if (dispose)
            {
                control.Dispose();
            }
            
            return control;
        }

        /// <summary>
        ///     Removes the control at the described <paramref name="index" /> from this ui manager.
        /// </summary>
        /// <param name="index"> Zero-based index of the control to remove. </param>
        public void RemoveAt(int index)
        {
            lock (_controls)
            {
                Control control = _controls[index];
                control._parent = null;
                control.SetUiManager(null);

                _controls[index]              = _controls[--_controlCount];
                _controls[index]._uiListIndex = index;
                _controls[_controlCount]      = null!;
            }

            _isDirty = true;
        }

        /// <summary>
        ///     Clears the control list to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            lock (_controls)
            {
                _controlCount = 0;
                Array.Clear(_controls, 0, _controls.Length);
            }

            _isDirty = true;
        }

        /// <inheritdoc />
        void IInitializable.Initialize(IServiceRegistry registry)
        {
            if (!_isInitialized)
            {
                _canvas        = new Canvas(registry.GetService<IGraphicsDevice>());
                _isInitialized = true;
            }
        }

        internal void SetFocusedControl(Control control, bool focus)
        {
            if (focus)
            {
                Interlocked.Exchange(ref _focusedControl, control)?.InternalSetFocus(false);

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (control is IKeyListener listener)
                {
                    Interlocked.Exchange(ref _keyListener, listener);
                }
            }
            else
            {
                Interlocked.Exchange(ref _focusedControl, null)?.InternalSetFocus(false);
                Interlocked.Exchange(ref _keyListener, null);
            }

            control.InternalSetFocus(focus);
        }

        internal void SetEnteredControl(Control control, in MouseEventArgs e)
        {
            Interlocked.Exchange(ref _enteredControl, control)?.InternalSetMouseEntered(false, in e);
            control.InternalSetMouseEntered(true, e);
        }

        private EventAction KeyDown(int keyValue, KeyModifier modifiers)
        {
            if (_keyListener != null)
            {
                _keyListener.KeyDown(keyValue, modifiers);
                return EventAction.StopPropagation;
            }
            return EventAction.Continue;
        }

        private EventAction KeyPress(char key)
        {
            if (_keyListener != null)
            {
                _keyListener.KeyPress(key);
                return EventAction.StopPropagation;
            }
            return EventAction.Continue;
        }

        private EventAction KeyUp(int keyValue, KeyModifier modifiers)
        {
            if (_keyListener != null)
            {
                _keyListener.KeyUp(keyValue, modifiers);
                return EventAction.StopPropagation;
            }
            return EventAction.Continue;
        }

        private EventAction MouseMove(in MouseEventArgs e)
        {
            EventAction eventAction = EventAction.Continue;
            if (_enteredControl == null)
            {
                lock (_controls)
                {
                    for (int i = _controlCount - 1; i >= 0; i--)
                    {
                        if (_controls[i].Enabled && _controls[i].InternalMouseMove(in e, ref eventAction))
                        {
                            return eventAction;
                        }
                    }
                }
            }
            else
            {
                if (_enteredControl.Enabled && _enteredControl.InternalMouseMove(in e, ref eventAction))
                {
                    return eventAction;
                }
                Interlocked.Exchange(ref _enteredControl, _enteredControl._parent)
                           ?.InternalSetMouseEntered(false, in e);

                while (_enteredControl != null)
                {
                    if (_enteredControl.Enabled && _enteredControl.InternalMouseMove(in e, ref eventAction))
                    {
                        return eventAction;
                    }
                    Interlocked.Exchange(ref _enteredControl, _enteredControl._parent);
                }
            }
            return EventAction.Continue;
        }

        private EventAction MouseDown(in MouseEventArgs e)
        {
            EventAction eventAction = EventAction.StopPropagation;
            if (_enteredControl != null && _enteredControl.Enabled &&
                _enteredControl.InternalMouseDown(in e, ref eventAction))
            {
                return eventAction;
            }

            return EventAction.Continue;
        }

        private EventAction MouseUp(in MouseEventArgs e)
        {
            EventAction eventAction = EventAction.StopPropagation;
            if (_enteredControl != null && _enteredControl.Enabled &&
                _enteredControl.InternalMouseUp(in e, ref eventAction))
            {
                return eventAction;
            }

            return EventAction.Continue;
        }

        #region IDisposable Support

        private bool _disposed;

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        /// <param name="disposing"> true if user code; false called by finalizer. </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _collector.DisposeAndClear(disposing);
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~UiManager()
        {
            Dispose(false);
        }

        #endregion
    }
}
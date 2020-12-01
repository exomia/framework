#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Threading;
using Exomia.Framework.Game;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.Mathematics;
using Exomia.Framework.UI.Controls;

namespace Exomia.Framework.UI
{
    /// <summary>
    ///     A ui manager. This class cannot be inherited.
    /// </summary>
    public sealed class UiManager : Renderer, IInputHandler
    {
        internal const int INITIAL_LIST_SIZE = 8;

        private const float DEFAULT_FREQUENCY = 30.0f;

        private Control[] _controls;
        private Control[] _currentlyControls;
        private int       _controlCount;
        private int       _currentlyControlCount;

        private float _frequency = DEFAULT_FREQUENCY;
        private float _cycleTime = 1f / DEFAULT_FREQUENCY;
        private int   _drawedThisCycle;

        private Canvas _canvas = null!;

        private IKeyListener? _keyListener;
        private Control?      _focusedControl;
        private Control?      _enteredControl;

        /// <summary>
        ///     Gets or sets the frequency.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <value>
        ///     The frequency.
        /// </value>
        public float Frequency
        {
            get { return _frequency; }
            set
            {
                if (value < 1.0f)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(value), "Frequency must be greater or equal than 1.0f.");
                }

                _frequency = value;
                _cycleTime = 1f / _frequency;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UiManager" /> class.
        /// </summary>
        /// <param name="name"> The name. </param>
        public UiManager(string name)
            : base(name)
        {
            _controls          = new Control[INITIAL_LIST_SIZE];
            _currentlyControls = new Control[INITIAL_LIST_SIZE];
        }

        void IInputHandler.RegisterInput(IInputDevice device)
        {
            device.RegisterMouseMove(MouseMove);
            device.RegisterMouseDown(MouseDown);
            device.RegisterMouseUp(MouseUp);

            device.RegisterKeyDown(KeyDown);
            device.RegisterKeyUp(KeyUp);
            device.RegisterKeyPress(KeyPress);
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

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            if (_currentlyControlCount == 0)
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
                _canvas.Begin();
            }

            int remaining = _currentlyControlCount - _drawedThisCycle;
            int toDraw = Math.Min(
                remaining, Math2.Ceiling((gameTime.DeltaTimeS / _cycleTime) * _currentlyControlCount));

            if (toDraw > 0)
            {
                for (int i = (_drawedThisCycle + toDraw) - 1; i >= _drawedThisCycle; i--)
                {
                    Control control = _currentlyControls[i];
                    if (control.BeginDraw())
                    {
                        control.Draw(_cycleTime, _canvas);
                        control.EndDraw();
                    }
                }
                _drawedThisCycle += toDraw;
            }

            if (_drawedThisCycle >= _currentlyControlCount)
            {
                _drawedThisCycle       = 0;
                _currentlyControlCount = 0;
                _canvas.End();
            }
        }

        /// <summary>
        ///     Adds the <paramref name="control" /> to this ui manger.
        /// </summary>
        /// <param name="control"> The control to add. </param>
        public void Add(Control control)
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
        }

        /// <summary>
        ///     Removes the given <paramref name="control" /> from this ui manager.
        /// </summary>
        /// <param name="control"> The control to remove. </param>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void Remove(Control control)
        {
            if (control._parent != null)
            {
                throw new InvalidOperationException(
                    $"The control doesn't belongs to this {nameof(UiManager)} instance.");
            }

            RemoveAt(control._uiListIndex);
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
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _canvas = new Canvas(registry.GetService<IGraphicsDevice>());
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
    }
}
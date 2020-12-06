#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A container.
    /// </summary>
    public class Container : Control
    {
        private Control[] _controls;
        private Control[] _currentlyControls;
        private int       _controlCount;
        private int       _currentlyControlCount;

        /// <inheritdoc />
        public Container()
        {
            _controls          = new Control[UiManager.INITIAL_LIST_SIZE];
            _currentlyControls = new Control[UiManager.INITIAL_LIST_SIZE];
        }

        /// <summary>
        ///     Adds the <paramref name="control" /> to this container.
        /// </summary>
        /// <param name="control"> The control to add. </param>
        public void Add(Control control)
        {
            if (control._parent != null || control.GetUiManager() != null)
            {
                throw new InvalidOperationException(
                    $"The control can't be added to the {nameof(Container)} instance! It's already part of an other container or manager!");
            }

            control.SetUiManager(GetUiManager());
            control._parent = this;
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
        ///     Removes the given <paramref name="control" /> from this container.
        /// </summary>
        /// <param name="control"> The control to remove. </param>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        public void Remove(Control control)
        {
            if (control._parent != this)
            {
                throw new InvalidOperationException(
                    $"The control doesn't belongs to this {nameof(Container)} instance.");
            }

            RemoveAt(control._uiListIndex);
        }

        /// <summary>
        ///     Removes the control at the described <paramref name="index" /> from this container.
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
        public override void Draw(float elapsedSeconds, Canvas canvas)
        {
            base.Draw(elapsedSeconds, canvas);

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

            for (int i = _currentlyControlCount - 1; i >= 0; i--)
            {
                Control control = _controls[i];
                if (control.BeginDraw())
                {
                    control.Draw(elapsedSeconds, canvas);
                    control.EndDraw();
                }
            }
        }

        /// <inheritdoc />
        internal override void SetUiManager(UiManager? manager)
        {
            lock (_controls)
            {
                for (int i = _controlCount - 1; i >= 0; i--)
                {
                    _controls[i].SetUiManager(manager);
                }
            }
            base.SetUiManager(manager);
        }

        /// <inheritdoc />
        internal override bool InternalMouseMove(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_visibleRectangle.Contains(e.Position))
            {
                lock (_controls)
                {
                    for (int i = _controlCount - 1; i >= 0; i--)
                    {
                        if (_controls[i].Enabled && _controls[i].InternalMouseMove(in e, ref eventAction))
                        {
                            return true;
                        }
                    }
                }

                OnMouseMove(in e, ref eventAction);
                if (!_isMouseEntered)
                {
                    _uiManager!.SetEnteredControl(this, in e);
                }
                return true;
            }

            return false;
        }
    }
}
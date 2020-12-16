#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using Exomia.Framework.Graphics;
using Exomia.Framework.Input;
using Exomia.Framework.UI.Brushes;
using SharpDX;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A checkbox.
    /// </summary>
    public class Checkbox : Control
    {
        /// <summary>
        ///     Occurs when Checked Changed.
        /// </summary>
        public event EventHandler<Checkbox>? CheckedChanged;

        private bool       _isMouseDown;
        private bool       _checked;
        private IBrush?    _checkedBrush;
        private RectangleF _checkedRectangle;

        /// <summary>
        ///     Gets or sets a value indicating whether the checkbox is checked or not.
        /// </summary>
        /// <value>
        ///     True if checked, false if not.
        /// </value>
        public bool Checked
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _checked; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (_checked != value)
                {
                    _checked = value;
                    CheckedChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the checked brush this brush is used if the <see cref="Checked" /> is true.
        /// </summary>
        /// <value>
        ///     The checked brush.
        /// </value>
        public IBrush? CheckedBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _checkedBrush; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _checkedBrush = value; }
        }

        /// <inheritdoc />
        public override void Draw(float elapsedSeconds, Canvas canvas)
        {
            base.Draw(elapsedSeconds, canvas);

            if (_checked)
            {
                _checkedBrush?.Render(canvas, in _checkedRectangle, _opacity);
            }
        }

        /// <inheritdoc />
        protected override void OnMouseDown(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (!_isMouseDown)
            {
                _isMouseDown = true;
            }
            base.OnMouseDown(in e, ref eventAction);
        }

        /// <inheritdoc />
        protected override void OnMouseUp(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                _checked     = !_checked;     //we don't use the setter here because we can skip the if check!
                CheckedChanged?.Invoke(this); // don't forget to fire the event!
            }
            base.OnMouseUp(in e, ref eventAction);
        }

        /// <inheritdoc />
        protected override void OnMouseLeaved(in MouseEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseLeaved(in e);
        }

        /// <inheritdoc />
        protected override void OnDrawRectangleChanged()
        {
            _checkedRectangle.X = _drawRectangle.X + _padding.W;
            _checkedRectangle.Y = _drawRectangle.Y + _padding.N;
            _checkedRectangle.Width = Math.Min(
                _drawRectangle.Width - _padding.W - _padding.E, _visibleRectangle.Width - _padding.W);
            _checkedRectangle.Height = Math.Min(
                _drawRectangle.Height - _padding.N - _padding.S, _visibleRectangle.Height - _padding.N);
        }
    }
}
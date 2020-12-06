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
using SharpDX;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A Slider.
    /// </summary>
    public class Slider : Control
    {
        /// <summary>
        ///     Occurs when Value Changed.
        /// </summary>
        public event EventHandler<Slider>? ValueChanged;

        private bool _isDirty          = true;
        private int  _value            = 0;
        private int  _minValue         = 0;
        private int  _maxValue         = 100;
        private int  _sliderCaretWidth = 5;
        private bool _isMouseDown      = false;

        private Brushes.IBrush? _sliderTrackBrush;
        private Brushes.IBrush? _sliderCaretBrush;

        private RectangleF _sliderTrackRectangle;
        private RectangleF _sliderCaretRectangle;

        /// <summary>
        ///     Gets or sets the value.
        /// </summary>
        /// <value>
        ///     The value.
        /// </value>
        public int Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                value = MathUtil.Clamp(value, _minValue, _maxValue);
                if (_value != value)
                {
                    _value   = value;
                    _isDirty = true;
                    ValueChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        ///     Gets or sets the minimum value.
        /// </summary>
        /// <value>
        ///     The minimum value.
        /// </value>
        public int MinValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _minValue; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                value = Math.Max(value, 0);
                if (_minValue != value && _minValue < _maxValue)
                {
                    _minValue = value;
                    _isDirty  = true;
                }
            }
        }

        /// <summary>
        ///     Gets or set the maximum value.
        /// </summary>
        /// <value>
        ///     The maximum value.
        /// </value>
        public int MaxValue
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                value = Math.Max(value, 0);
                if (_maxValue != value && _maxValue > _minValue)
                {
                    _maxValue = value;
                    _isDirty  = true;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the slider track brush.
        /// </summary>
        /// <value>
        ///     The slider track brush.
        /// </value>
        public Brushes.IBrush? SliderTrackBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _sliderTrackBrush; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _sliderTrackBrush = value; }
        }

        /// <summary>
        ///     Gets or sets the slider caret brush.
        /// </summary>
        /// <value>
        ///     The slider caret brush.
        /// </value>
        public Brushes.IBrush? SliderCaretBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _sliderCaretBrush; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _sliderCaretBrush = value; }
        }

        /// <summary>
        ///     Gets or sets the width of the slider caret.
        /// </summary>
        /// <value>
        ///     The width of the slider caret.
        /// </value>
        public int SliderCaretWidth
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _sliderCaretWidth; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                value = Math.Max(value, 1);
                if (_sliderCaretWidth != value)
                {
                    _sliderCaretWidth = value;
                    _isDirty          = true;
                }
            }
        }

        /// <inheritdoc />
        public override void Draw(float elapsedSeconds, Canvas canvas)
        {
            base.Draw(elapsedSeconds, canvas);

            if (_isDirty)
            {
                _sliderCaretRectangle.X = _sliderTrackRectangle.X +
                                          (((_sliderTrackRectangle.Width - _sliderCaretWidth) /
                                            (_maxValue - _minValue)) * _value);
                _sliderCaretRectangle.Width = _sliderCaretWidth;

                _isDirty = true;
            }

            _sliderTrackBrush?.Render(canvas, in _sliderTrackRectangle, _opacity);
            _sliderCaretBrush?.Render(canvas, in _sliderCaretRectangle, _opacity);
        }

        /// <inheritdoc />
        protected override void OnDrawRectangleChanged()
        {
            _sliderTrackRectangle.X      = _drawRectangle.X + _padding.W;
            _sliderTrackRectangle.Y      = _drawRectangle.Y + _padding.N;
            _sliderTrackRectangle.Width  = Math.Min(_drawRectangle.Width - _padding.W - _padding.E, _visibleRectangle.Width - _padding.W);
            _sliderTrackRectangle.Height = Math.Min(_drawRectangle.Height - _padding.N - _padding.S, _visibleRectangle.Height - _padding.N);

            _sliderCaretRectangle.Y      = _drawRectangle.Y;
            _sliderCaretRectangle.Height = _drawRectangle.Height;
        }

        /// <inheritdoc />
        protected override void OnMouseDown(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (!_isMouseDown && _sliderCaretRectangle.Contains(e.X, e.Y))
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
        protected override void OnMouseMove(in MouseEventArgs e, ref EventAction eventAction)
        {
            if (_isMouseDown)
            {
                eventAction = EventAction.StopPropagation;
                Value = (int)Math.Round(
                    (_maxValue - _minValue) * (1.0f / _sliderTrackRectangle.Width) * (e.X - _sliderTrackRectangle.X));
            }
            base.OnMouseMove(in e, ref eventAction);
        }
    }
}
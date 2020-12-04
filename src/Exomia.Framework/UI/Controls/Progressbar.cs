#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Graphics;
using Exomia.Framework.UI.Brushes;
using SharpDX;

namespace Exomia.Framework.UI.Controls
{
    /// <summary>
    ///     A progressbar.
    /// </summary>
    public class Progressbar : Control
    {
        /// <summary>
        ///     Occurs when Checked Changed.
        /// </summary>
        public event EventHandler<Progressbar>? ValueChanged;

        private bool       _isDirty = true;
        private float      _value;
        private IBrush?    _barBrush;
        private RectangleF _barRectangle;

        /// <summary>
        ///     Gets the value [0.0f, 1.0f] indicating the percentage this progressbar is filled.
        /// </summary>
        /// <value>
        ///     The value in the range from 0 to 1.
        /// </value>
        public float Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _value; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (_value != value)
                {
                    _value   = MathUtil.Clamp(value, 0.0f, 1.0f);
                    _isDirty = true;
                    ValueChanged?.Invoke(this);
                }
            }
        }

        /// <summary>
        ///     Gets the bar brush.
        /// </summary>
        /// <value>
        ///     The bar brush.
        /// </value>
        public IBrush? BarBrush
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return _barBrush; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { _barBrush = value; }
        }

        /// <inheritdoc />
        public override bool BeginDraw()
        {
            _isDirty |= IsDirty;
            return base.BeginDraw();
        }

        /// <inheritdoc />
        public override void Draw(float elapsedSeconds, Canvas canvas)
        {
            base.Draw(elapsedSeconds, canvas);

            if (_isDirty)
            {
                _barRectangle.X      = _drawRectangle.X + _padding.W;
                _barRectangle.Y      = _drawRectangle.Y + _padding.N;
                _barRectangle.Width  = (_drawRectangle.Width - _padding.W - _padding.E) * _value;
                _barRectangle.Height = _drawRectangle.Height - _padding.N - _padding.S;
            }

            _barBrush?.Render(canvas, _barRectangle, _opacity);
        }
    }
}
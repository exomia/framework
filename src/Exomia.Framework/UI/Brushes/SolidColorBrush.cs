#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.UI.Brushes
{
    /// <summary>
    ///     A solid color brush. This class cannot be inherited.
    /// </summary>
    public sealed class SolidColorBrush : IBrush
    {
        private readonly Color _color;
        private readonly float _opacity;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SolidColorBrush" /> class.
        /// </summary>
        /// <param name="color">   The color. </param>
        /// <param name="opacity"> (Optional) The opacity. </param>
        public SolidColorBrush(Color color, float opacity = 1.0f)
        {
            _color   = color;
            _opacity = opacity;
        }

        void IBrush.Render(Canvas canvas, RectangleF region, float opacity)
        {
            canvas.DrawFillRectangle(region, _color, 0, Vector2.Zero, _opacity * opacity);
        }
    }
}
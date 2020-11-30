#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.UI.Brushes
{
    /// <summary>
    ///     A border brush.
    /// </summary>
    public class BorderBrush : IBrush
    {
        /// <summary>
        ///     Bitfield of flags for specifying Options.
        /// </summary>
        [Flags]
        public enum Options : byte
        {
            /// <summary>
            ///     A binary constant representing the north flag.
            /// </summary>
            North = (byte)0x01,

            /// <summary>
            ///     A binary constant representing the south flag.
            /// </summary>
            South = (byte)0x02,

            /// <summary>
            ///     A binary constant representing the west flag.
            /// </summary>
            West = (byte)0x04,

            /// <summary>
            ///     A binary constant representing the east flag.
            /// </summary>
            East = (byte)0x08,

            /// <summary>
            ///     A binary constant representing all flag.
            /// </summary>
            All = North | South | West | East
        }

        private readonly Color   _color;
        private readonly float   _lineWidth;
        private readonly Options _options;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BorderBrush" /> class.
        /// </summary>
        /// <param name="color">     The color. </param>
        /// <param name="lineWidth"> (Optional) Width of the line. </param>
        /// <param name="options">   (Optional) Options for controlling the operation. </param>
        public BorderBrush(Color color, float lineWidth = 1.0f, Options options = Options.All)
        {
            _color     = color;
            _lineWidth = lineWidth;
            _options   = options;
        }

        void IBrush.Render(Canvas canvas, RectangleF region, float opacity)
        {
            if (!_color.Equals(Color.Transparent))
            {
                if (_options == Options.All)
                {
                    canvas.DrawRectangle(region, _color, _lineWidth, 0, Vector2.Zero, opacity);
                    return;
                }

                if ((_options & Options.North) == Options.North)
                {
                    canvas.DrawLine(region.TopLeft, region.TopRight, _color, _lineWidth, 0, Vector2.Zero, opacity);
                }
                if ((_options & Options.South) == Options.South)
                {
                    canvas.DrawLine(
                        region.BottomLeft, region.BottomRight, _color, _lineWidth, 0, Vector2.Zero, opacity);
                }
                if ((_options & Options.West) == Options.West)
                {
                    canvas.DrawLine(region.TopLeft, region.BottomLeft, _color, _lineWidth, 0, Vector2.Zero, opacity);
                }
                if ((_options & Options.East) == Options.East)
                {
                    canvas.DrawLine(region.TopRight, region.BottomRight, _color, _lineWidth, 0, Vector2.Zero, opacity);
                }
            }
        }
    }
}
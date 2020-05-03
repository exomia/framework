#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Additional information for mouse events.
    /// </summary>
    public readonly ref struct MouseEventArgs
    {
        /// <summary>
        ///     The X coordinate.
        /// </summary>
        public readonly int X;

        /// <summary>
        ///     The Y coordinate.
        /// </summary>
        public readonly int Y;

        /// <summary>
        ///     The buttons.
        /// </summary>
        public readonly MouseButtons Buttons;

        /// <summary>
        ///     The clicks.
        /// </summary>
        public readonly int Clicks;

        /// <summary>
        ///     The wheel delta.
        /// </summary>
        public readonly int WheelDelta;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MouseEventArgs" /> struct.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheel delta. </param>
        public MouseEventArgs(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            X          = x;
            Y          = y;
            Buttons    = buttons;
            Clicks     = clicks;
            WheelDelta = wheelDelta;
        }
    }
}
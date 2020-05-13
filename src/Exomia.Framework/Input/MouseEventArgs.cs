#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Additional information for mouse events.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public readonly ref struct MouseEventArgs
    {
        /// <summary>
        ///     The X coordinate.
        /// </summary>
        [FieldOffset(0)]
        public readonly int X;

        /// <summary>
        ///     The Y coordinate.
        /// </summary>
        [FieldOffset(4)]
        public readonly int Y;

        /// <summary>
        ///     The position.
        /// </summary>
        [FieldOffset(0)]
        public readonly Index2 Position;

        /// <summary>
        ///     The buttons.
        /// </summary>
        [FieldOffset(8)]
        public readonly MouseButtons Buttons;

        /// <summary>
        ///     The clicks.
        /// </summary>
        [FieldOffset(12)]
        public readonly int Clicks;

        /// <summary>
        ///     The wheel delta.
        /// </summary>
        [FieldOffset(16)]
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
            Position   = Index2.Zero;
            X          = x;
            Y          = y;
            Buttons    = buttons;
            Clicks     = clicks;
            WheelDelta = wheelDelta;
        }
    }
}
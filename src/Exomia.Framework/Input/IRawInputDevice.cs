#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Windows.Forms;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Interface for raw input device.
    /// </summary>
    interface IRawInputDevice
    {
        /// <summary>
        ///     Occurs when raw mouse down.
        /// </summary>
        event MouseEventHandler? RawMouseDown;

        /// <summary>
        ///     Occurs when raw mouse up.
        /// </summary>
        event MouseEventHandler? RawMouseUp;

        /// <summary>
        ///     Occurs when raw mouse down.
        /// </summary>
        event MouseEventHandler? RawMouseClick;

        /// <summary>
        ///     Occurs when raw mouse move.
        /// </summary>
        event MouseEventHandler? RawMouseMove;

        /// <summary>
        ///     Occurs when raw mouse wheel.
        /// </summary>
        event MouseEventHandler? RawMouseWheel;

        /// <summary>
        ///     Occurs when raw key event.
        /// </summary>
        event RefEventHandler<Message>? RawKeyEvent;
    }
}
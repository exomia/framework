#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Windows.Forms;
using Exomia.Framework.Game;
using SharpDX.RawInput;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Delegate for handling RKey events.
    /// </summary>
    /// <param name="key">              The key. </param>
    /// <param name="state">            The state. </param>
    /// <param name="extraInformation"> Information describing the extra. </param>
    /// <param name="sKeys">            The keys. </param>
    public delegate void RKeyEventHandler(int key, KeyState state, int extraInformation, RSpecialKeys sKeys);

    /// <summary>
    ///     Delegate for handling RMouse events.
    /// </summary>
    /// <param name="x">          The x coordinate. </param>
    /// <param name="y">          The y coordinate. </param>
    /// <param name="buttons">    The buttons. </param>
    /// <param name="clicks">     The clicks. </param>
    /// <param name="wheelDelta"> The wheel delta. </param>
    public delegate void RMouseEventHandler(int x, int y, RMouseButtons buttons, int clicks, int wheelDelta);

    /// <summary>
    ///     Bitfield of flags for specifying RMouseButtons.
    /// </summary>
    [Flags]
    public enum RMouseButtons
    {
        /// <summary>
        ///     A binary constant representing the left flag.
        /// </summary>
        Left = 1 << 0,

        /// <summary>
        ///     A binary constant representing the middle flag.
        /// </summary>
        Middle = 1 << 1,

        /// <summary>
        ///     A binary constant representing the right flag.
        /// </summary>
        Right = 1 << 2,

        /// <summary>
        ///     A binary constant representing the button 4 flag.
        /// </summary>
        Button4 = 1 << 3,

        /// <summary>
        ///     A binary constant representing the button 5 flag.
        /// </summary>
        Button5 = 1 << 4
    }

    /// <summary>
    ///     Bitfield of flags for specifying RSpecialKeys.
    /// </summary>
    [Flags]
    public enum RSpecialKeys
    {
        /// <summary>
        ///     A binary constant representing the none flag.
        /// </summary>
        None = 1 << 0,

        /// <summary>
        ///     A binary constant representing the shift flag.
        /// </summary>
        Shift = 1 << 1,

        /// <summary>
        ///     A binary constant representing the Alternate flag.
        /// </summary>
        Alt = 1 << 2,

        /// <summary>
        ///     A binary constant representing the control flag.
        /// </summary>
        Control = 1 << 3
    }

    /// <summary>
    ///     Interface for raw input device.
    /// </summary>
    public interface IRawInputDevice
    {
        /// <summary>
        ///     Occurs when Key Down.
        /// </summary>
        event RKeyEventHandler KeyDown;

        /// <summary>
        ///     Occurs when Key Press.
        /// </summary>
        event RKeyEventHandler KeyPress;

        /// <summary>
        ///     Occurs when Key Up.
        /// </summary>
        event RKeyEventHandler KeyUp;

        /// <summary>
        ///     Occurs when Mouse Down.
        /// </summary>
        event RMouseEventHandler MouseDown;

        /// <summary>
        ///     Occurs when Mouse Move.
        /// </summary>
        event RMouseEventHandler MouseMove;

        /// <summary>
        ///     Occurs when Mouse Up.
        /// </summary>
        event RMouseEventHandler MouseUp;

        /// <summary>
        ///     Gets information describing the wheel.
        /// </summary>
        /// <value>
        ///     Information describing the wheel.
        /// </value>
        int WheelData { get; }

        /// <summary>
        ///     Ends an update.
        /// </summary>
        void EndUpdate();

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="window"> The window. </param>
        void Initialize(IGameWindow window);

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="panel"> The panel. </param>
        void Initialize(Panel panel);

        /// <summary>
        ///     Query if 'key' is key down.
        /// </summary>
        /// <param name="key"> A variable-length parameters list containing key. </param>
        /// <returns>
        ///     True if key down, false if not.
        /// </returns>
        bool IsKeyDown(int key);

        /// <summary>
        ///     Query if 'key' is key down.
        /// </summary>
        /// <param name="key"> A variable-length parameters list containing key. </param>
        /// <returns>
        ///     True if key down, false if not.
        /// </returns>
        bool IsKeyDown(params int[] key);

        /// <summary>
        ///     Query if 'key' is key up.
        /// </summary>
        /// <param name="button"> The button. </param>
        /// <returns>
        ///     True if key up, false if not.
        /// </returns>
        bool IsKeyUp(RMouseButtons button);

        /// <summary>
        ///     Query if 'key' is key up.
        /// </summary>
        /// <param name="buttons"> A variable-length parameters list containing buttons. </param>
        /// <returns>
        ///     True if key up, false if not.
        /// </returns>
        bool IsKeyUp(params RMouseButtons[] buttons);

        /// <summary>
        ///     Query if 'key' is key up.
        /// </summary>
        /// <param name="key"> A variable-length parameters list containing key. </param>
        /// <returns>
        ///     True if key up, false if not.
        /// </returns>
        bool IsKeyUp(int key);

        /// <summary>
        ///     Query if 'key' is key up.
        /// </summary>
        /// <param name="key"> A variable-length parameters list containing key. </param>
        /// <returns>
        ///     True if key up, false if not.
        /// </returns>
        bool IsKeyUp(params int[] key);

        /// <summary>
        ///     Query if 'buttons' is mouse button down.
        /// </summary>
        /// <param name="button"> The button. </param>
        /// <returns>
        ///     True if mouse button down, false if not.
        /// </returns>
        bool IsMouseButtonDown(RMouseButtons button);

        /// <summary>
        ///     Query if 'buttons' is mouse button down.
        /// </summary>
        /// <param name="buttons"> A variable-length parameters list containing buttons. </param>
        /// <returns>
        ///     True if mouse button down, false if not.
        /// </returns>
        bool IsMouseButtonDown(params RMouseButtons[] buttons);
    }
}
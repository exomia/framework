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
    ///     Interface for raw input handler.
    /// </summary>
    public interface IRawInputHandler
    {
        /// <summary>
        ///     called than a key event occured.
        /// </summary>
        /// <param name="message"> [in,out] The message. </param>
        void Input_KeyEvent(ref Message message);

        /// <summary>
        ///     called than a mouse button is clicked.
        /// </summary>
        /// <param name="x">          x-coordinate. </param>
        /// <param name="y">          x-coordinate. </param>
        /// <param name="buttons">    buttons. </param>
        /// <param name="clicks">     clicks. </param>
        /// <param name="wheelDelta"> wheel delta. </param>
        void Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a mouse button is down.
        /// </summary>
        /// <param name="x">          x-coordinate. </param>
        /// <param name="y">          x-coordinate. </param>
        /// <param name="buttons">    buttons. </param>
        /// <param name="clicks">     clicks. </param>
        /// <param name="wheelDelta"> wheel delta. </param>
        void Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than the mouse moved.
        /// </summary>
        /// <param name="x">          x-coordinate. </param>
        /// <param name="y">          x-coordinate. </param>
        /// <param name="buttons">    buttons. </param>
        /// <param name="clicks">     clicks. </param>
        /// <param name="wheelDelta"> wheel delta. </param>
        void Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a mouse button is up.
        /// </summary>
        /// <param name="x">          x-coordinate. </param>
        /// <param name="y">          x-coordinate. </param>
        /// <param name="buttons">    buttons. </param>
        /// <param name="clicks">     clicks. </param>
        /// <param name="wheelDelta"> wheel delta. </param>
        void Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than the mouse wheel delta is changed.
        /// </summary>
        /// <param name="x">          x-coordinate. </param>
        /// <param name="y">          x-coordinate. </param>
        /// <param name="buttons">    buttons. </param>
        /// <param name="clicks">     clicks. </param>
        /// <param name="wheelDelta"> wheel delta. </param>
        void Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);
    }
}
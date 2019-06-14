#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;

namespace Exomia.Framework.Input
{
    /// <summary>
    ///     MouseButtons enum.
    /// </summary>
    [Flags]
    public enum MouseButtons
    {
        /// <summary>
        ///     default
        /// </summary>
        None = 0,

        /// <summary>
        ///     Left mouse button
        /// </summary>
        Left = 1 << 1,

        /// <summary>
        ///     Middle mouse button
        /// </summary>
        Middle = 1 << 2,

        /// <summary>
        ///     Right mouse button
        /// </summary>
        Right = 1 << 3,

        /// <summary>
        ///     Button4 mouse button
        /// </summary>
        Button4 = 1 << 4,

        /// <summary>
        ///     Button5 mouse button
        /// </summary>
        Button5 = 1 << 5
    }

    /// <summary>
    ///     default KeyEventHandler.
    /// </summary>
    /// <param name="keyValue"> keyValue. </param>
    /// <param name="shift">    shift key pressed. </param>
    /// <param name="alt">      alt key pressed. </param>
    /// <param name="ctrl">     ctrl key pressed. </param>
    public delegate void KeyEventHandler(int keyValue, bool shift, bool alt, bool ctrl);

    /// <summary>
    ///     default KeyPressEventHandler.
    /// </summary>
    /// <param name="key"> key. </param>
    public delegate void KeyPressEventHandler(char key);

    /// <summary>
    ///     default MouseEventHandler.
    /// </summary>
    /// <param name="x">          mouse position x. </param>
    /// <param name="y">          mouse position y. </param>
    /// <param name="buttons">    pressed buttons. </param>
    /// <param name="clicks">     clicks. </param>
    /// <param name="wheelDelta"> wheelDelta. </param>
    public delegate void MouseEventHandler(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

    /// <summary>
    ///     IInputDevice interface.
    /// </summary>
    public interface IInputDevice
    {
        /// <summary>
        ///     called than a key is down once.
        /// </summary>
        event KeyEventHandler KeyDown;

        /// <summary>
        ///     called every time a key is pressed.
        /// </summary>
        event KeyPressEventHandler KeyPress;

        /// <summary>
        ///     called than a key is up once after a key was down.
        /// </summary>
        event KeyEventHandler KeyUp;

        /// <summary>
        ///     called every time a mouse button is clicked.
        /// </summary>
        event MouseEventHandler MouseClick;

        /// <summary>
        ///     called than a mouse button is down once.
        /// </summary>
        event MouseEventHandler MouseDown;

        /// <summary>
        ///     called than the mouse moves.
        /// </summary>
        event MouseEventHandler MouseMove;

        /// <summary>
        ///     called than a mose button is up once after a button was down.
        /// </summary>
        event MouseEventHandler MouseUp;

        /// <summary>
        ///     called if the wheel delta is changed.
        /// </summary>
        event MouseEventHandler MouseWheel;

        /// <summary>
        ///     check if a specified keyValue is down.
        /// </summary>
        /// <param name="keyValue"> keyValue. </param>
        /// <returns>
        ///     <c>true</c> if the specified keyValue is down; <c>false</c> otherwise.
        /// </returns>
        bool IsKeyDown(int keyValue);

        /// <summary>
        ///     check if one of the specified keyValues is down.
        /// </summary>
        /// <param name="keyValues"> keyValues. </param>
        /// <returns>
        ///     <c>true</c> if one of the specified keyValues is down; <c>false</c> otherwise.
        /// </returns>
        bool IsKeyDown(params int[] keyValues);

        /// <summary>
        ///     check if a specified keyValue is up.
        /// </summary>
        /// <param name="keyValues"> keyValue. </param>
        /// <returns>
        ///     <c>true</c> if the specified keyValue is up; <c>false</c> otherwise.
        /// </returns>
        bool IsKeyUp(int keyValues);

        /// <summary>
        ///     check if one of the specified keyValues is up.
        /// </summary>
        /// <param name="keyValues"> keyValues. </param>
        /// <returns>
        ///     <c>true</c> if one of the specified keyValues is up; <c>false</c> otherwise.
        /// </returns>
        bool IsKeyUp(params int[] keyValues);

        /// <summary>
        ///     check if a specified mouse button is down.
        /// </summary>
        /// <param name="button"> MouseButtons. </param>
        /// <returns>
        ///     <c>true</c> if the specified button is down; <c>false</c> otherwise.
        /// </returns>
        bool IsMouseButtonDown(MouseButtons button);

        /// <summary>
        ///     check if one of the specified mouse buttons is down.
        /// </summary>
        /// <param name="buttons"> MouseButtons. </param>
        /// <returns>
        ///     <c>true</c> if one of the specified buttons is down; <c>false</c> otherwise.
        /// </returns>
        bool IsMouseButtonDown(params MouseButtons[] buttons);

        /// <summary>
        ///     check if a specified mouse button is up.
        /// </summary>
        /// <param name="button"> MouseButtons. </param>
        /// <returns>
        ///     <c>true</c> if the specified button is up; <c>false</c> otherwise.
        /// </returns>
        bool IsMouseButtonUp(MouseButtons button);

        /// <summary>
        ///     check if one of the specified mouse buttons is up.
        /// </summary>
        /// <param name="buttons"> MouseButtons. </param>
        /// <returns>
        ///     <c>true</c> if one of the specified buttons is up; <c>false</c> otherwise.
        /// </returns>
        bool IsMouseButtonUp(params MouseButtons[] buttons);

        /// <summary>
        ///     set the mouse to a specified position on the current window.
        /// </summary>
        /// <param name="x"> x-position. </param>
        /// <param name="y"> y-position. </param>
        void SetMousePosition(int x, int y);
    }
}
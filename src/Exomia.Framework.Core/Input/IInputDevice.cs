#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Input
{
    /// <summary> Interface for input device. </summary>
    public interface IInputDevice
    {
        /// <summary> Registers the key up. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterKeyUp(KeyEventHandler handler, int position = -1);

        /// <summary> Registers the key press. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterKeyPress(KeyPressEventHandler handler, int position = -1);

        /// <summary> Registers the key down. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterKeyDown(KeyEventHandler handler, int position = -1);

        /// <summary> Registers the raw mouse input. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterRawMouseInput(MouseEventHandler handler, int position = -1);

        /// <summary> Registers the mouse down. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterMouseDown(MouseEventHandler handler, int position = -1);

        /// <summary> Registers the mouse up. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterMouseUp(MouseEventHandler handler, int position = -1);

        /// <summary> Registers the mouse click. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterMouseClick(MouseEventHandler handler, int position = -1);

        /// <summary> Registers the mouse move. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterMouseMove(MouseEventHandler handler, int position = -1);

        /// <summary> Registers the mouse wheel. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterMouseWheel(MouseEventHandler handler, int position = -1);

        /// <summary> Unregister the key up described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterKeyUp(KeyEventHandler handler);

        /// <summary> Unregister the key press described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterKeyPress(KeyPressEventHandler handler);

        /// <summary> Unregister the key down described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterKeyDown(KeyEventHandler handler);

        /// <summary> Unregister the raw mouse input described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterRawMouseInput(MouseEventHandler handler);

        /// <summary> Unregister the mouse down described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterMouseDown(MouseEventHandler handler);

        /// <summary> Unregister the mouse up described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterMouseUp(MouseEventHandler handler);

        /// <summary> Unregister the mouse click described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterMouseClick(MouseEventHandler handler);

        /// <summary> Unregister the mouse move described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterMouseMove(MouseEventHandler handler);

        /// <summary> Unregister the mouse wheel described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterMouseWheel(MouseEventHandler handler);
    }
}
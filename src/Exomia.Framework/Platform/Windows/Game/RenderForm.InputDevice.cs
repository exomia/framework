#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Exomia.Framework.Input;

namespace Exomia.Framework.Platform.Windows.Game
{
    sealed partial class RenderForm : IInputDevice
    {
        private readonly Pipe<RawKeyEventHandler>   _rawKeyPipe;
        private readonly Pipe<KeyEventHandler>      _keyUpPipe, _keyDownPipe;
        private readonly Pipe<KeyPressEventHandler> _keyPressPipe;

        private readonly Pipe<MouseEventHandler> _mouseMovePipe,
                                                 _mouseUpPipe,
                                                 _mouseDownPipe,
                                                 _mouseClickPipe,
                                                 _mouseWheelPipe;

        private readonly Pipe<MouseEventHandler> _mouseRawInputPipe;

        /// <inheritdoc />
        void IInputDevice.RegisterRawKeyEvent(RawKeyEventHandler handler, int position)
        {
            _rawKeyPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterKeyUp(KeyEventHandler handler, int position)
        {
            _keyUpPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterKeyPress(KeyPressEventHandler handler, int position)
        {
            _keyPressPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterKeyDown(KeyEventHandler handler, int position)
        {
            _keyDownPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterMouseDown(MouseEventHandler handler, int position)
        {
            _mouseDownPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterMouseUp(MouseEventHandler handler, int position)
        {
            _mouseUpPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterMouseClick(MouseEventHandler handler, int position)
        {
            _mouseClickPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterMouseMove(MouseEventHandler handler, int position)
        {
            _mouseMovePipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterRawMouseInput(MouseEventHandler handler, int position)
        {
            _mouseRawInputPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.RegisterMouseWheel(MouseEventHandler handler, int position)
        {
            _mouseWheelPipe.Register(handler, position);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterRawKeyEvent(RawKeyEventHandler handler)
        {
            _rawKeyPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterKeyUp(KeyEventHandler handler)
        {
            _keyUpPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterKeyPress(KeyPressEventHandler handler)
        {
            _keyPressPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterKeyDown(KeyEventHandler handler)
        {
            _keyDownPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterRawMouseInput(MouseEventHandler handler)
        {
            _mouseRawInputPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterMouseDown(MouseEventHandler handler)
        {
            _mouseDownPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterMouseUp(MouseEventHandler handler)
        {
            _mouseUpPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterMouseClick(MouseEventHandler handler)
        {
            _mouseClickPipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterMouseMove(MouseEventHandler handler)
        {
            _mouseMovePipe.Unregister(handler);
        }

        /// <inheritdoc />
        void IInputDevice.UnregisterMouseWheel(MouseEventHandler handler)
        {
            _mouseWheelPipe.Unregister(handler);
        }

        /// <summary>
        ///     A pipe.
        /// </summary>
        /// <typeparam name="TDelegate"> Type of the delegate. </typeparam>
        private sealed class Pipe<TDelegate>
            where TDelegate : Delegate
        {
            /// <summary>
            ///     The list.
            /// </summary>
            private readonly List<TDelegate> _list;

            /// <summary>
            ///     Gets the number of the registered delegates.
            /// </summary>
            /// <value>
            ///     The count.
            /// </value>
            public int Count
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return _list.Count; }
            }

            /// <summary>
            ///     Indexer to get items within this collection using array index syntax.
            /// </summary>
            /// <param name="index"> Zero-based index of the entry to access. </param>
            /// <returns>
            ///     The indexed item.
            /// </returns>
            public TDelegate this[int index]
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return _list[index]; }
            }

            /// <summary>
            ///     Initializes a new instance of the &lt;see cref="Pipe&lt;TDelegate&gt;"/&gt; class.
            /// </summary>
            public Pipe()
            {
                _list = new List<TDelegate>(8);
            }

            /// <summary>
            ///     Registers this object.
            /// </summary>
            /// <param name="handler">  The handler. </param>
            /// <param name="position"> (Optional) The position. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Register(in TDelegate handler, int position = -1)
            {
                if (position == -1) { _list.Add(handler); }
                else { _list.Insert(position >= 0 ? position : _list.Count + position, handler); }
            }

            /// <summary>
            ///     Deregisters this object.
            /// </summary>
            /// <param name="handler"> The handler. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Unregister(in TDelegate handler)
            {
                _list.Remove(handler);
            }

            /// <summary>
            ///     Deregisters this object.
            /// </summary>
            /// <param name="position"> The position. </param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Unregister(int position)
            {
                _list.RemoveAt(position);
            }
        }
    }
}
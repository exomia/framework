#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using SharpDX.X3DAudio;
using SharpDX.XAudio2;

namespace Exomia.Framework.Audio
{
    /// <summary>
    ///     List of linked sounds. This class cannot be inherited.
    /// </summary>
    sealed class LinkedSoundList : IEnumerable<LinkedSoundList.Sound>
    {
        /// <summary>
        ///     The capacity.
        /// </summary>
        private readonly int _capacity;

        /// <summary>
        ///     this lock.
        /// </summary>
        private readonly object _thisLock;

        /// <summary>
        ///     The head.
        /// </summary>
        private Sound? _head;

        /// <summary>
        ///     Number of.
        /// </summary>
        private int _count;

        /// <summary>
        ///     Gets the capacity.
        /// </summary>
        /// <value>
        ///     The capacity.
        /// </value>
        public int Capacity
        {
            // ReSharper disable once InconsistentlySynchronizedField
            get { return _capacity; }
        }

        /// <summary>
        ///     Gets the number of.
        /// </summary>
        /// <value>
        ///     The count.
        /// </value>
        public int Count
        {
            // ReSharper disable once InconsistentlySynchronizedField
            get { return _count; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LinkedSoundList" /> class.
        /// </summary>
        /// <param name="capacity"> (Optional) The capacity. </param>
        public LinkedSoundList(int capacity = int.MaxValue)
        {
            _capacity = capacity;
            _count    = 0;
            _thisLock = new object();
        }

        /// <summary>
        ///     Gets the enumerator.
        /// </summary>
        /// <returns>
        ///     The enumerator.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator<Sound> IEnumerable<Sound>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds sound.
        /// </summary>
        /// <param name="sound"> The sound to remove. </param>
        public void Add(Sound sound)
        {
            lock (_thisLock)
            {
                if (_count + 1 >= _capacity) { return; }

                if (_head == null)
                {
                    sound.Next     = sound;
                    sound.Previous = sound;
                    _head          = sound;
                }
                else
                {
                    sound.Next     = _head;
                    sound.Previous = _head.Previous;

                    _head.Previous!.Next = sound;
                    _head.Previous      = sound;
                }

                _count++;
            }
        }

        /// <summary>
        ///     Clears this object to its blank/initial state.
        /// </summary>
        public void Clear()
        {
            lock (_thisLock)
            {
                _head  = null;
                _count = 0;
            }
        }

        /// <summary>
        ///     Removes the given sound.
        /// </summary>
        /// <param name="sound"> The sound to remove. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public void Remove(Sound sound)
        {
            lock (_thisLock)
            {
                if (sound == null) { throw new ArgumentNullException(nameof(sound)); }
                if (sound.Next == sound) { _head = null; }
                else
                {
                    sound.Previous!.Next = sound.Next;
                    sound.Next!.Previous = sound.Previous;
                    if (_head == sound) { _head = sound.Next; }
                }
                sound.Invalidate();
                _count--;
            }
        }

        /// <summary>
        ///     Gets the enumerator.
        /// </summary>
        /// <returns>
        ///     The enumerator.
        /// </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        ///     An enumerator.
        /// </summary>
        public struct Enumerator : IEnumerator<Sound>
        {
            /// <summary>
            ///     The list.
            /// </summary>
            private readonly LinkedSoundList _list;

            /// <summary>
            ///     Gets the node.
            /// </summary>
            /// <value>
            ///     The node.
            /// </value>
            private Sound? _current, _node;

            /// <inheritdoc />
            public Sound Current
            {
                get { return _current!; }
            }

            /// <inheritdoc />
            object IEnumerator.Current
            {
                get { return Current; }
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="LinkedSoundList" /> class.
            /// </summary>
            /// <param name="list"> The list. </param>
            public Enumerator(LinkedSoundList list)
            {
                _list    = list;
                _node    = list._head;
                _current = null;
            }

            /// <inheritdoc />
            public bool MoveNext()
            {
                if (_node == null)
                {
                    return false;
                }
                _current = _node;
                _node    = _node.Next;
                if (_node == _list._head)
                {
                    _node = null;
                }
                return true;
            }

            /// <inheritdoc />
            public void Reset()
            {
                _node    = _list._head;
                _current = null;
            }

            /// <inheritdoc />
            public void Dispose() { }
        }

        /// <summary>
        ///     A sound. This class cannot be inherited.
        /// </summary>
        internal sealed class Sound
        {
            /// <summary>
            ///     The emitter.
            /// </summary>
            internal readonly Emitter Emitter;

            /// <summary>
            ///     Source voice.
            /// </summary>
            internal readonly SourceVoice SourceVoice;

            /// <summary>
            ///     The next.
            /// </summary>
            internal Sound? Next;

            /// <summary>
            ///     The previous.
            /// </summary>
            internal Sound? Previous;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Sound" /> class.
            /// </summary>
            /// <param name="emitter">     The emitter. </param>
            /// <param name="sourceVoice"> Source voice. </param>
            public Sound(Emitter emitter, SourceVoice sourceVoice)
            {
                Emitter     = emitter;
                SourceVoice = sourceVoice;
            }

            /// <summary>
            ///     Invalidates this object.
            /// </summary>
            internal void Invalidate()
            {
                Next     = null;
                Previous = null;
            }
        }
    }
}
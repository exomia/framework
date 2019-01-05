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

using System.Collections.Generic;

namespace Exomia.Framework.Audio
{
    sealed class LinkedSoundList
    {
        private readonly int _capacity;

        private readonly object _thisLock;
        private Sound _tail;
        private int _count;

        public int Capacity
        {
            // ReSharper disable once InconsistentlySynchronizedField
            get { return _capacity; }
        }

        public int Count
        {
            // ReSharper disable once InconsistentlySynchronizedField
            get { return _count; }
        }

        public LinkedSoundList(int capacity = int.MaxValue)
        {
            _capacity = capacity;
            _count    = 0;
            _thisLock = new object();
        }

        public void Add(Sound sound)
        {
            lock (_thisLock)
            {
                if (_count + 1 >= _capacity) { return; }
                _count++;

                sound.Previous = _tail;
                sound.Next     = null;

                if (_tail != null)
                {
                    _tail.Next = sound;
                }
                _tail = sound;
            }
        }

        public void Clear()
        {
            _tail = null;

            // ReSharper disable once InconsistentlySynchronizedField
            _count = 0;
        }

        public IEnumerable<Sound> Enumerate()
        {
            Sound end = _tail;
            while (end != null)
            {
                yield return end;
                end = end.Previous;
            }
        }

        public void Remove(Sound sound)
        {
            lock (_thisLock)
            {
                _count--;

                if (sound.Next != null)
                {
                    if (sound.Previous != null)
                    {
                        sound.Previous.Next = sound.Next;
                    }
                    sound.Next.Previous = sound.Previous;
                }

                if (sound.Previous != null)
                {
                    if (sound.Next != null)
                    {
                        sound.Next.Previous = sound.Previous;
                    }
                    sound.Previous.Next = sound.Next;
                }
            }
        }
    }
}
using System.Collections.Generic;

namespace Exomia.Framework.Audio
{
    internal sealed class LinkedSoundList
    {
        private Sound _tail;

        public LinkedSoundList(int capacity = int.MaxValue)
        {
            Capacity = capacity;
        }

        public int Capacity { get; }

        public int Count { get; private set; }

        public void Add(Sound sound)
        {
            lock (this)
            {
                if (Count + 1 >= Capacity) { return; }

                sound.Previous = _tail;
                sound.Next = null;

                if (_tail != null)
                {
                    _tail.Next = sound;
                }
                _tail = sound;

                Count++;
            }
        }

        public void Remove(Sound sound)
        {
            lock (this)
            {
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
                Count--;
            }
        }

        public void Clear()
        {
            _tail = null;
            Count = 0;
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
    }
}
﻿#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using SharpDX;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     Random2 class
    /// </summary>
    public sealed class Random2
    {
        private const float SINGLE_UNIT_INT = 1.0f / int.MaxValue;
        private const double REAL_UNIT_INT = 1.0 / int.MaxValue;

        private const double REAL_UNIT_INT1 = 1.0 / (int.MaxValue + 1.0);
        private const double REAL_UNIT_UINT1 = 1.0 / (uint.MaxValue + 1.0);
        private const uint Y = 842502087, Z = 3579807591, W = 273326509;

        /// <summary>
        ///     Default Random2
        /// </summary>
        public static Random2 Default = new Random2();

        private uint _x, _y, _z, _w;

        /// <inheritdoc />
        public Random2()
            : this((int)DateTime.Now.Ticks) { }

        /// <summary>
        ///     Random2 constructor
        /// </summary>
        public Random2(int seed)
        {
            _x = (uint)seed;
            _y = Y;
            _z = Z;
            _w = W;
        }

        /// <summary>
        ///     Gets the next random byte value.
        /// </summary>
        /// <returns>A random byte value between 0 and int.MaxValue.</returns>
        public int Next()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8)));
        }

        /// <summary>
        ///     Gets the next random int value which is greater than zero and less than
        ///     the specified maximum value.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns>A random int value between zero and the specified maximum value.</returns>
        public int Next(int max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (int)(REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * max);
        }

        /// <summary>
        ///     Gets the next random int value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns>A random int value between the specified minimum and maximum values.</returns>
        public int Next(int min, int max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;

            int range = max - min;
            if (range < 0)
            {
                return min + (int)(REAL_UNIT_UINT1 * (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8)) * (max - (long)min));
            }
            return min + (int)(REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * range);
        }

        /// <summary>
        ///     Gets the next random angle value.
        /// </summary>
        /// <returns>A random angle value.</returns>
        public float NextAngle()
        {
            return (float)NextDouble(-Math.PI, Math.PI);
        }

        /// <summary>
        ///     Gets the next random byte value.
        /// </summary>
        /// <returns>A random byte value between 0 and 255.</returns>
        public byte NextByte()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (byte)(REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * byte.MaxValue);
        }

        /// <summary>
        ///     Gets the next random byte value which is greater than zero and less than or equal to
        ///     the specified maximum value.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns>A random byte value between zero and the specified maximum value.</returns>
        public byte NextByte(byte max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (byte)(REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * max);
        }

        /// <summary>
        ///     Gets the next random byte value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns>A random byte value between the specified minimum and maximum values.</returns>
        public byte NextByte(byte min, byte max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (byte)(min + REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) *
                          (max - min));
        }

        /// <summary>
        ///     Fills a byte array with random values.
        /// </summary>
        /// <param name="buffer">the byte array to fill with random values</param>
        public void NextBytes(byte[] buffer)
        {
            uint x = _x, y = _y, z = _z, w = _w;
            int i = 0;
            uint t;
            int l = buffer.Length;
            for (int bound = l - 3; i < bound;)
            {
                t = x ^ (x << 11);
                x = y;
                y = z;
                z = w;
                w = w ^ (w >> 19) ^ t ^ (t >> 8);

                buffer[i++] = (byte)w;
                buffer[i++] = (byte)(w >> 8);
                buffer[i++] = (byte)(w >> 16);
                buffer[i++] = (byte)(w >> 24);
            }

            if (i < l)
            {
                t = x ^ (x << 11);
                x = y;
                y = z;
                z = w;
                w = w ^ (w >> 19) ^ t ^ (t >> 8);

                buffer[i++] = (byte)w;
                if (i < l)
                {
                    buffer[i++] = (byte)(w >> 8);
                    if (i < l)
                    {
                        buffer[i++] = (byte)(w >> 16);
                        if (i < l)
                        {
                            buffer[i] = (byte)(w >> 24);
                        }
                    }
                }
            }
            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        /// <summary>
        ///     Fills a byte array with random values.
        /// </summary>
        /// <param name="buffer">the byte array to fill with random values</param>
        public unsafe void NextBytesUnsafe(byte[] buffer)
        {
            int l = buffer.Length;
            if (l % 8 != 0)
            {
                throw new ArgumentException("Buffer length must be divisible by 8", nameof(buffer));
            }

            uint x = _x, y = _y, z = _z, w = _w;

            fixed (byte* pByte0 = buffer)
            {
                uint* pDWord = (uint*)pByte0;
                for (int i = 0, len = l >> 2; i < len; i += 2)
                {
                    uint t = x ^ (x << 11);
                    x         = y;
                    y         = z;
                    z         = w;
                    pDWord[i] = w = w ^ (w >> 19) ^ t ^ (t >> 8);

                    t             = x ^ (x << 11);
                    x             = y;
                    y             = z;
                    z             = w;
                    pDWord[i + 1] = w = w ^ (w >> 19) ^ t ^ (t >> 8);
                }
            }

            _x = x;
            _y = y;
            _z = z;
            _w = w;
        }

        /// <summary>
        ///     Gets the next random color.
        ///     Note: alpha = 255
        /// </summary>
        /// <returns>a random color between min and max.</returns>
        public Color NextColor()
        {
            return new Color(
                Next(255),
                Next(255),
                Next(255),
                255);
        }

        /// <summary>
        ///     Gets the next random color between min and max.
        /// </summary>
        /// <param name="min">min color</param>
        /// <param name="max">max color</param>
        /// <returns>a random color between min and max.</returns>
        public Color NextColor(Color min, Color max)
        {
            return new Color(
                Next(min.R, max.R),
                Next(min.G, max.G),
                Next(min.B, max.B),
                Next(min.A, max.A));
        }

        /// <summary>
        ///     Gets the next random double value.
        /// </summary>
        /// <returns>A random double value between 0 and 1.</returns>
        public double NextDouble()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8)));
        }

        /// <summary>
        ///     Gets the next random double value which is greater than zero and less than or equal to
        ///     the specified maximum value.
        /// </summary>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random double value between zero and the specified maximum value.</returns>
        public double NextDouble(double max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * max;
        }

        /// <summary>
        ///     Gets the next random double value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random double value between the specified minimum and maximum values.</returns>
        public double NextDouble(double min, double max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * (max - min);
        }

        /// <summary>
        ///     Gets the next random single value.
        /// </summary>
        /// <returns>A random single value between 0 and 1.</returns>
        public float NextSingle()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return SINGLE_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8)));
        }

        /// <summary>
        ///     Gets the next random single value which is greater than zero and less than or equal to
        ///     the specified maximum value.
        /// </summary>
        /// <param name="max">The exclusive maximum value.</param>
        /// <returns>A random single value between zero and the specified maximum value.</returns>
        public float NextSingle(float max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return SINGLE_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * max;
        }

        /// <summary>
        ///     Gets the next random single value between the specified minimum and maximum values.
        /// </summary>
        /// <param name="min">The inclusive minimum value.</param>
        /// <param name="max">The inclusive maximum value.</param>
        /// <returns>A random single value between the specified minimum and maximum values.</returns>
        public float NextSingle(float min, float max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + SINGLE_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * (max - min);
        }

        /// <summary>
        ///     Gets the next random unit vector.
        /// </summary>
        /// <returns>A random unit vector.</returns>
        public Vector2 NextUnitVector()
        {
            float angle = NextAngle();
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        ///     Gets the next random unit vector.
        /// </summary>
        /// <returns>A random unit vector.</returns>
        public unsafe void NextUnitVector(Vector2* vector)
        {
            float angle = NextAngle();
            vector->X = (float)Math.Cos(angle);
            vector->Y = (float)Math.Sin(angle);
        }
    }
}
#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;

namespace Exomia.Framework.Mathematics
{
    /// <summary> A random 2. This class cannot be inherited. </summary>
    public sealed class Random2
    {
        private const float  SINGLE_UNIT_INT = 1.0f / int.MaxValue;
        private const double REAL_UNIT_INT   = 1.0 / int.MaxValue;
        private const double REAL_UNIT_INT1  = 1.0 / (int.MaxValue + 1.0);
        private const double REAL_UNIT_UINT1 = 1.0 / (uint.MaxValue + 1.0);
        private const uint   Y               = 842502087, Z = 3579807591, W = 273326509;

        /// <summary> Default Random2. </summary>
        public static Random2 Default = new Random2();

        private uint _x, _y, _z, _w;

        /// <summary> Initializes a new instance of the <see cref="Random2" /> class. </summary>
        public Random2()
            : this(DateTime.Now.Ticks ^ Environment.TickCount) { }

        /// <summary> Initializes a new instance of the <see cref="Random2" /> class. </summary>
        /// <param name="seed"> The seed. </param>
        public Random2(long seed)
        {
            _x = (uint)seed;
            _y = (uint)(seed >> 32) ^ Y;
            _z = Z;
            _w = W;
        }

        /// <summary> Gets the next random byte value. </summary>
        /// <returns> A random byte value between 0 and int.MaxValue. </returns>
        public int Next()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8)));
        }

        /// <summary>
        ///     Gets the next random int value which is greater than zero and less than the specified
        ///     maximum value.
        /// </summary>
        /// <param name="max"> The exclusive maximum value. </param>
        /// <returns> A random int value between zero and the specified maximum value. </returns>
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
        /// <param name="min"> The inclusive minimum value. </param>
        /// <param name="max"> The exclusive maximum value. </param>
        /// <returns> A random int value between the specified minimum and maximum values. </returns>
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

        /// <summary> Gets the next random angle value. </summary>
        /// <returns> A random angle value. </returns>
        public float NextAngle()
        {
            return (float)NextDouble(-Math.PI, Math.PI);
        }

        /// <summary> Gets the next random byte value. </summary>
        /// <returns> A random byte value between 0 and 255. </returns>
        public byte NextByte()
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (byte)(REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * byte.MaxValue);
        }

        /// <summary>
        ///     Gets the next random byte value which is greater than zero and less than or equal to the
        ///     specified maximum value.
        /// </summary>
        /// <param name="max"> The exclusive maximum value. </param>
        /// <returns> A random byte value between zero and the specified maximum value. </returns>
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
        /// <param name="min"> The inclusive minimum value. </param>
        /// <param name="max"> The exclusive maximum value. </param>
        /// <returns> A random byte value between the specified minimum and maximum values. </returns>
        public byte NextByte(byte min, byte max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return (byte)(min + (REAL_UNIT_INT1 * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) *
                                 (max - min)));
        }

        /// <summary> Fills a byte array with random values. </summary>
        /// <param name="buffer"> the byte array to fill with random values. </param>
        public void NextBytes(byte[] buffer)
        {
            uint x = _x, y = _y, z = _z, w = _w;
            int  i = 0;
            uint t;
            int  l = buffer.Length;
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

        /// <summary> Fills a byte array with random values. </summary>
        /// <param name="buffer"> the byte array to fill with random values. </param>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
        ///                                      illegal values. </exception>
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

        /// <summary> Gets the next random double value. </summary>
        /// <returns> A random double value between 0 and 1. </returns>
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
        /// <param name="max"> The inclusive maximum value. </param>
        /// <returns> A random double value between zero and the specified maximum value. </returns>
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
        /// <param name="min"> The inclusive minimum value. </param>
        /// <param name="max"> The inclusive maximum value. </param>
        /// <returns> A random double value between the specified minimum and maximum values. </returns>
        public double NextDouble(double min, double max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + (REAL_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * (max - min));
        }

        /// <summary> Gets the next random single value. </summary>
        /// <returns> A random single value between 0 and 1. </returns>
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
        /// <param name="max"> The exclusive maximum value. </param>
        /// <returns> A random single value between zero and the specified maximum value. </returns>
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
        /// <param name="min"> The inclusive minimum value. </param>
        /// <param name="max"> The inclusive maximum value. </param>
        /// <returns> A random single value between the specified minimum and maximum values. </returns>
        public float NextSingle(float min, float max)
        {
            uint t = _x ^ (_x << 11);
            _x = _y;
            _y = _z;
            _z = _w;
            return min + (SINGLE_UNIT_INT * (int)(0x7FFFFFFF & (_w = _w ^ (_w >> 19) ^ t ^ (t >> 8))) * (max - min));
        }

        /// <summary> Gets the next random unit vector. </summary>
        /// <returns> A random unit vector. </returns>
        public Vector2 NextUnitVector()
        {
            Vector2 v;
            Math2.SinCos(NextAngle(), out v.X, out v.Y);
            return v;
        }

        /// <summary> Gets the next random unit vector. </summary>
        /// <param name="v"> [out] The out Vector2 to process. </param>
        public void NextUnitVector(out Vector2 v)
        {
            Math2.SinCos(NextAngle(), out v.X, out v.Y);
        }

        /// <summary> Gets the next random unit vector. </summary>
        /// <param name="vector"> [in,out] If non-null, the vector. </param>
        public unsafe void NextUnitVector(Vector2* vector)
        {
            Math2.SinCos(NextAngle(), out vector->X, out vector->Y);
        }
    }
}
#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     The mathematics 2.
    /// </summary>
    public static partial class Math2
    {
        private const long L_OFFSET_MAX = int.MaxValue + 1L;

        /// <summary>
        ///     calculates the absolute value of x
        /// </summary>
        /// <param name="x">value</param>
        /// <returns>positive x</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            return (x + (x >> 31)) ^ (x >> 31);
        }

        /// <summary>
        ///     Returns the smallest integer greater than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="f">A floating-point number with single precision</param>
        /// <returns>The smallest integer, which is greater than or equal to f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(double f)
        {
            return (int)(L_OFFSET_MAX - (long)(L_OFFSET_MAX - f));
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        public static int CountOnes(byte x)
        {
            int y = x;
            y -= (y >> 1) & 0x55;
            y =  ((y >> 2) & 0x33) + (y & 0x33);
            return (y & 0x0F) + (y >> 4);
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        public static int CountOnes(ushort x)
        {
            int y = x;
            y -= (y >> 1) & 0x5555;
            y =  ((y >> 2) & 0x3333) + (y & 0x3333);
            y =  ((y >> 4) + y) & 0x0F0F;
            return (y + (y >> 8)) & 0x001F;
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnes(int x)
        {
            return CountOnes((uint)x);
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        public static int CountOnes(uint x)
        {
            x -= (x >> 1) & 0x55555555;
            x =  ((x >> 2) & 0x33333333) + (x & 0x33333333);
            x =  ((x >> 4) + x) & 0x0F0F0F0F;
            x += x >> 8;
            return (int)((x + (x >> 16)) & 0x0000003F);
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnes(long x)
        {
            return CountOnes((ulong)x);
        }

        /// <summary>
        ///     Returns the number of 'on' bits in x
        /// </summary>
        public static int CountOnes(ulong x)
        {
            x -= (x >> 1) & 0x5555555555555555u;
            x =  ((x >> 2) & 0x3333333333333333u) + (x & 0x3333333333333333u);
            x =  ((x >> 4) + x) & 0x0F0F0F0F0F0F0F0Fu;
            x += x >> 8;
            x += x >> 16;
            return ((int)x + (int)(x >> 32)) & 0x0000007F;
        }

        /// <summary>
        ///     Returns the largest integer less than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="f">A floating-point number with single precision</param>
        /// <returns>The largest integer, which is less than or equal to f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(double f)
        {
            return (int)((long)(f + L_OFFSET_MAX) - L_OFFSET_MAX);
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Lerp(byte a, byte b, double t)
        {
            return (byte)(a + (t * (b - a)));
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, double t)
        {
            return (float)(a + (t * (b - a)));
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, double t)
        {
            return a + (t * (b - a));
        }

        /// <summary>
        ///     LinearInterpolate
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <param name="t">t</param>
        /// <returns>new Vector2</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }

        /// <summary>
        ///     Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9
        /// </summary>
        /// <remarks>
        ///     The return value is -1 for an input of zero (for which the logarithm is technically undefined.)
        /// </remarks>
        public static int Log2Floor(uint x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            return CountOnes(x | (x >> 16)) - 1;
        }

        /// <summary>
        ///     Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9
        /// </summary>
        /// <remarks>
        ///     The return value is -1 for an input of zero (for which the logarithm is technically undefined.)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(int x)
        {
            if (x < 0) { throw new ArgumentOutOfRangeException(nameof(x), "Can't compute Log2Floor of a negative"); }
            return Log2Floor((uint)x);
        }

        /// <summary>
        ///     Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9
        /// </summary>
        /// <remarks>
        ///     The return value is -1 for an input of zero (for which the logarithm is technically undefined.)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(ulong x)
        {
            uint xHi = (uint)(x >> 32);
            if (xHi != 0)
            {
                return 32 + Log2Floor(xHi);
            }
            return Log2Floor((uint)x);
        }

        /// <summary>
        ///     Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9
        /// </summary>
        /// <remarks>
        ///     The return value is -1 for an input of zero (for which the logarithm is technically undefined.)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(long x)
        {
            if (x < 0) { throw new ArgumentOutOfRangeException(nameof(x), "Can't compute Log2Floor of a negative"); }
            return Log2Floor((ulong)x);
        }

        /// <summary>
        ///     Maps a value from l1 to u1 to l2 to u2
        /// </summary>
        /// <param name="v">Value </param>
        /// <param name="l1">Lower 1</param>
        /// <param name="u1">Upper 1</param>
        /// <param name="l2">Lower 2</param>
        /// <param name="u2">Upper 2</param>
        /// <returns>maped value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float v, float l1, float u1, float l2, float u2)
        {
            return (((v - l1) / (u1 - l1)) * (u2 - l2)) + l2;
        }

        /// <summary>
        ///     Maps a value from l1 to u1 to l2 to u2
        /// </summary>
        /// <param name="v">Value </param>
        /// <param name="l1">Lower 1</param>
        /// <param name="u1">Upper 1</param>
        /// <param name="l2">Lower 2</param>
        /// <param name="u2">Upper 2</param>
        /// <returns>maped value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Map(double v, double l1, double u1, double l2, double u2)
        {
            return (((v - l1) / (u1 - l1)) * (u2 - l2)) + l2;
        }

        /// <summary>
        ///     raise b to the power of e
        /// </summary>
        /// <param name="b">base</param>
        /// <param name="e">exponent</param>
        /// <returns>b^e</returns>
        public static double Pow(double b, int e)
        {
            if (e < 0) { throw new ArgumentException($"{nameof(e)} must be positive", nameof(e)); }

            double result = 1;
            while (e != 0)
            {
                if ((e & 1) == 1)
                {
                    result *= b;
                }
                e >>= 1;
                b *=  b;
            }
            return result;
        }

        /// <summary>
        ///     raise b to the power of e
        /// </summary>
        /// <param name="b">base</param>
        /// <param name="e">exponent</param>
        /// <returns>b^e</returns>
        public static float Pow(float b, int e)
        {
            if (e < 0) { throw new ArgumentException($"{nameof(e)} must be positive", nameof(e)); }

            float result = 1;
            while (e != 0)
            {
                if ((e & 1) == 1)
                {
                    result *= b;
                }
                e >>= 1;
                b *=  b;
            }
            return result;
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, it is returned unchanged.
        ///     If it is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpToPowerOfTwo(int value)
        {
            return value <= 0 ? 0 : (int)RoundUpToPowerOfTwo((uint)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, or zero, it is returned unchanged.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RoundUpToPowerOfTwo(uint value)
        {
            if (value > 0x80000000)
            {
                throw new ArgumentOutOfRangeException();
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, it is returned unchanged.
        ///     If it is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RoundUpToPowerOfTwo(long value)
        {
            return value <= 0 ? 0 : (long)RoundUpToPowerOfTwo((ulong)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, or zero, it is returned unchanged.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundUpToPowerOfTwo(ulong value)
        {
            if (value > 0x8000000000000000)
            {
                throw new ArgumentOutOfRangeException();
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return value + 1;
        }

        /// <summary>
        ///     Returns the square root of a specified number.
        /// </summary>
        /// <param name="value">The number whose square root is to be found.</param>
        /// <returns>the square root</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sqrt(long value)
        {
            if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Can't compute Sqrt of a negative"); }
            return Sqrt((ulong)value);
        }

        /// <summary>
        ///     Returns the square root of a specified number.
        /// </summary>
        /// <param name="value">The number whose square root is to be found.</param>
        /// <returns>the square root</returns>
        public static uint Sqrt(ulong value)
        {
            if (value == 0)
            {
                return 0;
            }

            uint g     = 0;
            int  bshft = Log2Floor(value) >> 1;
            uint b     = 1u               << bshft;
            do
            {
                ulong temp = (ulong)(g + g + b) << bshft;

                if (value >= temp)
                {
                    g     += b;
                    value -= temp;
                }
                b >>= 1;
            } while (bshft-- > 0);

            return g;
        }

        /// <summary>
        ///     Returns the square root of a specified number.
        /// </summary>
        /// <param name="value">The number whose square root is to be found.</param>
        /// <returns>the square root</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(int value)
        {
            if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value), "Can't compute Sqrt of a negative"); }
            return (int)Sqrt((uint)value);
        }

        /// <summary>
        ///     Returns the square root of a specified number.
        /// </summary>
        /// <param name="value">The number whose square root is to be found.</param>
        /// <returns>the square root</returns>
        public static uint Sqrt(uint value)
        {
            if (value == 0)
            {
                return 0;
            }

            uint g     = 0;
            int  bshft = Log2Floor(value) >> 1;
            uint b     = 1u               << bshft;
            do
            {
                uint temp = (g + g + b) << bshft;
                if (value >= temp)
                {
                    g     += b;
                    value -= temp;
                }
                b >>= 1;
            } while (bshft-- > 0);

            return g;
        }
    }
}
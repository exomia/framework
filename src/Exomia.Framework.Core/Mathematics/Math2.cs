#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Core.Mathematics
{
    /// <summary> The mathematics 2. </summary>
    public static partial class Math2
    {
        private const long L_OFFSET_MAX = int.MaxValue + 1L;

        /// <summary> 1e-6f. </summary>
        public const float ZERO_TOLERANCE = 1e-6f;

        /// <summary> 3.14159265358979323846. </summary>
        public const float PI = (float)Math.PI;

        /// <summary> 2PI. </summary>
        public const float TWO_PI = (float)(2.0 * Math.PI);

        /// <summary> 1 / 2PI. </summary>
        public const float ONE_OVER_TWO_PI = 1.0f / TWO_PI;

        /// <summary> PI / 2. </summary>
        public const float PI_OVER_TWO = (float)(Math.PI / 2.0);

        /// <summary> PI / 4. </summary>
        public const float PI_OVER_FOUR = (float)(Math.PI / 4.0);

        /// <summary> Determines whether the specified value is close to zero (0.0f). </summary>
        /// <param name="a"> The floating value. </param>
        /// <returns> <c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>. </returns>
        public static bool IsZero(float a)
        {
            return Math.Abs(a) < ZERO_TOLERANCE;
        }

        /// <summary> Determines whether the specified value is not close to zero (0.0f). </summary>
        /// <param name="a"> The floating value. </param>
        /// <returns> <c>true</c> if the specified value is not close to zero (0.0f); otherwise, <c>false</c>. </returns>
        public static bool IsNotZero(float a)
        {
            return Math.Abs(a) >= ZERO_TOLERANCE;
        }

        /// <summary> Determines whether the specified value is close to one (1.0f). </summary>
        /// <param name="a"> The floating value. </param>
        /// <returns> <c>true</c> if the specified value is close to one (1.0f); otherwise, <c>false</c>. </returns>
        public static bool IsOne(float a)
        {
            return IsZero(a - 1.0f);
        }

        /// <summary>
        ///     Checks if a and b are almost equals, taking into account the magnitude of floating point numbers (unlike
        ///     <see cref="WithinEpsilon" /> method). See Remarks.
        ///     See remarks.
        /// </summary>
        /// <param name="a"> The left value to compare. </param>
        /// <param name="b"> The right value to compare. </param>
        /// <returns> <c>true</c> if a almost equal to b, <c>false</c> otherwise. </returns>
        /// <remarks>
        ///     The code is using the technique described by Bruce Dawson in
        ///     <a href="http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/">
        ///         Comparing Floating point numbers 2012 edition
        ///     </a>
        ///     .
        /// </remarks>
        public static unsafe bool NearEqual(float a, float b)
        {
            // Check if the numbers are really close -- needed
            // when comparing numbers near zero.
            if (IsZero(a - b))
            {
                return true;
            }

            // Original from Bruce Dawson: http://randomascii.wordpress.com/2012/02/25/comparing-floating-point-numbers-2012-edition/
            int aInt = *(int*)&a;
            int bInt = *(int*)&b;

            // Different signs means they do not match.
            if ((aInt < 0) != (bInt < 0))
            {
                return false;
            }

            // Find the difference in ULPs.
            int ulp = Math.Abs(aInt - bInt);

            // Choose of maxUlp = 4
            // according to http://code.google.com/p/googletest/source/browse/trunk/include/gtest/internal/gtest-internal.h
            const int maxUlp = 4;
            return (ulp <= maxUlp);
        }

        /// <summary> Checks if a - b are almost equals within a float epsilon. </summary>
        /// <param name="a">       The left value to compare. </param>
        /// <param name="b">       The right value to compare. </param>
        /// <param name="epsilon"> Epsilon value. </param>
        /// <returns> <c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise. </returns>
        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float num = a - b;
            return ((-epsilon <= num) && (num <= epsilon));
        }

        /// <summary> calculates the absolute value of x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> positive x. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            return (x + (x >> 31)) ^ (x >> 31);
        }

        /// <summary> Returns the smallest integer greater than or equal to the specified floating-point number. </summary>
        /// <param name="f"> A floating-point number with single precision. </param>
        /// <returns> The smallest integer, which is greater than or equal to f. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(double f)
        {
            return (int)(L_OFFSET_MAX - (long)(L_OFFSET_MAX - f));
        }

        /// <summary> Returns the largest integer less than or equal to the specified floating-point number. </summary>
        /// <param name="f"> A floating-point number with single precision. </param>
        /// <returns> The largest integer, which is less than or equal to f. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(double f)
        {
            return (int)((long)(f + L_OFFSET_MAX) - L_OFFSET_MAX);
        }

        /// <summary> raise b to the power of e. </summary>
        /// <param name="b"> base. </param>
        /// <param name="e"> exponent. </param>
        /// <returns> b^e. </returns>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
        public static double Pow(double b, int e)
        {
            if (e < 0)
            {
                throw new ArgumentException($"{nameof(e)} must be positive", nameof(e));
            }

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

        /// <summary> raise b to the power of e. </summary>
        /// <param name="b"> base. </param>
        /// <param name="e"> exponent. </param>
        /// <returns> b^e. </returns>
        /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
        public static float Pow(float b, int e)
        {
            if (e < 0)
            {
                throw new ArgumentException($"{nameof(e)} must be positive", nameof(e));
            }

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

        /// <summary> Returns the approximated sinus of a specified number. </summary>
        /// <param name="x"> The value. </param>
        /// <returns> A float. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Sin(float x)
        {
            return Cos(x - PI_OVER_TWO);
        }

        /// <summary> Returns the approximated cosine of a specified number. </summary>
        /// <param name="x"> The value. </param>
        /// <returns> A float. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Cos(float x)
        {
            x *= ONE_OVER_TWO_PI;
            x -= 0.25f + Floor(x + 0.25f);
            x *= 16.0f * (Math.Abs(x) - 0.5f);
            x += 0.225f * x * (Math.Abs(x) - 1.0f);
            return x;
        }

        /// <summary> Sine cosine. </summary>
        /// <param name="x">   The value in radians. </param>
        /// <param name="sin"> [out] The sine. </param>
        /// <param name="cos"> [out] The cosine. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SinCos(float x, out float sin, out float cos)
        {
            const float A = 1.27323954f, B = 0.405284735f;
            x -= TWO_PI * Floor((x / TWO_PI) + 0.5f);
            sin = x < 0
                ? (A + (B * x)) * x
                : (A - (B * x)) * x;
            x += PI_OVER_TWO;
            if (x > PI)
            {
                x -= TWO_PI;
            }

            cos = x < 0
                ? (A + (B * x)) * x
                : (A - (B * x)) * x;
        }

        /// <summary> Atan 2. </summary>
        /// <param name="y"> The y value. </param>
        /// <param name="x"> The x value. </param>
        /// <returns> A float. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Atan2(float y, float x)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (x == 0f)
            {
                if (y > 0f)
                {
                    return PI_OVER_TWO;
                }

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (y == 0f)
                {
                    return 0f;
                }

                return -PI_OVER_TWO;
            }

            float atan, z = y / x;
            if (Math.Abs(z) < 1f)
            {
                atan = z / (1f + (0.28f * z * z));
                if (x < 0f)
                {
                    return atan + (y < 0f
                        ? -PI
                        : PI);
                }

                return atan;
            }

            atan = PI_OVER_TWO - (z / ((z * z) + 0.28f));
            return y < 0f
                ? atan - PI
                : atan;
        }

        #region Clamp

        /// <summary> Clamps the specified value. </summary>
        /// <param name="value"> The value. </param>
        /// <param name="min">   The min. </param>
        /// <param name="max">   The max. </param>
        /// <returns> The result of clamping a value between min and max. </returns>
        public static float Clamp(float value, float min, float max)
        {
            return value < min
                ? min
                : value > max
                    ? max
                    : value;
        }

        /// <summary> Clamps the specified value. </summary>
        /// <param name="value"> The value. </param>
        /// <param name="min">   The min. </param>
        /// <param name="max">   The max. </param>
        /// <returns> The result of clamping a value between min and max. </returns>
        public static int Clamp(int value, int min, int max)
        {
            return value < min
                ? min
                : value > max
                    ? max
                    : value;
        }

        #endregion

        #region RoundUpToPowerOfTwo

        /// <summary>
        ///     Rounds the given value up to a power of two. If it is already a power of two, it is returned unchanged. If it
        ///     is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"> . </param>
        /// <returns> An int. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpToPowerOfTwo(int value)
        {
            return value <= 0
                ? 0
                : (int)RoundUpToPowerOfTwo((uint)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two. If it is already a power of two, or zero, it is returned
        ///     unchanged.
        /// </summary>
        /// <param name="value"> . </param>
        /// <returns> An uint. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
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
        ///     Rounds the given value up to a power of two. If it is already a power of two, it is returned unchanged. If it
        ///     is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"> . </param>
        /// <returns> A long. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RoundUpToPowerOfTwo(long value)
        {
            return value <= 0
                ? 0
                : (long)RoundUpToPowerOfTwo((ulong)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two. If it is already a power of two, or zero, it is returned
        ///     unchanged.
        /// </summary>
        /// <param name="value"> . </param>
        /// <returns> An ulong. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
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

        #endregion

        #region SQRT

        /// <summary> Returns the square root of a specified number. </summary>
        /// <param name="value"> The number whose square root is to be found. </param>
        /// <returns> the square root. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Sqrt(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Can't compute Sqrt of a negative");
            }

            return Sqrt((ulong)value);
        }

        /// <summary> Returns the square root of a specified number. </summary>
        /// <param name="value"> The number whose square root is to be found. </param>
        /// <returns> the square root. </returns>
        public static uint Sqrt(ulong value)
        {
            if (value == 0)
            {
                return 0;
            }

            uint g     = 0;
            int  bshft = Log2Floor(value) >> 1;
            uint b     = 1u << bshft;
            do
            {
                ulong temp = (ulong)(g + g + b) << bshft;

                if (value >= temp)
                {
                    g     += b;
                    value -= temp;
                }

                b >>= 1;
            }
            while (bshft-- > 0);

            return g;
        }

        /// <summary> Returns the square root of a specified number. </summary>
        /// <param name="value"> The number whose square root is to be found. </param>
        /// <returns> the square root. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Sqrt(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Can't compute Sqrt of a negative");
            }

            return (int)Sqrt((uint)value);
        }

        /// <summary> Returns the square root of a specified number. </summary>
        /// <param name="value"> The number whose square root is to be found. </param>
        /// <returns> the square root. </returns>
        public static uint Sqrt(uint value)
        {
            if (value == 0)
            {
                return 0;
            }

            uint g     = 0;
            int  bshft = Log2Floor(value) >> 1;
            uint b     = 1u << bshft;
            do
            {
                uint temp = (g + g + b) << bshft;
                if (value >= temp)
                {
                    g     += b;
                    value -= temp;
                }

                b >>= 1;
            }
            while (bshft-- > 0);

            return g;
        }

        /// <summary> Returns the approximated square root of a specified number. </summary>
        /// <param name="value"> The number whose square root is to be found. </param>
        /// <returns> the approximation of the square root. </returns>
        public static unsafe float SqrtFast(float value)
        {
            uint i = *(uint*)&value;

            // adjust bias
            i += 127 << 23;

            // approximation of square root
            i >>= 1;

            return *(float*)&i;
        }

        /// <summary> Fast inverse sqrt. </summary>
        /// <param name="number"> The number whose inverse square root is to be found. </param>
        /// <returns> A float. </returns>
        public static float FastInverseSqrt(float number)
        {
            const float    THREEHALFS = 1.5F;
            FastISqrtUnion s;
            s.I = 0; //use of possibly unassigned field 'I' if we doesn't set it here to 0
            s.F = number;
            s.I = 0x5f3759df - (s.I >> 1);
            return s.F * (THREEHALFS - (number * 0.5F * s.F * s.F));
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct FastISqrtUnion
        {
            /// <summary> The float to process. </summary>
            [FieldOffset(0)]
            public float F;

            /// <summary> Zero-based index of the. </summary>
            [FieldOffset(0)]
            public int I;
        }

        #endregion

        #region Log2Floor

        /// <summary> Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9. </summary>
        /// <param name="x"> value. </param>
        /// <returns> An int. </returns>
        /// <remarks> The return value is -1 for an input of zero (for which the logarithm is technically undefined.) </remarks>
        public static int Log2Floor(uint x)
        {
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            return CountOnes(x | (x >> 16)) - 1;
        }

        /// <summary> Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9. </summary>
        /// <param name="x"> value. </param>
        /// <returns> An int. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <remarks> The return value is -1 for an input of zero (for which the logarithm is technically undefined.) </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(int x)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Can't compute Log2Floor of a negative");
            }

            return Log2Floor((uint)x);
        }

        /// <summary> Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9. </summary>
        /// <param name="x"> value. </param>
        /// <returns> An int. </returns>
        /// <remarks> The return value is -1 for an input of zero (for which the logarithm is technically undefined.) </remarks>
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

        /// <summary> Returns the floor of the base-2 logarithm of x. e.g. 1024 -> 10, 1000 -> 9. </summary>
        /// <param name="x"> value. </param>
        /// <returns> An int. </returns>
        /// <exception cref="ArgumentOutOfRangeException"> Thrown when one or more arguments are outside the required range. </exception>
        /// <remarks> The return value is -1 for an input of zero (for which the logarithm is technically undefined.) </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Log2Floor(long x)
        {
            if (x < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(x), "Can't compute Log2Floor of a negative");
            }

            return Log2Floor((ulong)x);
        }

        #endregion

        #region Lerp

        /// <summary> Interpolates between two values using a linear function by a given amount. </summary>
        /// <param name="a"> Value to interpolate from. </param>
        /// <param name="b"> Value to interpolate to. </param>
        /// <param name="t"> Interpolation amount. </param>
        /// <returns> The result of linear interpolation of values based on the amount. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Lerp(byte a, byte b, double t)
        {
            return (byte)(a + (t * (b - a)));
        }

        /// <summary> Interpolates between two values using a linear function by a given amount. </summary>
        /// <param name="a"> Value to interpolate from. </param>
        /// <param name="b"> Value to interpolate to. </param>
        /// <param name="t"> Interpolation amount. </param>
        /// <returns> The result of linear interpolation of values based on the amount. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, double t)
        {
            return (float)(a + (t * (b - a)));
        }

        /// <summary> Interpolates between two values using a linear function by a given amount. </summary>
        /// <param name="a"> Value to interpolate from. </param>
        /// <param name="b"> Value to interpolate to. </param>
        /// <param name="t"> Interpolation amount. </param>
        /// <returns> The result of linear interpolation of values based on the amount. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, double t)
        {
            return a + (t * (b - a));
        }

        /// <summary> LinearInterpolate. </summary>
        /// <param name="a"> a. </param>
        /// <param name="b"> b. </param>
        /// <param name="t"> t. </param>
        /// <returns> new Vector2. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }

        #endregion

        #region SmoothStep

        /// <summary> Performs smooth (cubic Hermite) interpolation between 0 and 1. </summary>
        /// <param name="amount"> Value between 0 and 1 indicating interpolation amount. </param>
        /// <returns> A float. </returns>
        /// <remarks> See https://en.wikipedia.org/wiki/Smoothstep. </remarks>
        public static float SmoothStep(float amount)
        {
            return (amount <= 0)
                ? 0
                : (amount >= 1)
                    ? 1
                    : amount * amount * (3 - (2 * amount));
        }

        /// <summary> Performs a smooth(er) interpolation between 0 and 1 with 1st and 2nd order derivatives of zero at endpoints. </summary>
        /// <param name="amount"> Value between 0 and 1 indicating interpolation amount. </param>
        /// <returns> A float. </returns>
        /// <remarks> See https://en.wikipedia.org/wiki/Smoothstep. </remarks>
        public static float SmootherStep(float amount)
        {
            return (amount <= 0)
                ? 0
                : (amount >= 1)
                    ? 1
                    : amount * amount * amount * (amount * ((amount * 6) - 15) + 10);
        }

        #endregion

        #region Wrap

        /// <summary> Wraps the specified value into a range [min, max]. </summary>
        /// <param name="value"> The value to wrap. </param>
        /// <param name="min">   The min. </param>
        /// <param name="max">   The max. </param>
        /// <returns> Result of the wrapping. </returns>
        /// <exception cref="ArgumentException"> Is thrown when <paramref name="min" /> is greater than <paramref name="max" />. </exception>
        public static int Wrap(int value, int min, int max)
        {
            if (min > max)
            {
                throw new ArgumentException(
                    $"min {min} should be less than or equal to max {max}",
                    nameof(min));
            }

            // Code from http://stackoverflow.com/a/707426/1356325
            int range_size = max - min + 1;

            if (value < min)
            {
                value += range_size * ((min - value) / range_size + 1);
            }

            return min + (value - min) % range_size;
        }

        /// <summary> Wraps the specified value into a range [min, max[. </summary>
        /// <param name="value"> The value. </param>
        /// <param name="min">   The min. </param>
        /// <param name="max">   The max. </param>
        /// <returns> Result of the wrapping. </returns>
        /// <exception cref="ArgumentException"> Is thrown when <paramref name="min" /> is greater than <paramref name="max" />. </exception>
        public static float Wrap(float value, float min, float max)
        {
            if (NearEqual(min, max))
            {
                return min;
            }

            double mind   = min;
            double maxd   = max;
            double valued = value;

            if (mind > maxd)
            {
                throw new ArgumentException(
                    $"min {min} should be less than or equal to max {max}",
                    nameof(min));
            }

            double range_size = maxd - mind;
            return (float)(mind + (valued - mind) - (range_size * Math.Floor((valued - mind) / range_size)));
        }

        #endregion

        #region Map

        /// <summary> Maps a value from l1 to u1 to l2 to u2. </summary>
        /// <param name="v">  Value. </param>
        /// <param name="l1"> Lower 1. </param>
        /// <param name="u1"> Upper 1. </param>
        /// <param name="l2"> Lower 2. </param>
        /// <param name="u2"> Upper 2. </param>
        /// <returns> maped value. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float v, float l1, float u1, float l2, float u2)
        {
            return (((v - l1) / (u1 - l1)) * (u2 - l2)) + l2;
        }

        /// <summary> Maps a value from l1 to u1 to l2 to u2. </summary>
        /// <param name="v">  Value. </param>
        /// <param name="l1"> Lower 1. </param>
        /// <param name="u1"> Upper 1. </param>
        /// <param name="l2"> Lower 2. </param>
        /// <param name="u2"> Upper 2. </param>
        /// <returns> maped value. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Map(double v, double l1, double u1, double l2, double u2)
        {
            return (((v - l1) / (u1 - l1)) * (u2 - l2)) + l2;
        }

        #endregion

        #region CountOnes

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        public static int CountOnes(byte x)
        {
            int y = x;
            y -= (y >> 1) & 0x55;
            y =  ((y >> 2) & 0x33) + (y & 0x33);
            return (y & 0x0F) + (y >> 4);
        }

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        public static int CountOnes(ushort x)
        {
            int y = x;
            y -= (y >> 1) & 0x5555;
            y =  ((y >> 2) & 0x3333) + (y & 0x3333);
            y =  ((y >> 4) + y) & 0x0F0F;
            return (y + (y >> 8)) & 0x001F;
        }

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnes(int x)
        {
            return CountOnes((uint)x);
        }

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        public static int CountOnes(uint x)
        {
            x -= (x >> 1) & 0x55555555;
            x =  ((x >> 2) & 0x33333333) + (x & 0x33333333);
            x =  ((x >> 4) + x) & 0x0F0F0F0F;
            x += x >> 8;
            return (int)((x + (x >> 16)) & 0x0000003F);
        }

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOnes(long x)
        {
            return CountOnes((ulong)x);
        }

        /// <summary> Returns the number of 'on' bits in x. </summary>
        /// <param name="x"> value. </param>
        /// <returns> The total number of ones. </returns>
        public static int CountOnes(ulong x)
        {
            x -= (x >> 1) & 0x5555555555555555u;
            x =  ((x >> 2) & 0x3333333333333333u) + (x & 0x3333333333333333u);
            x =  ((x >> 4) + x) & 0x0F0F0F0F0F0F0F0Fu;
            x += x >> 8;
            x += x >> 16;
            return ((int)x + (int)(x >> 32)) & 0x0000007F;
        }

        #endregion
    }
}
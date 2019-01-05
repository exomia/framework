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
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Mathematics.Extensions.Numeric
{
    /// <summary>
    ///     adds extensions for numerical types
    /// </summary>
    public static class NumericExtensions
    {
        private const double PI_OVER_180_D = Math.PI / 180.0;
        private const float PI_OVER_180_F = (float)(Math.PI / 180.0);

        private const double I80_OVER_PI_D = 180.0 / Math.PI;
        private const float I80_OVER_PI_F = (float)(180.0 / Math.PI);

        /// <summary>
        ///     Convert to Degrees.
        /// </summary>
        /// <param name="value">The value to convert to degrees</param>
        /// <returns>The value in degrees</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegree(this double value)
        {
            return value * I80_OVER_PI_D;
        }

        /// <summary>
        ///     Convert to Degrees.
        /// </summary>
        /// <param name="value">The value to convert to degrees</param>
        /// <returns>The value in degrees</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegree(this float value)
        {
            return value * I80_OVER_PI_F;
        }

        /// <summary>
        ///     Convert to Radians.
        /// </summary>
        /// <param name="value">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this double value)
        {
            return PI_OVER_180_D * value;
        }

        /// <summary>
        ///     Convert to Radians.
        /// </summary>
        /// <param name="value">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(this float value)
        {
            return PI_OVER_180_F * value;
        }

        /// <summary>
        ///     Convert to Radians.
        /// </summary>
        /// <param name="value">The value to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this int value)
        {
            return PI_OVER_180_D * value;
        }
    }
}
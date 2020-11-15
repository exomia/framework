#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     A 2d line.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 16)]
    public readonly struct Line2 : IFormattable
    {
        /// <summary>
        ///     The first x value.
        /// </summary>
        public readonly float X1;

        /// <summary>
        ///     The second x value.
        /// </summary>
        public readonly float X2;

        /// <summary>
        ///     The first y value.
        /// </summary>
        public readonly float Y1;

        /// <summary>
        ///     The second y value.
        /// </summary>
        public readonly float Y2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="x1"> The first x value. </param>
        /// <param name="y1"> The first y value. </param>
        /// <param name="x2"> The second x value. </param>
        /// <param name="y2"> The second y value. </param>
        public Line2(float x1, float y1, float x2, float y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="a"> The VectorI2 to process. </param>
        /// <param name="b"> The VectorI2 to process. </param>
        public Line2(VectorI2 a, VectorI2 b)
            : this(a.X, a.Y, b.X, b.Y) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="a"> The Vector2 to process. </param>
        /// <param name="b"> The Vector2 to process. </param>
        public Line2(Vector2 a, Vector2 b)
            : this(a.X, a.Y, b.X, b.Y) { }

        /// <summary>
        ///     Determines whether the specified <see cref="Line2" /> is equal to this instance.
        /// </summary>
        /// <param name="other"> The <see cref="Line2" /> to compare with this instance. </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Line2" /> is equal to this instance; <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Line2 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return
                X1 == other.X1 &&
                Y1 == other.Y1 &&
                X2 == other.X2 &&
                Y2 == other.Y2;

            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object value)
        {
            if (value is Line2 other)
            {
                return Equals(in other);
            }
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (((((X1.GetHashCode() * 307) ^ Y1.GetHashCode()) * 521) ^ X2.GetHashCode()) * 853) ^
                   Y2.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3}", X1, Y1, X2, Y2);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string? format)
        {
            if (format == null)
            {
                return ToString();
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3}",
                X1.ToString(format, CultureInfo.CurrentCulture),
                Y1.ToString(format, CultureInfo.CurrentCulture),
                X2.ToString(format, CultureInfo.CurrentCulture),
                Y2.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider"> The format provider. </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3}", X1, Y1, X2, Y2);
        }

        /// <inheritdoc />
        public string ToString(string? format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(
                formatProvider,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3}",
                X1.ToString(format, formatProvider),
                Y1.ToString(format, formatProvider),
                X2.ToString(format, formatProvider),
                Y2.ToString(format, formatProvider));
        }
    }
}
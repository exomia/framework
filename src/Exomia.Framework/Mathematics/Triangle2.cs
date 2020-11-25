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
    ///     A 2d triangle.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 24)]
    public readonly struct Triangle2 : IFormattable
    {
        /// <summary>
        ///     The first x value.
        /// </summary>
        [FieldOffset(0)]
        public readonly float X1;

        /// <summary>
        ///     The first y value.
        /// </summary>
        [FieldOffset(4)]
        public readonly float Y1;

        /// <summary>
        ///     The first xy.
        /// </summary>
        [FieldOffset(0)]
        public readonly Vector2 XY1;

        /// <summary>
        ///     The second y value.
        /// </summary>
        [FieldOffset(8)]
        public readonly float X2;

        /// <summary>
        ///     The second x value.
        /// </summary>
        [FieldOffset(12)]
        public readonly float Y2;

        /// <summary>
        ///     The second xy.
        /// </summary>
        [FieldOffset(8)]
        public readonly Vector2 XY2;

        /// <summary>
        ///     The third x value.
        /// </summary>
        [FieldOffset(16)]
        public readonly float X3;

        /// <summary>
        ///     The third y value.
        /// </summary>
        [FieldOffset(20)]
        public readonly float Y3;

        /// <summary>
        ///     The third xy.
        /// </summary>
        [FieldOffset(16)]
        public readonly Vector2 XY3;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Triangle2" /> struct.
        /// </summary>
        /// <param name="x1"> The first x value. </param>
        /// <param name="y1"> The first y value. </param>
        /// <param name="x2"> The second x value. </param>
        /// <param name="y2"> The second y value. </param>
        /// <param name="x3"> The third x value. </param>
        /// <param name="y3"> The third y value. </param>
        public Triangle2(float x1, float y1, float x2, float y2, float x3, float y3)
            : this()
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
            X3 = x3;
            Y3 = y3;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Triangle2" /> struct.
        /// </summary>
        /// <param name="a"> The <see cref="VectorI2" /> to process. </param>
        /// <param name="b"> The <see cref="VectorI2" /> to process. </param>
        /// <param name="c"> The <see cref="VectorI2" /> to process. </param>
        public Triangle2(in VectorI2 a, in VectorI2 b, in VectorI2 c)
            : this(a.X, a.Y, b.X, b.Y, c.X, c.Y) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Triangle2" /> struct.
        /// </summary>
        /// <param name="a"> The <see cref="Vector2" /> to process. </param>
        /// <param name="b"> The <see cref="Vector2" /> to process. </param>
        /// <param name="c"> The <see cref="Vector2" /> to process. </param>
        public Triangle2(in Vector2 a, in Vector2 b, in Vector2 c)
            : this()
        {
            XY1 = a;
            XY2 = b;
            XY3 = c;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Triangle2" /> is equal to this instance.
        /// </summary>
        /// <param name="other"> The <see cref="Triangle2" /> to compare with this instance. </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Triangle2" /> is equal to this instance; <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Triangle2 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return
                X1 == other.X1 &&
                Y1 == other.Y1 &&
                X2 == other.X2 &&
                Y2 == other.Y2 &&
                X3 == other.X3 &&
                Y3 == other.Y3;

            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object value)
        {
            return value is Triangle2 other && Equals(in other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (((((((((X1.GetHashCode() * 307) ^ Y1.GetHashCode()) * 521) ^ X2.GetHashCode()) * 853) ^
                       Y2.GetHashCode()) * 443) ^ X3.GetHashCode()) * 937) ^ Y3.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3} | X3:{4} Y3:{5}",
                X1, Y1, X2, Y2, X3, Y3);
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
                "X1:{0} Y1:{1} | X2:{2} Y2:{3} | X3:{4} Y3:{5}",
                X1.ToString(format, CultureInfo.CurrentCulture),
                Y1.ToString(format, CultureInfo.CurrentCulture),
                X2.ToString(format, CultureInfo.CurrentCulture),
                Y2.ToString(format, CultureInfo.CurrentCulture),
                X3.ToString(format, CultureInfo.CurrentCulture),
                Y3.ToString(format, CultureInfo.CurrentCulture));
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
                "X1:{0} Y1:{1} | X2:{2} Y2:{3} | X3:{4} Y3:{5}",
                X1, Y1, X2, Y2, X3, Y3);
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
                "X1:{0} Y1:{1} | X2:{2} Y2:{3} | X3:{4} Y3:{5}",
                X1.ToString(format, formatProvider),
                Y1.ToString(format, formatProvider),
                X2.ToString(format, formatProvider),
                Y2.ToString(format, formatProvider),
                X3.ToString(format, formatProvider),
                Y3.ToString(format, formatProvider));
        }

        /// <summary>
        ///     Rotates the triangle around the given <paramref name="origin" />.
        /// </summary>
        /// <param name="triangle"> The triangle. </param>
        /// <param name="rotation"> The rotation (in radians). </param>
        /// <param name="origin">   The origin. </param>
        /// <returns>
        ///     A new <see cref="Triangle2" />.
        /// </returns>
        public static Triangle2 RotateAround(in Triangle2 triangle, float rotation, in Vector2 origin)
        {
            double sin = Math.Sin(rotation);
            double cos = Math.Cos(rotation);

            float x1 = triangle.X1 - origin.X;
            float y1 = triangle.Y1 - origin.Y;

            float x2 = triangle.X2 - origin.X;
            float y2 = triangle.Y2 - origin.Y;

            float x3 = triangle.X3 - origin.X;
            float y3 = triangle.Y3 - origin.Y;

            return new Triangle2(
                (float)((x1 * cos) - (y1 * sin)) + origin.X, (float)((x1 * sin) + (y1 * cos)) + origin.Y,
                (float)((x2 * cos) - (y2 * sin)) + origin.X, (float)((x2 * sin) + (y2 * cos)) + origin.Y,
                (float)((x3 * cos) - (y3 * sin)) + origin.X, (float)((x3 * sin) + (y3 * cos)) + origin.Y);
        }
    }
}
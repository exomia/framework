#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     A 2d line.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 4, Size = 16)]
    public readonly struct Line2
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
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="x1"> The first x value. </param>
        /// <param name="y1"> The first y value. </param>
        /// <param name="x2"> The second x value. </param>
        /// <param name="y2"> The second y value. </param>
        public Line2(float x1, float y1, float x2, float y2)
            : this()
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="a"> [in,out] The <see cref="VectorI2" /> to process. </param>
        /// <param name="b"> [in,out] The <see cref="VectorI2" /> to process. </param>
        public Line2(in VectorI2 a, in VectorI2 b)
            : this(a.X, a.Y, b.X, b.Y) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Line2" /> struct.
        /// </summary>
        /// <param name="a"> [in,out] The <see cref="Vector2" /> to process. </param>
        /// <param name="b"> [in,out] The <see cref="Vector2" /> to process. </param>
        public Line2(in Vector2 a, in Vector2 b)
            : this()
        {
            XY1 = a;
            XY2 = b;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (((((X1.GetHashCode() * 307) ^ Y1.GetHashCode()) * 521) ^ X2.GetHashCode()) * 853) ^
                   Y2.GetHashCode();
        }

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
        public override bool Equals(object? value)
        {
            return value is Line2 other && Equals(in other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X1:{0} Y1:{1} | X2:{2} Y2:{3}",
                X1.ToString(CultureInfo.CurrentCulture),
                Y1.ToString(CultureInfo.CurrentCulture),
                X2.ToString(CultureInfo.CurrentCulture),
                Y2.ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Intersect with other <see cref="Line2" />.
        /// </summary>
        /// <param name="other">             The <see cref="Line2" /> to compare with this instance. </param>
        /// <param name="intersectionPoint"> [out] The intersection point. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool IntersectWith(in Line2 other, out Vector2 intersectionPoint)
        {
            float a1 = Y2 - Y1;
            float b1 = X1 - X2;
            float c1 = (a1 * X1) + (b1 * Y1);

            float a2 = other.Y2 - other.Y1;
            float b2 = other.X1 - other.X2;
            float c2 = (a2 * other.X1) + (b2 * other.Y1);

            float det = (a1 * b2) - (a2 * b1);

            if (det == 0.0f)
            {
                intersectionPoint = default;
                return false;
            }

            intersectionPoint.X = ((b2 * c1) - (b1 * c2)) / det;
            intersectionPoint.Y = ((a1 * c2) - (a2 * c1)) / det;
            return true;
        }

        /// <summary>
        ///     Gets a perpendicular from this line.
        /// </summary>
        /// <param name="offset"> The offset. </param>
        /// <returns>
        ///     The perpendicular.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Line2 GetPerpendicular(float offset)
        {
            float dx = X2 - X1;
            float dy = Y2 - Y1;

            double dl = Math.Sqrt((dx * dx) + (dy * dy));
            float  nx = (float)((dy / dl) * offset);
            float  ny = (float)((dx / dl) * offset);

            return new Line2(X1 - nx, Y1 + ny, X2 - nx, Y2 + ny);
        }

        /// <summary>
        ///     Rotates the line around the given <paramref name="origin" />.
        /// </summary>
        /// <param name="line"> The line. </param>
        /// <param name="rotation"> The rotation (in radians). </param>
        /// <param name="origin">   The origin. </param>
        /// <returns>
        ///     A new <see cref="Line2" />.
        /// </returns>
        public static Line2 RotateAround(in Line2 line, float rotation, in Vector2 origin)
        {
            double sin = Math.Sin(rotation);
            double cos = Math.Cos(rotation);

            float x1 = line.X1 - origin.X;
            float y1 = line.Y1 - origin.Y;

            float x2 = line.X2 - origin.X;
            float y2 = line.Y2 - origin.Y;

            return new Line2(
                (float)((x1 * cos) - (y1 * sin)) + origin.X, (float)((x1 * sin) + (y1 * cos)) + origin.Y,
                (float)((x2 * cos) - (y2 * sin)) + origin.X, (float)((x2 * sin) + (y2 * cos)) + origin.Y);
        }

        /// <summary>
        ///     Creates a line out of <paramref name="p1" /> and <paramref name="p2" />, as well the perpendicular from this line.
        /// </summary>
        /// <param name="p1">            [in,out] The first ref <see cref="Vector2" />. </param>
        /// <param name="p2">            [in,out] The second ref <see cref="Vector2" />. </param>
        /// <param name="offset">        The offset. </param>
        /// <param name="perpendicular"> [out] The perpendicular. </param>
        /// <returns>
        ///     The line created from <paramref name="p1" /> and <paramref name="p2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Line2 CreateWithPerpendicular(ref Vector2 p1,
                                                    ref Vector2 p2,
                                                    float       offset,
                                                    out Line2   perpendicular)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;

            double dl = Math.Sqrt((dx * dx) + (dy * dy));
            float  nx = (float)((dy / dl) * offset);
            float  ny = (float)((dx / dl) * offset);

            perpendicular = new Line2(p1.X - nx, p1.Y + ny, p2.X - nx, p2.Y + ny);
            return new Line2(p1.X, p1.Y, p2.X, p2.Y);
        }
    }
}
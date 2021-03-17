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

namespace Exomia.Framework.Core.Mathematics
{
    /// <summary>
    ///     A 2d arc.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 20)]
    public readonly struct Arc2
    {
        /// <summary>
        ///     The x value.
        /// </summary>
        public readonly float X; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary>
        ///     The y value.
        /// </summary>
        public readonly float Y; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary>
        ///     The radius.
        /// </summary>
        public readonly float Radius; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary>
        ///     The start angle in radians.
        /// </summary>
        public readonly float Start; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary>
        ///     The end angle in radians.
        /// </summary>
        public readonly float End; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary> Initializes a new instance of the <see cref="Arc2" /> struct. </summary>
        /// <param name="x">      The x value. </param>
        /// <param name="y">      The y value. </param>
        /// <param name="radius"> The radius. </param>
        /// <param name="start">  (Optional) The start angle in radians. </param>
        /// <param name="end">    (Optional) The end angle in radians. </param>
        public Arc2(float x, float y, float radius, float start = 0, float end = Math2.TWO_PI)
        {
            X      = x;
            Y      = y;
            Radius = radius;
            Start  = start;
            End    = end;
        }

        /// <summary> Initializes a new instance of the <see cref="Arc2" /> struct. </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        /// <param name="start">  (Optional) The start angle in radians. </param>
        /// <param name="end">    (Optional) The end angle in radians. </param>
        public Arc2(VectorI2 center, float radius, float start = 0, float end = Math2.TWO_PI)
            : this(center.X, center.Y, radius, start, end) { }

        /// <summary> Initializes a new instance of the <see cref="Arc2" /> struct. </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        /// <param name="start">  (Optional) The start angle in radians. </param>
        /// <param name="end">    (Optional) The end angle in radians. </param>
        public Arc2(Vector2 center, float radius, float start = 0, float end = Math2.TWO_PI)
            : this(center.X, center.Y, radius, start, end) { }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (((((((X.GetHashCode() * 307) ^ Y.GetHashCode()) * 521) ^ Radius.GetHashCode()) * 853) ^
                     Start.GetHashCode()) * 443) ^ End.GetHashCode();
        }

        /// <summary> Determines whether the specified <see cref="Arc2" /> is equal to this instance. </summary>
        /// <param name="other"> The <see cref="Line2" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cref="Arc2" /> is equal to this instance; <c>false</c> otherwise. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Arc2 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return
                X == other.X &&
                Y == other.Y &&
                Radius == other.Radius &&
                Start == other.Start &&
                End == other.End;

            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? value)
        {
            return value is Arc2 other && Equals(in other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1} | Radius:{2}, Start:{3}, End:{4}",
                X.ToString(CultureInfo.CurrentCulture),
                Y.ToString(CultureInfo.CurrentCulture),
                Radius.ToString(CultureInfo.CurrentCulture),
                Start.ToString(CultureInfo.CurrentCulture),
                End.ToString(CultureInfo.CurrentCulture));
        }
    }
}
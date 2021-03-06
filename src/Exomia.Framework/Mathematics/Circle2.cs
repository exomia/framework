﻿#region License

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
    ///     A 2d circle.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    public readonly struct Circle2 : IFormattable
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
        ///     Initializes a new instance of the <see cref="Circle2" /> struct.
        /// </summary>
        /// <param name="x">      The x value. </param>
        /// <param name="y">      The y value. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(float x, float y, float radius)
        {
            X      = x;
            Y      = y;
            Radius = radius;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle2" /> struct.
        /// </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(VectorI2 center, float radius)
            : this(center.X, center.Y, radius) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Circle2" /> struct.
        /// </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(Vector2 center, float radius)
            : this(center.X, center.Y, radius) { }

        /// <summary>
        ///     Determines whether the specified <see cref="Circle2" /> is equal to this instance.
        /// </summary>
        /// <param name="other"> The <see cref="Line2" /> to compare with this instance. </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Circle2" /> is equal to this instance; <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Circle2 other)
        {
            // ReSharper disable CompareOfFloatsByEqualityOperator
            return
                X == other.X &&
                Y == other.Y &&
                Radius == other.Radius;

            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object value)
        {
            return value is Circle2 other && Equals(in other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (((X.GetHashCode() * 307) ^ Y.GetHashCode()) * 521) ^ Radius.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1} | Radius:{2}", X, Y, Radius);
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
                "X:{0} Y:{1} | Radius:{2}",
                X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture),
                Radius.ToString(format, CultureInfo.CurrentCulture));
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
                "X:{0} Y:{1} | Radius:{2}", X, Y, Radius);
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
                "X:{0} Y:{1} | Radius:{2}",
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider),
                Radius.ToString(format, formatProvider));
        }
    }
}
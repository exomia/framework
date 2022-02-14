#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Core.Mathematics
{
    /// <summary> A 2d circle. </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    public readonly struct Circle2
    {
        /// <summary> The x value. </summary>
        public readonly float X; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary> The y value. </summary>
        public readonly float Y; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary> The radius. </summary>
        public readonly float Radius; //Note: do not reorder this field, unless you know what you are doing.

        /// <summary> Initializes a new instance of the <see cref="Circle2" /> struct. </summary>
        /// <param name="x">      The x value. </param>
        /// <param name="y">      The y value. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(float x, float y, float radius)
        {
            X      = x;
            Y      = y;
            Radius = radius;
        }

        /// <summary> Initializes a new instance of the <see cref="Circle2" /> struct. </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(VectorI2 center, float radius)
            : this(center.X, center.Y, radius) { }

        /// <summary> Initializes a new instance of the <see cref="Circle2" /> struct. </summary>
        /// <param name="center"> The center. </param>
        /// <param name="radius"> The radius. </param>
        public Circle2(Vector2 center, float radius)
            : this(center.X, center.Y, radius) { }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (((X.GetHashCode() * 307) ^ Y.GetHashCode()) * 521) ^ Radius.GetHashCode();
        }

        /// <summary> Determines whether the specified <see cref="Circle2" /> is equal to this instance. </summary>
        /// <param name="other"> The <see cref="Line2" /> to compare with this instance. </param>
        /// <returns> <c>true</c> if the specified <see cref="Circle2" /> is equal to this instance; <c>false</c> otherwise. </returns>
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
        public override bool Equals(object? value)
        {
            return value is Circle2 other && Equals(in other);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1} | Radius:{2}",
                X.ToString(CultureInfo.CurrentCulture),
                Y.ToString(CultureInfo.CurrentCulture),
                Radius.ToString(CultureInfo.CurrentCulture));
        }
    }
}
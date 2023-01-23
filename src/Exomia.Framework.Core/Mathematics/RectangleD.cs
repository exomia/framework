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

namespace Exomia.Framework.Core.Mathematics;

/// <summary>
///     A RectangleD class.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RectangleD
{
    /// <summary> An empty rectangle. </summary>
    public static readonly RectangleD Empty;

    /// <summary> An infinite rectangle. </summary>
    public static readonly RectangleD Infinite = new()
    {
        Left   = double.NegativeInfinity,
        Top    = double.NegativeInfinity,
        Right  = double.PositiveInfinity,
        Bottom = double.PositiveInfinity
    };

    /// <summary> The left. </summary>
    public double Left;

    /// <summary> The top. </summary>
    public double Top;

    /// <summary> The right. </summary>
    public double Right;

    /// <summary> The bottom. </summary>
    public double Bottom;

    /// <summary> Initializes a new instance of the <see cref="RectangleD" /> struct. </summary>
    /// <param name="left"> The left. </param>
    /// <param name="top"> The top. </param>
    /// <param name="right"> The right. </param>
    /// <param name="bottom"> The bottom. </param>
    public RectangleD(double left, double top, double right, double bottom)
    {
        Left   = left;
        Top    = top;
        Right  = right;
        Bottom = bottom;
    }

    /// <summary> Returns a hash code for this instance. </summary>
    /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override int GetHashCode()
    {
        unchecked
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            int result = Left.GetHashCode();
            result = (result * 397) ^ Top.GetHashCode();
            result = (result * 397) ^ Right.GetHashCode();
            result = (result * 397) ^ Bottom.GetHashCode();
            return result;

            // ReSharper enable NonReadonlyMemberInGetHashCode
        }
    }

    /// <summary> Determines whether the specified <see cref="System.Object" /> is equal to this instance. </summary>
    /// <param name="value"> The <see cref="System.Object" /> to compare with this instance. </param>
    /// <returns> true if the specified <see cref="System.Object" /> is equal to this instance; otherwise, false. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly override bool Equals(object? value)
    {
        return value is RectangleD other && Equals(in other);
    }

    /// <summary> Determines whether the specified <see cref="RectangleD" /> is equal to this instance. </summary>
    /// <param name="other"> The <see cref="RectangleD" /> to compare with this instance. </param>
    /// <returns> true if the specified <see cref="RectangleD" /> is equal to this instance; otherwise, false. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Equals(in RectangleD other)
    {
        return Math2.NearEqual(other.Left, Left)  &&
            Math2.NearEqual(other.Right,   Right) &&
            Math2.NearEqual(other.Top,     Top)   &&
            Math2.NearEqual(other.Bottom,  Bottom);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "Left:{0} Top:{1} Right:{2} Bottom:{3}",
            Left.ToString(CultureInfo.InvariantCulture),
            Top.ToString(CultureInfo.InvariantCulture),
            Right.ToString(CultureInfo.InvariantCulture),
            Bottom.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary> Implements the operator ==. </summary>
    /// <param name="left"> The left. </param>
    /// <param name="right"> The right. </param>
    /// <returns> The result of the operator. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(in RectangleD left, in RectangleD right)
    {
        return left.Equals(in right);
    }

    /// <summary> Implements the operator !=. </summary>
    /// <param name="left"> The left. </param>
    /// <param name="right"> The right. </param>
    /// <returns> The result of the operator. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(in RectangleD left, in RectangleD right)
    {
        return !left.Equals(in right);
    }
}
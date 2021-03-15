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
    ///     A RectangleF class.
    ///     This structure is slightly different from System.Drawing.RectangleF as it is
    ///     internally storing Left, Top, Right, Bottom instead of Left, Top, Width, Height.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RectangleF
    {
        /// <summary>
        ///     An empty rectangle.
        /// </summary>
        public static readonly RectangleF Empty;

        /// <summary>
        ///     An infinite rectangle. See remarks.
        /// </summary>
        /// <remarks>
        ///     http://msdn.microsoft.com/en-us/library/windows/desktop/dd372261%28v=vs.85%29.aspx
        ///     Any properties that involve computations, like <see cref="Center" />, <see cref="Width" /> or <see cref="Height" />
        ///     may return incorrect results - <see cref="float.NaN" />.
        /// </remarks>
        public static readonly RectangleF Infinite = new()
        {
            Left   = float.NegativeInfinity,
            Top    = float.NegativeInfinity,
            Right  = float.PositiveInfinity,
            Bottom = float.PositiveInfinity
        };

        /// <summary>
        ///     The left.
        /// </summary>
        public float Left;

        /// <summary>
        ///     The top.
        /// </summary>
        public float Top;

        /// <summary>
        ///     The right.
        /// </summary>
        public float Right;

        /// <summary>
        ///     The bottom.
        /// </summary>
        public float Bottom;

        /// <summary>
        ///     Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        public float X
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return Left; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Right = value + Width;
                Left  = value;
            }
        }

        /// <summary>
        ///     Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        public float Y
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return Top; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                Bottom = value + Height;
                Top    = value;
            }
        }

        /// <summary>
        ///     Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return Right - Left; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Right = Left + value; }
        }

        /// <summary>
        ///     Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return Bottom - Top; }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { Bottom = Top + value; }
        }

        /// <summary>
        ///     Gets or sets the location.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        public Vector2 Location
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get { return new(X, Y); }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        ///     Gets the Point that specifies the center of the rectangle.
        /// </summary>
        /// <value>
        ///     The center.
        /// </value>
        public readonly Vector2 Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new(X + Width / 2, Y + Height / 2); }
        }

        /// <summary>
        ///     Gets a value that indicates whether the rectangle is empty.
        /// </summary>
        /// <value>
        ///     <c>true</c> if [is empty]; otherwise, <c>false</c>.
        /// </value>
        public readonly bool IsEmpty
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return Width == 0.0f && Height == 0.0f && X == 0.0f && Y == 0.0f; }
        }

        /// <summary>
        ///     Gets the position of the top-left corner of the rectangle.
        /// </summary>
        /// <value>The top-left corner of the rectangle.</value>
        public readonly Vector2 TopLeft
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new(Left, Top); }
        }

        /// <summary>
        ///     Gets the position of the top-right corner of the rectangle.
        /// </summary>
        /// <value>The top-right corner of the rectangle.</value>
        public readonly Vector2 TopRight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new(Right, Top); }
        }

        /// <summary>
        ///     Gets the position of the bottom-left corner of the rectangle.
        /// </summary>
        /// <value>The bottom-left corner of the rectangle.</value>
        public readonly Vector2 BottomLeft
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new(Left, Bottom); }
        }

        /// <summary>
        ///     Gets the position of the bottom-right corner of the rectangle.
        /// </summary>
        /// <value>The bottom-right corner of the rectangle.</value>
        public readonly Vector2 BottomRight
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return new(Right, Bottom); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RectangleF" /> struct.
        /// </summary>
        /// <param name="x">The left.</param>
        /// <param name="y">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public RectangleF(float x, float y, float width, float height)
        {
            Left   = x;
            Top    = y;
            Right  = x + width;
            Bottom = y + height;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
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

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object? value)
        {
            return value is RectangleF other && Equals(in other);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="RectangleF" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="RectangleF" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="RectangleF" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in RectangleF other)
        {
            return Math2.NearEqual(other.Left,   Left) &&
                   Math2.NearEqual(other.Right,  Right) &&
                   Math2.NearEqual(other.Top,    Top) &&
                   Math2.NearEqual(other.Bottom, Bottom);
        }
        
        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "X:{0} Y:{1} Width:{2} Height:{3}",
                X.ToString(CultureInfo.InvariantCulture),
                Y.ToString(CultureInfo.InvariantCulture),
                Width.ToString(CultureInfo.InvariantCulture),
                Height.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>Changes the position of the rectangle.</summary>
        /// <param name="amount">The values to adjust the position of the rectangle by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(VectorI2 amount)
        {
            Offset(amount.X, amount.Y);
        }

        /// <summary>Changes the position of the rectangle.</summary>
        /// <param name="amount">The values to adjust the position of the rectangle by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(Vector2 amount)
        {
            Offset(amount.X, amount.Y);
        }

        /// <summary>Changes the position of the rectangle.</summary>
        /// <param name="offsetX">Change in the x-position.</param>
        /// <param name="offsetY">Change in the y-position.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Offset(float offsetX, float offsetY)
        {
            X += offsetX;
            Y += offsetY;
        }

        /// <summary>Pushes the edges of the rectangle out by the horizontal and vertical values specified.</summary>
        /// <param name="horizontalAmount">Value to push the sides out by.</param>
        /// <param name="verticalAmount">Value to push the top and bottom out by.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Inflate(float horizontalAmount, float verticalAmount)
        {
            X      -= horizontalAmount;
            Y      -= verticalAmount;
            Width  += horizontalAmount * 2;
            Height += verticalAmount * 2;
        }

        /// <summary>
        ///     Checks, if specified <see cref="Vector2" /> is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="Vector2" />.</param>
        /// <returns><c>true</c> if <see cref="Vector2" /> is inside <see cref="RectangleF" />, otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Vector2 value)
        {
            return value.X >= Left && value.X <= Right && value.Y >= Top && value.Y <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified <see cref="Vector2" /> is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="Vector2" />.</param>
        /// <param name="result">
        ///     [Out] true if the specified <see cref="Vector2" /> is contained within this rectangle; false
        ///     otherwise.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Contains(in Vector2 value, out bool result)
        {
            result = value.X >= Left && value.X <= Right && value.Y >= Top && value.Y <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified <see cref="Rectangle" /> is entirely inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="Rectangle" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Rectangle value)
        {
            return X <= value.X && value.Right <= Right && Y <= value.Y && value.Bottom <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified <see cref="Rectangle" /> is entirely inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="RectangleF" />.</param>
        /// <param name="result">
        ///     [Out] true if the specified <see cref="RectangleF" /> is entirely contained within this rectangle;
        ///     false otherwise.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Contains(in Rectangle value, out bool result)
        {
            result = X <= value.X && value.Right <= Right && Y <= value.Y && value.Bottom <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified <see cref="RectangleF" /> is entirely inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="RectangleF" />.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in RectangleF value)
        {
            return X <= value.X && value.Right <= Right && Y <= value.Y && value.Bottom <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified <see cref="RectangleF" /> is entirely inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="value">The <see cref="RectangleF" />.</param>
        /// <param name="result">
        ///     [Out] true if the specified <see cref="RectangleF" /> is entirely contained within this rectangle;
        ///     false otherwise.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Contains(in RectangleF value, out bool result)
        {
            result = X <= value.X && value.Right <= Right && Y <= value.Y && value.Bottom <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified x and y value is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <returns><c>true</c> if point is inside <see cref="RectangleF" />, otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(int x, int y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified specified x and y value is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <param name="result">
        ///     [Out] true if the specified <see cref="Vector2" /> is contained within this rectangle; false
        ///     otherwise.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Contains(int x, int y, out bool result)
        {
            result = x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified x and y value is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <returns><c>true</c> if point is inside <see cref="RectangleF" />, otherwise <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(float x, float y)
        {
            return x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>
        ///     Checks, if specified specified x and y value is inside <see cref="RectangleF" />.
        /// </summary>
        /// <param name="x">The x value</param>
        /// <param name="y">The y value</param>
        /// <param name="result">
        ///     [Out] true if the specified <see cref="Vector2" /> is contained within this rectangle; false
        ///     otherwise.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Contains(float x, float y, out bool result)
        {
            result = x >= Left && x <= Right && y >= Top && y <= Bottom;
        }

        /// <summary>Determines whether a specified rectangle intersects with this rectangle.</summary>
        /// <param name="value">The rectangle to evaluate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in Rectangle value)
        {
            return value.X < Right && X < value.Right && value.Y < Bottom && Y < value.Bottom;
        }

        /// <summary>
        ///     Determines whether a specified rectangle intersects with this rectangle.
        /// </summary>
        /// <param name="value">The rectangle to evaluate</param>
        /// <param name="result">[OutAttribute] true if the specified rectangle intersects with this one; false otherwise.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Intersects(in Rectangle value, out bool result)
        {
            result = value.X < Right && X < value.Right && value.Y < Bottom && Y < value.Bottom;
        }

        /// <summary>Determines whether a specified rectangle intersects with this rectangle.</summary>
        /// <param name="value">The rectangle to evaluate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Intersects(in RectangleF value)
        {
            return value.X < Right && X < value.Right && value.Y < Bottom && Y < value.Bottom;
        }

        /// <summary>
        ///     Determines whether a specified rectangle intersects with this rectangle.
        /// </summary>
        /// <param name="value">The rectangle to evaluate</param>
        /// <param name="result">[OutAttribute] true if the specified rectangle intersects with this one; false otherwise.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Intersects(in RectangleF value, out bool result)
        {
            result = value.X < Right && X < value.Right && value.Y < Bottom && Y < value.Bottom;
        }

        /// <summary>
        ///     Creates a rectangle defining the area where one rectangle overlaps with another rectangle.
        /// </summary>
        /// <param name="value1">The first Rectangle to compare.</param>
        /// <param name="value2">The second Rectangle to compare.</param>
        /// <returns>The intersection rectangle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Intersect(in RectangleF value1, in RectangleF value2)
        {
            Intersect(in value1, in value2, out RectangleF result);
            return result;
        }

        /// <summary>Creates a rectangle defining the area where one rectangle overlaps with another rectangle.</summary>
        /// <param name="value1">The first rectangle to compare.</param>
        /// <param name="value2">The second rectangle to compare.</param>
        /// <param name="result">[OutAttribute] The area where the two first parameters overlap.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Intersect(in RectangleF value1, in RectangleF value2, out RectangleF result)
        {
            float newLeft = value1.X > value2.X
                ? value1.X
                : value2.X;
            float newTop = value1.Y > value2.Y
                ? value1.Y
                : value2.Y;
            float newRight = value1.Right < value2.Right
                ? value1.Right
                : value2.Right;
            float newBottom = value1.Bottom < value2.Bottom
                ? value1.Bottom
                : value2.Bottom;
            if (newRight > newLeft && newBottom > newTop)
            {
                result.Left   = newLeft;
                result.Top    = newTop;
                result.Right  = newRight;
                result.Bottom = newBottom;
            }
            else
            {
                result = Empty;
            }
        }

        /// <summary>
        ///     Creates a new rectangle that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first rectangle to contain.</param>
        /// <param name="value2">The second rectangle to contain.</param>
        /// <returns>The union rectangle.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RectangleF Union(in RectangleF value1, in RectangleF value2)
        {
            Union(in value1, in value2, out RectangleF result);
            return result;
        }

        /// <summary>
        ///     Creates a new rectangle that exactly contains two other rectangles.
        /// </summary>
        /// <param name="value1">The first rectangle to contain.</param>
        /// <param name="value2">The second rectangle to contain.</param>
        /// <param name="result">[OutAttribute] The rectangle that must be the union of the first two rectangles.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Union(in RectangleF value1, in RectangleF value2, out RectangleF result)
        {
            result.Left   = Math.Min(value1.Left, value2.Left);
            result.Right  = Math.Max(value1.Right, value2.Right);
            result.Top    = Math.Min(value1.Top, value2.Top);
            result.Bottom = Math.Max(value1.Bottom, value2.Bottom);
        }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in RectangleF left, in RectangleF right)
        {
            return left.Equals(in right);
        }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in RectangleF left, in RectangleF right)
        {
            return !left.Equals(in right);
        }

        /// <summary>
        ///     Performs an explicit conversion to <see cref="Rectangle" /> structure.
        /// </summary>
        /// <remarks>Performs direct float to int conversion, any fractional data is truncated.</remarks>
        /// <param name="value">The source <see cref="RectangleF" /> value.</param>
        /// <returns>A converted <see cref="Rectangle" /> structure.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator Rectangle(in RectangleF value)
        {
            return new Rectangle((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
        }
    }
}
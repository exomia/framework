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

namespace Exomia.Framework
{
    /// <summary>
    ///     Represents a two dimensional mathematical vector.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
    public struct VectorI2 : IFormattable
    {
        /// <summary>
        ///     A <see cref="VectorI2" /> with all of its components set to zero.
        /// </summary>
        public static readonly VectorI2 Zero = new VectorI2(0, 0);

        /// <summary>
        ///     The X unit <see cref="VectorI2" /> (1, 0).
        /// </summary>
        public static readonly VectorI2 UnitX = new VectorI2(1, 0);

        /// <summary>
        ///     The Y unit <see cref="VectorI2" /> (0, 1).
        /// </summary>
        public static readonly VectorI2 UnitY = new VectorI2(0, 1);

        /// <summary>
        ///     A <see cref="VectorI2" /> with all of its components set to one.
        /// </summary>
        public static readonly VectorI2 One = new VectorI2(1, 1);

        /// <summary>
        ///     The X component of the vector.
        /// </summary>
        public int X;

        /// <summary>
        ///     The Y component of the vector.
        /// </summary>
        public int Y;

        /// <summary>
        ///     Initializes a new instance of the vector2 struct.
        /// </summary>
        /// <param name="value"> The value that will be assigned to all components. </param>
        public VectorI2(int value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        ///     Initializes a new instance of the vector2 struct.
        /// </summary>
        /// <param name="x"> Initial value for the X component of the vector. </param>
        /// <param name="y"> Initial value for the Y component of the vector. </param>
        public VectorI2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="VectorI2" /> is equal to this instance.
        /// </summary>
        /// <param name="other"> The <see cref="VectorI2" /> to compare with this instance. </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="VectorI2" /> is equal to this instance; <c>false</c> otherwise.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in VectorI2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object value)
        {
            if (value is VectorI2 other)
            {
                return Equals(in other);
            }
            return false;
        }

        /// <inheritdoc />
        public override readonly int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (X.GetHashCode() * 307) ^ Y.GetHashCode();

                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        /// <inheritdoc />
        public override readonly string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1}", X, Y);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format"> The format. </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public readonly string ToString(string format)
        {
            if (format == null)
            {
                return ToString();
            }

            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1}",
                X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider"> The format provider. </param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public readonly string ToString(IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                "X:{0} Y:{1}", X, Y);
        }

        /// <inheritdoc />
        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(
                formatProvider,
                "X:{0} Y:{1}",
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider));
        }

        /// <summary>
        ///     Calculates the length of the vector.
        /// </summary>
        /// <returns>
        ///     The length of the vector.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        ///     Calculates the squared length of the vector.
        /// </summary>
        /// <returns>
        ///     The squared length of the vector.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly int LengthSquared()
        {
            return (X * X) + (Y * Y);
        }

        /// <summary>
        ///     Adds two vectors.
        /// </summary>
        /// <param name="left">   [in,out] The first vector to add. </param>
        /// <param name="right">  [in,out] The second vector to add. </param>
        /// <param name="result"> [out] When the method completes, contains the sum of the two vectors. </param>
        public static void Add(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
        }

        /// <summary>
        ///     Adds two vectors.
        /// </summary>
        /// <param name="left">  The first vector to add. </param>
        /// <param name="right"> The second vector to add. </param>
        /// <returns>
        ///     The sum of the two vectors.
        /// </returns>
        public static VectorI2 Add(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="left">   [in,out] The input vector. </param>
        /// <param name="right">  [in,out] The scalar value to be added to elements. </param>
        /// <param name="result"> [out] The vector with added scalar for each element. </param>
        public static void Add(ref VectorI2 left, ref int right, out VectorI2 result)
        {
            result.X = left.X + right;
            result.Y = left.Y + right;
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="left">  The input vector. </param>
        /// <param name="right"> The scalar value to be added to elements. </param>
        /// <returns>
        ///     The vector with added scalar for each element.
        /// </returns>
        public static VectorI2 Add(in VectorI2 left, int right)
        {
            return new VectorI2(left.X + right, left.Y + right);
        }

        /// <summary>
        ///     Subtracts two vectors.
        /// </summary>
        /// <param name="left">   [in,out] The first vector to subtract. </param>
        /// <param name="right">  [in,out] The second vector to subtract. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the difference of the two
        ///     vectors.
        /// </param>
        public static void Subtract(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
        }

        /// <summary>
        ///     Subtracts two vectors.
        /// </summary>
        /// <param name="left">  The first vector to subtract. </param>
        /// <param name="right"> The second vector to subtract. </param>
        /// <returns>
        ///     The difference of the two vectors.
        /// </returns>
        public static VectorI2 Subtract(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">   [in,out] The input vector. </param>
        /// <param name="right">  [in,out] The scalar value to be subtracted from elements. </param>
        /// <param name="result"> [out] The vector with subtracted scalar for each element. </param>
        public static void Subtract(ref VectorI2 left, ref int right, out VectorI2 result)
        {
            result.X = left.X - right;
            result.Y = left.Y - right;
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">  The input vector. </param>
        /// <param name="right"> The scalar value to be subtracted from elements. </param>
        /// <returns>
        ///     The vector with subtracted scalar for each element.
        /// </returns>
        public static VectorI2 Subtract(in VectorI2 left, int right)
        {
            return new VectorI2(left.X - right, left.Y - right);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">   [in,out] The scalar value to be subtracted from elements. </param>
        /// <param name="right">  [in,out] The input vector. </param>
        /// <param name="result"> [out] The vector with subtracted scalar for each element. </param>
        public static void Subtract(ref int left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left - right.X;
            result.Y = left - right.Y;
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">  The scalar value to be subtracted from elements. </param>
        /// <param name="right"> The input vector. </param>
        /// <returns>
        ///     The vector with subtracted scalar for each element.
        /// </returns>
        public static VectorI2 Subtract(int left, in VectorI2 right)
        {
            return new VectorI2(left - right.X, left - right.Y);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value">  [in,out] The vector to scale. </param>
        /// <param name="scale">  The amount by which to vector the vector. </param>
        /// <param name="result"> [out] When the method completes, contains the scaled vector. </param>
        public static void Multiply(ref VectorI2 value, int scale, out VectorI2 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value"> The vector to scale. </param>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 Multiply(in VectorI2 value, int scale)
        {
            return new VectorI2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Multiplies a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">   [in,out] The first vector to multiply. </param>
        /// <param name="right">  [in,out] The second vector to multiply. </param>
        /// <param name="result"> [out] When the method completes, contains the multiplied vector. </param>
        public static void Multiply(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X * right.X;
            result.Y = left.Y * right.Y;
        }

        /// <summary>
        ///     Multiplies a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">  The first vector to multiply. </param>
        /// <param name="right"> The vector vector to multiply. </param>
        /// <returns>
        ///     The multiplied vector.
        /// </returns>
        public static VectorI2 Multiply(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value">  [in,out] The vector to scale. </param>
        /// <param name="scale">  The amount by which to vector the vector. </param>
        /// <param name="result"> [out] When the method completes, contains the scaled vector. </param>
        public static void Divide(ref VectorI2 value, int scale, out VectorI2 result)
        {
            result.X = value.X / scale;
            result.Y = value.Y / scale;
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value"> The vector to scale. </param>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 Divide(in VectorI2 value, int scale)
        {
            return new VectorI2(value.X / scale, value.Y / scale);
        }

        /// <summary>
        ///     Scales a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">   [in,out] The first vector to multiply. </param>
        /// <param name="right">  [in,out] The second vector to multiply. </param>
        /// <param name="result"> [out] When the method completes, contains the multiplied vector. </param>
        public static void Divide(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X / right.X;
            result.Y = left.Y / right.Y;
        }

        /// <summary>
        ///     Scales a vector with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">  The first vector to multiply. </param>
        /// <param name="right"> The vector vector to multiply. </param>
        /// <returns>
        ///     The multiplied vector.
        /// </returns>
        public static VectorI2 Divide(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        ///     Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value">  [in,out] The vector to negate. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains a vector facing in the opposite
        ///     direction.
        /// </param>
        public static void Negate(ref VectorI2 value, out VectorI2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        /// <summary>
        ///     Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value"> The vector to negate. </param>
        /// <returns>
        ///     A vector facing in the opposite direction.
        /// </returns>
        public static VectorI2 Negate(in VectorI2 value)
        {
            return new VectorI2(-value.X, -value.Y);
        }

        /// <summary>
        ///     Returns per component absolute value of a vector.
        /// </summary>
        /// <param name="value">  [in,out] Input vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains a vector with each component
        ///     being the absolute value of the input component.
        /// </param>
        public static void Abs(ref VectorI2 value, out VectorI2 result)
        {
            result.X = (value.X + (value.X >> 31)) ^ (value.X >> 31);
            result.Y = (value.Y + (value.Y >> 31)) ^ (value.Y >> 31);
        }

        /// <summary>
        ///     Returns per component absolute value of a vector.
        /// </summary>
        /// <param name="value"> Input vector. </param>
        /// <returns>
        ///     A vector with each component being the absolute value of the input component.
        /// </returns>
        public static VectorI2 Abs(in VectorI2 value)
        {
            return new VectorI2(
                (value.X + (value.X >> 31)) ^ (value.X >> 31),
                (value.Y + (value.Y >> 31)) ^ (value.Y >> 31));
        }

        /// <summary>
        ///     Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1"> [in,out] The first vector. </param>
        /// <param name="value2"> [in,out] The second vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the distance between the two
        ///     vectors.
        /// </param>
        public static void Distance(ref VectorI2 value1, ref VectorI2 value2, out double result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            result = Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        ///     Calculates the distance between two vectors.
        /// </summary>
        /// <param name="value1"> The first vector. </param>
        /// <param name="value2"> The second vector. </param>
        /// <returns>
        ///     The distance between the two vectors.
        /// </returns>
        public static double Distance(in VectorI2 value1, in VectorI2 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1"> [in,out] The first vector. </param>
        /// <param name="value2"> [in,out] The second vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the squared distance between
        ///     the two vectors.
        /// </param>
        public static void DistanceSquared(ref VectorI2 value1, ref VectorI2 value2, out int result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            result = (x * x) + (y * y);
        }

        /// <summary>
        ///     Calculates the squared distance between two vectors.
        /// </summary>
        /// <param name="value1"> The first vector. </param>
        /// <param name="value2"> The second vector. </param>
        /// <returns>
        ///     The squared distance between the two vectors.
        /// </returns>
        public static int DistanceSquared(in VectorI2 value1, in VectorI2 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return (x * x) + (y * y);
        }

        /// <summary>
        ///     Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">   [in,out] First source vector. </param>
        /// <param name="right">  [in,out] Second source vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the dot product of the two
        ///     vectors.
        /// </param>
        public static void Dot(ref VectorI2 left, ref VectorI2 right, out int result)
        {
            result = (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        ///     Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="left">  First source vector. </param>
        /// <param name="right"> Second source vector. </param>
        /// <returns>
        ///     The dot product of the two vectors.
        /// </returns>
        public static int Dot(in VectorI2 left, in VectorI2 right)
        {
            return (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        ///     Returns a vector containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">   [in,out] The first source vector. </param>
        /// <param name="right">  [in,out] The second source vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains an new vector composed of the
        ///     largest components of the source vectors.
        /// </param>
        public static void Max(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X > right.X ? left.X : right.X;
            result.Y = left.Y > right.Y ? left.Y : right.Y;
        }

        /// <summary>
        ///     Returns a vector containing the largest components of the specified vectors.
        /// </summary>
        /// <param name="left">  The first source vector. </param>
        /// <param name="right"> The second source vector. </param>
        /// <returns>
        ///     A vector containing the largest components of the source vectors.
        /// </returns>
        public static VectorI2 Max(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(
                left.X > right.X ? left.X : right.X,
                left.Y > right.Y ? left.Y : right.Y);
        }

        /// <summary>
        ///     Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">   [in,out] The first source vector. </param>
        /// <param name="right">  [in,out] The second source vector. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains an new vector composed of the
        ///     smallest components of the source vectors.
        /// </param>
        public static void Min(ref VectorI2 left, ref VectorI2 right, out VectorI2 result)
        {
            result.X = left.X < right.X ? left.X : right.X;
            result.Y = left.Y < right.Y ? left.Y : right.Y;
        }

        /// <summary>
        ///     Returns a vector containing the smallest components of the specified vectors.
        /// </summary>
        /// <param name="left">  The first source vector. </param>
        /// <param name="right"> The second source vector. </param>
        /// <returns>
        ///     A vector containing the smallest components of the source vectors.
        /// </returns>
        public static VectorI2 Min(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(
                left.X < right.X ? left.X : right.X,
                left.Y < right.Y ? left.Y : right.Y);
        }

        /// <summary>
        ///     Adds two vectors.
        /// </summary>
        /// <param name="left">  The first vector to add. </param>
        /// <param name="right"> The second vector to add. </param>
        /// <returns>
        ///     The sum of the two vectors.
        /// </returns>
        public static VectorI2 operator +(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="value">  The input vector. </param>
        /// <param name="scalar"> The scalar value to be added on elements. </param>
        /// <returns>
        ///     The vector with added scalar for each element.
        /// </returns>
        public static VectorI2 operator +(in VectorI2 value, int scalar)
        {
            return new VectorI2(value.X + scalar, value.Y + scalar);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="scalar"> The scalar value to be added on elements. </param>
        /// <param name="value">  The input vector. </param>
        /// <returns>
        ///     The vector with added scalar for each element.
        /// </returns>
        public static VectorI2 operator +(int scalar, in VectorI2 value)
        {
            return new VectorI2(scalar + value.X, scalar + value.Y);
        }

        /// <summary>
        ///     Assert a vector (return it unchanged).
        /// </summary>
        /// <param name="value"> The vector to assert (unchanged). </param>
        /// <returns>
        ///     The asserted (unchanged) vector.
        /// </returns>
        public static VectorI2 operator +(in VectorI2 value)
        {
            return value;
        }

        /// <summary>
        ///     Subtract two vectors.
        /// </summary>
        /// <param name="left">  The first vector be subtracted from. </param>
        /// <param name="right"> The second vector to subtract. </param>
        /// <returns>
        ///     The sum of the two vectors.
        /// </returns>
        public static VectorI2 operator -(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="value">  The input vector. </param>
        /// <param name="scalar"> The scalar value to be subtracted from elements. </param>
        /// <returns>
        ///     The vector with subtracted scalar from each element.
        /// </returns>
        public static VectorI2 operator -(in VectorI2 value, int scalar)
        {
            return new VectorI2(value.X - scalar, value.Y - scalar);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="scalar"> The scalar value to be subtracted from elements. </param>
        /// <param name="value">  The input vector. </param>
        /// <returns>
        ///     The vector with subtracted scalar from each element.
        /// </returns>
        public static VectorI2 operator -(int scalar, in VectorI2 value)
        {
            return new VectorI2(scalar - value.X, scalar - value.Y);
        }

        /// <summary>
        ///     Reverses the direction of a given vector.
        /// </summary>
        /// <param name="value"> The vector to negate. </param>
        /// <returns>
        ///     A vector facing in the opposite direction.
        /// </returns>
        public static VectorI2 operator -(in VectorI2 value)
        {
            return new VectorI2(-value.X, -value.Y);
        }

        /// <summary>
        ///     Multiplies a vector with another by performing component-wise multiplication equivalent to
        ///     <see cref="Multiply(ref VectorI2, ref VectorI2, out VectorI2)" />.
        /// </summary>
        /// <param name="left">  The first vector to multiply. </param>
        /// <param name="right"> The second vector to multiply. </param>
        /// <returns>
        ///     The multiplication of the two vectors.
        /// </returns>
        public static VectorI2 operator *(in VectorI2 left, in VectorI2 right)
        {
            return new VectorI2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <param name="value"> The vector to scale. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 operator *(int scale, in VectorI2 value)
        {
            return new VectorI2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value"> The vector to scale. </param>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 operator *(in VectorI2 value, int scale)
        {
            return new VectorI2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value"> The vector to scale. </param>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 operator /(in VectorI2 value, int scale)
        {
            return new VectorI2(value.X / scale, value.Y / scale);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <param name="value"> The vector to scale. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 operator /(int scale, in VectorI2 value)
        {
            return new VectorI2(scale / value.X, scale / value.Y);
        }

        /// <summary>
        ///     Scales a vector by the given value.
        /// </summary>
        /// <param name="value"> The vector to scale. </param>
        /// <param name="scale"> The amount by which to scale the vector. </param>
        /// <returns>
        ///     The scaled vector.
        /// </returns>
        public static VectorI2 operator /(in VectorI2 value, in VectorI2 scale)
        {
            return new VectorI2(value.X / scale.X, value.Y / scale.Y);
        }

        /// <summary>
        ///     Tests for equality between two objects.
        /// </summary>
        /// <param name="left">  The first value to compare. </param>
        /// <param name="right"> The second value to compare. </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> has the same value as <paramref name="right" />;
        ///     otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in VectorI2 left, in VectorI2 right)
        {
            return left.Equals(in right);
        }

        /// <summary>
        ///     Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">  The first value to compare. </param>
        /// <param name="right"> The second value to compare. </param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> has a different value than
        ///     <paramref name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in VectorI2 left, in VectorI2 right)
        {
            return !left.Equals(in right);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="int" /> to <see cref="VectorI2" />. equal
        ///     to <see cref="VectorI2" /> (value)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator VectorI2(int value)
        {
            return new VectorI2(value, value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="VectorI2" /> to <see cref="Vector2" />.
        ///     equal to <see cref="Vector2" /> (value.x, value.y)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Vector2(in VectorI2 value)
        {
            return new Vector2(value.X, value.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Vector2" /> to <see cref="VectorI2" />.
        ///     equal to <see cref="VectorI2" /> ((int)value.x, (int)value.y)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator VectorI2(in Vector2 value)
        {
            return new VectorI2((int)value.X, (int)value.Y);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="VectorI2" /> to <see cref="Vector3" />.
        ///     equal to <see cref="Vector3" /> (value.x, value.y, 0)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Vector3(in VectorI2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Vector3" /> to <see cref="VectorI2" />.
        ///     equal to <see cref="VectorI2" /> ((int)value.x, (int)value.y)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator VectorI2(in Vector3 value)
        {
            return new VectorI2((int)value.X, (int)value.Y);
        }
    }
}
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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SharpDX;

namespace Exomia.Framework
{
    /// <summary>
    ///     Represents a two dimensional mathematical index.
    /// </summary>
    /// <inheritdoc cref="IFormattable" />
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 8)]
    public struct Index2 : IFormattable
    {
        /// <summary>
        ///     A <see cref="Index2" /> with all of its components set to zero.
        /// </summary>
        public static readonly Index2 Zero = new Index2(0, 0);

        /// <summary>
        ///     The X unit <see cref="Index2" /> (1, 0).
        /// </summary>
        public static readonly Index2 UnitX = new Index2(1, 0);

        /// <summary>
        ///     The Y unit <see cref="Index2" /> (0, 1).
        /// </summary>
        public static readonly Index2 UnitY = new Index2(0, 1);

        /// <summary>
        ///     A <see cref="Index2" /> with all of its components set to one.
        /// </summary>
        public static readonly Index2 One = new Index2(1, 1);

        /// <summary>
        ///     The X component of the index.
        /// </summary>
        public int X;

        /// <summary>
        ///     The Y component of the index.
        /// </summary>
        public int Y;

        /// <summary>
        ///     Initializes a new instance of the Index2 struct.
        /// </summary>
        /// <param name="value"> The value that will be assigned to all components. </param>
        public Index2(int value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        ///     Initializes a new instance of the Index2 struct.
        /// </summary>
        /// <param name="x"> Initial value for the X component of the index. </param>
        /// <param name="y"> Initial value for the Y component of the index. </param>
        public Index2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Index2" /> is equal to this instance.
        /// </summary>
        /// <param name="other"> The <see cref="Index2" /> to compare with this instance. </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="Index2" /> is equal to this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Index2 other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object value)
        {
            if (value is Index2 other)
            {
                return Equals(in other);
            }
            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (X.GetHashCode() * 307) ^ Y.GetHashCode();

                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        /// <inheritdoc />
        public override string ToString()
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
        public string ToString(string format)
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
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                "X:{0} Y:{1}", X, Y);
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
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
        ///     Calculates the length of the index.
        /// </summary>
        /// <returns>
        ///     The length of the index.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Length()
        {
            return (float)Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        ///     Calculates the squared length of the index.
        /// </summary>
        /// <returns>
        ///     The squared length of the index.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LengthSquared()
        {
            return (X * X) + (Y * Y);
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">   [in,out] The first index to add. </param>
        /// <param name="right">  [in,out] The second index to add. </param>
        /// <param name="result"> [out] When the method completes, contains the sum of the two indices. </param>
        public static void Add(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">  The first index to add. </param>
        /// <param name="right"> The second index to add. </param>
        /// <returns>
        ///     The sum of the two indexs.
        /// </returns>
        public static Index2 Add(in Index2 left, in Index2 right)
        {
            return new Index2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="left">   [in,out] The input index. </param>
        /// <param name="right">  [in,out] The scalar value to be added to elements. </param>
        /// <param name="result"> [out] The index with added scalar for each element. </param>
        public static void Add(ref Index2 left, ref int right, out Index2 result)
        {
            result.X = left.X + right;
            result.Y = left.Y + right;
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="left">  The input index. </param>
        /// <param name="right"> The scalar value to be added to elements. </param>
        /// <returns>
        ///     The index with added scalar for each element.
        /// </returns>
        public static Index2 Add(in Index2 left, int right)
        {
            return new Index2(left.X + right, left.Y + right);
        }

        /// <summary>
        ///     Subtracts two indices.
        /// </summary>
        /// <param name="left">   [in,out] The first index to subtract. </param>
        /// <param name="right">  [in,out] The second index to subtract. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the difference of the two
        ///     indices.
        /// </param>
        public static void Subtract(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
        }

        /// <summary>
        ///     Subtracts two indices.
        /// </summary>
        /// <param name="left">  The first index to subtract. </param>
        /// <param name="right"> The second index to subtract. </param>
        /// <returns>
        ///     The difference of the two indices.
        /// </returns>
        public static Index2 Subtract(in Index2 left, in Index2 right)
        {
            return new Index2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">   [in,out] The input index. </param>
        /// <param name="right">  [in,out] The scalar value to be subtracted from elements. </param>
        /// <param name="result"> [out] The index with subtracted scalar for each element. </param>
        public static void Subtract(ref Index2 left, ref int right, out Index2 result)
        {
            result.X = left.X - right;
            result.Y = left.Y - right;
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">  The input index. </param>
        /// <param name="right"> The scalar value to be subtracted from elements. </param>
        /// <returns>
        ///     The index with subtracted scalar for each element.
        /// </returns>
        public static Index2 Subtract(in Index2 left, int right)
        {
            return new Index2(left.X - right, left.Y - right);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">   [in,out] The scalar value to be subtracted from elements. </param>
        /// <param name="right">  [in,out] The input index. </param>
        /// <param name="result"> [out] The index with subtracted scalar for each element. </param>
        public static void Subtract(ref int left, ref Index2 right, out Index2 result)
        {
            result.X = left - right.X;
            result.Y = left - right.Y;
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="left">  The scalar value to be subtracted from elements. </param>
        /// <param name="right"> The input index. </param>
        /// <returns>
        ///     The index with subtracted scalar for each element.
        /// </returns>
        public static Index2 Subtract(int left, in Index2 right)
        {
            return new Index2(left - right.X, left - right.Y);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">  [in,out] The index to scale. </param>
        /// <param name="scale">  The amount by which to index the index. </param>
        /// <param name="result"> [out] When the method completes, contains the scaled index. </param>
        public static void Multiply(ref Index2 value, int scale, out Index2 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value"> The index to scale. </param>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 Multiply(in Index2 value, int scale)
        {
            return new Index2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">   [in,out] The first index to multiply. </param>
        /// <param name="right">  [in,out] The second index to multiply. </param>
        /// <param name="result"> [out] When the method completes, contains the multiplied index. </param>
        public static void Multiply(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X * right.X;
            result.Y = left.Y * right.Y;
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">  The first index to multiply. </param>
        /// <param name="right"> The index index to multiply. </param>
        /// <returns>
        ///     The multiplied index.
        /// </returns>
        public static Index2 Multiply(in Index2 left, in Index2 right)
        {
            return new Index2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">  [in,out] The index to scale. </param>
        /// <param name="scale">  The amount by which to index the index. </param>
        /// <param name="result"> [out] When the method completes, contains the scaled index. </param>
        public static void Divide(ref Index2 value, int scale, out Index2 result)
        {
            result.X = value.X / scale;
            result.Y = value.Y / scale;
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value"> The index to scale. </param>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 Divide(in Index2 value, int scale)
        {
            return new Index2(value.X / scale, value.Y / scale);
        }

        /// <summary>
        ///     Scales a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">   [in,out] The first index to multiply. </param>
        /// <param name="right">  [in,out] The second index to multiply. </param>
        /// <param name="result"> [out] When the method completes, contains the multiplied index. </param>
        public static void Divide(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X / right.X;
            result.Y = left.Y / right.Y;
        }

        /// <summary>
        ///     Scales a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">  The first index to multiply. </param>
        /// <param name="right"> The index index to multiply. </param>
        /// <returns>
        ///     The multiplied index.
        /// </returns>
        public static Index2 Divide(in Index2 left, in Index2 right)
        {
            return new Index2(left.X / right.X, left.Y / right.Y);
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value">  [in,out] The index to negate. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains a index facing in the opposite
        ///     direction.
        /// </param>
        public static void Negate(ref Index2 value, out Index2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value"> The index to negate. </param>
        /// <returns>
        ///     A index facing in the opposite direction.
        /// </returns>
        public static Index2 Negate(in Index2 value)
        {
            return new Index2(-value.X, -value.Y);
        }

        /// <summary>
        ///     Returns per component absolute value of a index.
        /// </summary>
        /// <param name="value">  [in,out] Input index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains a index with each component
        ///     being the absolute value of the input component.
        /// </param>
        public static void Abs(ref Index2 value, out Index2 result)
        {
            result.X = (value.X + (value.X >> 31)) ^ (value.X >> 31);
            result.Y = (value.Y + (value.Y >> 31)) ^ (value.Y >> 31);
        }

        /// <summary>
        ///     Returns per component absolute value of a index.
        /// </summary>
        /// <param name="value"> Input index. </param>
        /// <returns>
        ///     A index with each component being the absolute value of the input component.
        /// </returns>
        public static Index2 Abs(in Index2 value)
        {
            return new Index2(
                (value.X + (value.X >> 31)) ^ (value.X >> 31),
                (value.Y + (value.Y >> 31)) ^ (value.Y >> 31));
        }

        /// <summary>
        ///     Calculates the distance between two indices.
        /// </summary>
        /// <param name="value1"> [in,out] The first index. </param>
        /// <param name="value2"> [in,out] The second index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the distance between the two
        ///     indices.
        /// </param>
        public static void Distance(ref Index2 value1, ref Index2 value2, out double result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            result = Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        ///     Calculates the distance between two indices.
        /// </summary>
        /// <param name="value1"> The first index. </param>
        /// <param name="value2"> The second index. </param>
        /// <returns>
        ///     The distance between the two indices.
        /// </returns>
        public static double Distance(in Index2 value1, in Index2 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return Math.Sqrt((x * x) + (y * y));
        }

        /// <summary>
        ///     Calculates the squared distance between two indices.
        /// </summary>
        /// <param name="value1"> [in,out] The first index. </param>
        /// <param name="value2"> [in,out] The second index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the squared distance between
        ///     the two indices.
        /// </param>
        public static void DistanceSquared(ref Index2 value1, ref Index2 value2, out int result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            result = (x * x) + (y * y);
        }

        /// <summary>
        ///     Calculates the squared distance between two indices.
        /// </summary>
        /// <param name="value1"> The first index. </param>
        /// <param name="value2"> The second index. </param>
        /// <returns>
        ///     The squared distance between the two indices.
        /// </returns>
        public static int DistanceSquared(in Index2 value1, in Index2 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;

            return (x * x) + (y * y);
        }

        /// <summary>
        ///     Calculates the dot product of two indices.
        /// </summary>
        /// <param name="left">   [in,out] First source index. </param>
        /// <param name="right">  [in,out] Second source index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains the dot product of the two
        ///     indices.
        /// </param>
        public static void Dot(ref Index2 left, ref Index2 right, out int result)
        {
            result = (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        ///     Calculates the dot product of two indices.
        /// </summary>
        /// <param name="left">  First source index. </param>
        /// <param name="right"> Second source index. </param>
        /// <returns>
        ///     The dot product of the two indices.
        /// </returns>
        public static int Dot(in Index2 left, in Index2 right)
        {
            return (left.X * right.X) + (left.Y * right.Y);
        }

        /// <summary>
        ///     Returns a index containing the largest components of the specified indices.
        /// </summary>
        /// <param name="left">   [in,out] The first source index. </param>
        /// <param name="right">  [in,out] The second source index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains an new index composed of the
        ///     largest components of the source indices.
        /// </param>
        public static void Max(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X > right.X ? left.X : right.X;
            result.Y = left.Y > right.Y ? left.Y : right.Y;
        }

        /// <summary>
        ///     Returns a index containing the largest components of the specified indices.
        /// </summary>
        /// <param name="left">  The first source index. </param>
        /// <param name="right"> The second source index. </param>
        /// <returns>
        ///     A index containing the largest components of the source indices.
        /// </returns>
        public static Index2 Max(in Index2 left, in Index2 right)
        {
            return new Index2(
                left.X > right.X ? left.X : right.X,
                left.Y > right.Y ? left.Y : right.Y);
        }

        /// <summary>
        ///     Returns a index containing the smallest components of the specified indices.
        /// </summary>
        /// <param name="left">   [in,out] The first source index. </param>
        /// <param name="right">  [in,out] The second source index. </param>
        /// <param name="result">
        ///     [out] When the method completes, contains an new index composed of the
        ///     smallest components of the source indices.
        /// </param>
        public static void Min(ref Index2 left, ref Index2 right, out Index2 result)
        {
            result.X = left.X < right.X ? left.X : right.X;
            result.Y = left.Y < right.Y ? left.Y : right.Y;
        }

        /// <summary>
        ///     Returns a index containing the smallest components of the specified indices.
        /// </summary>
        /// <param name="left">  The first source index. </param>
        /// <param name="right"> The second source index. </param>
        /// <returns>
        ///     A index containing the smallest components of the source indices.
        /// </returns>
        public static Index2 Min(in Index2 left, in Index2 right)
        {
            return new Index2(
                left.X < right.X ? left.X : right.X,
                left.Y < right.Y ? left.Y : right.Y);
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">  The first index to add. </param>
        /// <param name="right"> The second index to add. </param>
        /// <returns>
        ///     The sum of the two indices.
        /// </returns>
        public static Index2 operator +(in Index2 left, in Index2 right)
        {
            return new Index2(left.X + right.X, left.Y + right.Y);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="value">  The input index. </param>
        /// <param name="scalar"> The scalar value to be added on elements. </param>
        /// <returns>
        ///     The index with added scalar for each element.
        /// </returns>
        public static Index2 operator +(in Index2 value, int scalar)
        {
            return new Index2(value.X + scalar, value.Y + scalar);
        }

        /// <summary>
        ///     Perform a component-wise addition.
        /// </summary>
        /// <param name="scalar"> The scalar value to be added on elements. </param>
        /// <param name="value">  The input index. </param>
        /// <returns>
        ///     The index with added scalar for each element.
        /// </returns>
        public static Index2 operator +(int scalar, in Index2 value)
        {
            return new Index2(scalar + value.X, scalar + value.Y);
        }

        /// <summary>
        ///     Assert a index (return it unchanged).
        /// </summary>
        /// <param name="value"> The index to assert (unchanged). </param>
        /// <returns>
        ///     The asserted (unchanged) index.
        /// </returns>
        public static Index2 operator +(in Index2 value)
        {
            return value;
        }

        /// <summary>
        ///     Subtract two indices.
        /// </summary>
        /// <param name="left">  The first index be subtracted from. </param>
        /// <param name="right"> The second index to subtract. </param>
        /// <returns>
        ///     The sum of the two indices.
        /// </returns>
        public static Index2 operator -(in Index2 left, in Index2 right)
        {
            return new Index2(left.X - right.X, left.Y - right.Y);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="value">  The input index. </param>
        /// <param name="scalar"> The scalar value to be subtracted from elements. </param>
        /// <returns>
        ///     The index with subtracted scalar from each element.
        /// </returns>
        public static Index2 operator -(in Index2 value, int scalar)
        {
            return new Index2(value.X - scalar, value.Y - scalar);
        }

        /// <summary>
        ///     Perform a component-wise subtraction.
        /// </summary>
        /// <param name="scalar"> The scalar value to be subtracted from elements. </param>
        /// <param name="value">  The input index. </param>
        /// <returns>
        ///     The index with subtracted scalar from each element.
        /// </returns>
        public static Index2 operator -(int scalar, in Index2 value)
        {
            return new Index2(scalar - value.X, scalar - value.Y);
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value"> The index to negate. </param>
        /// <returns>
        ///     A index facing in the opposite direction.
        /// </returns>
        public static Index2 operator -(in Index2 value)
        {
            return new Index2(-value.X, -value.Y);
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication equivalent to
        ///     <see cref="Multiply(ref Index2, ref Index2, out Index2)" />.
        /// </summary>
        /// <param name="left">  The first index to multiply. </param>
        /// <param name="right"> The second index to multiply. </param>
        /// <returns>
        ///     The multiplication of the two indices.
        /// </returns>
        public static Index2 operator *(in Index2 left, in Index2 right)
        {
            return new Index2(left.X * right.X, left.Y * right.Y);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <param name="value"> The index to scale. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 operator *(int scale, in Index2 value)
        {
            return new Index2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value"> The index to scale. </param>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 operator *(in Index2 value, int scale)
        {
            return new Index2(value.X * scale, value.Y * scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value"> The index to scale. </param>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 operator /(in Index2 value, int scale)
        {
            return new Index2(value.X / scale, value.Y / scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <param name="value"> The index to scale. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 operator /(int scale, in Index2 value)
        {
            return new Index2(scale / value.X, scale / value.Y);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value"> The index to scale. </param>
        /// <param name="scale"> The amount by which to scale the index. </param>
        /// <returns>
        ///     The scaled index.
        /// </returns>
        public static Index2 operator /(in Index2 value, in Index2 scale)
        {
            return new Index2(value.X / scale.X, value.Y / scale.Y);
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
        public static bool operator ==(in Index2 left, in Index2 right)
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
        public static bool operator !=(in Index2 left, in Index2 right)
        {
            return !left.Equals(in right);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="int" /> to <see cref="Index2" />. equal
        ///     to <see cref="Index2" /> (value)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Index2(int value)
        {
            return new Index2(value, value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Index2" /> to <see cref="Vector3" />.
        ///     equal to <see cref="Vector3" /> (value.x, value.y, 0)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Vector3(in Index2 value)
        {
            return new Vector3(value.X, value.Y, 0);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Vector3" /> to <see cref="Index2" />.
        ///     equal to <see cref="Index2" /> ((int)value.x, (int)value.y)
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator Index2(in Vector3 value)
        {
            return new Index2((int)value.X, (int)value.Y);
        }
    }
}
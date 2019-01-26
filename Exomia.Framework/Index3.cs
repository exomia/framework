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

namespace Exomia.Framework
{
    /// <inheritdoc cref="IFormattable" />
    /// <summary>
    ///     Represents a three dimensional mathematical index.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    public struct Index3 : IFormattable
    {
        /// <summary>
        ///     A <see cref="Index3" /> with all of its components set to zero.
        /// </summary>
        public static readonly Index3 Zero = new Index3(0, 0, 0);

        /// <summary>
        ///     The X unit <see cref="Index3" /> (1, 0, 0).
        /// </summary>
        public static readonly Index3 UnitX = new Index3(1, 0, 0);

        /// <summary>
        ///     The Y unit <see cref="Index3" /> (0, 1, 0).
        /// </summary>
        public static readonly Index3 UnitY = new Index3(0, 1, 0);

        /// <summary>
        ///     The Z unit <see cref="Index3" /> (0, 0, 1).
        /// </summary>
        public static readonly Index3 UnitZ = new Index3(0, 0, 1);

        /// <summary>
        ///     A <see cref="Index3" /> with all of its components set to one.
        /// </summary>
        public static readonly Index3 One = new Index3(1, 1, 1);

        /// <summary>
        ///     The X component of the index
        /// </summary>
        public int X;

        /// <summary>
        ///     The Y component of the index
        /// </summary>
        public int Y;

        /// <summary>
        ///     The Z component of the index
        /// </summary>
        public int Z;

        /// <summary>
        ///     Initializes a new instance of the Index3 struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Index3(int value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        ///     Initializes a new instance of the Index3 struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the index.</param>
        /// <param name="y">Initial value for the Y component of the index.</param>
        /// <param name="z">Initial value for the Z component of the index.</param>
        public Index3(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        ///     Initializes a new instance of the Index3 struct.
        ///     equal to <see cref="Index3" /> (value.x, value.y, 0)
        /// </summary>
        /// <param name="value">Initial value for the X and Y component of the index.</param>
        /// <param name="z">Initial value for the Z component of the index.</param>
        public Index3(in Index2 value, int z)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="Index3" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Index3" /> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="Index3" /> is equal to this instance; <c>false</c> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in Index3 other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Z == other.Z;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object value)
        {
            if (value is Index3 other)
            {
                return Equals(in other);
            }
            return false;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                return (((X.GetHashCode() * 307) ^
                         Y.GetHashCode()) * 521) ^ Z.GetHashCode();

                // ReSharper restore NonReadonlyMemberInGetHashCode
            }
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
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
                "X:{0} Y:{1} Z:{2}",
                X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture),
                Z.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(
                formatProvider,
                "X:{0} Y:{1} Z:{2}", X, Y, Z);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString(formatProvider);
            }

            return string.Format(
                formatProvider,
                "X:{0} Y:{1} Z:{2}",
                X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider),
                Z.ToString(format, formatProvider));
        }

        /// <summary>
        ///     Calculates the length of the index.
        /// </summary>
        /// <returns>The length of the index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length()
        {
            return Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        /// <summary>
        ///     Calculates the squared length of the index.
        /// </summary>
        /// <returns>The squared length of the index.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">The first index to add.</param>
        /// <param name="right">The second index to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two indices.</param>
        public static void Add(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X + right.Y;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">The first index to add.</param>
        /// <param name="right">The second index to add.</param>
        /// <returns>The sum of the two indexs.</returns>
        public static Index3 Add(in Index3 left, in Index3 right)
        {
            return new Index3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        ///     Perform a component-wise addition
        /// </summary>
        /// <param name="left">The input index</param>
        /// <param name="right">The scalar value to be added to elements</param>
        /// <param name="result">The index with added scalar for each element.</param>
        public static void Add(ref Index3 left, ref int right, out Index3 result)
        {
            result.X = left.X + right;
            result.Y = left.Y + right;
            result.Z = left.Z + right;
        }

        /// <summary>
        ///     Perform a component-wise addition
        /// </summary>
        /// <param name="left">The input index</param>
        /// <param name="right">The scalar value to be added to elements</param>
        /// <returns>The index with added scalar for each element.</returns>
        public static Index3 Add(in Index3 left, int right)
        {
            return new Index3(left.X + right, left.Y + right, left.Z + right);
        }

        /// <summary>
        ///     Subtracts two indices.
        /// </summary>
        /// <param name="left">The first index to subtract.</param>
        /// <param name="right">The second index to subtract.</param>
        /// <param name="result">When the method completes, contains the difference of the two indices.</param>
        public static void Subtract(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
        }

        /// <summary>
        ///     Subtracts two indices.
        /// </summary>
        /// <param name="left">The first index to subtract.</param>
        /// <param name="right">The second index to subtract.</param>
        /// <returns>The difference of the two indices.</returns>
        public static Index3 Subtract(in Index3 left, in Index3 right)
        {
            return new Index3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="left">The input index</param>
        /// <param name="right">The scalar value to be subtracted from elements</param>
        /// <param name="result">The index with subtracted scalar for each element.</param>
        public static void Subtract(ref Index3 left, ref int right, out Index3 result)
        {
            result.X = left.X - right;
            result.Y = left.Y - right;
            result.Z = left.Z - right;
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="left">The input index</param>
        /// <param name="right">The scalar value to be subtracted from elements</param>
        /// <returns>The index with subtracted scalar for each element.</returns>
        public static Index3 Subtract(in Index3 left, int right)
        {
            return new Index3(left.X - right, left.Y - right, left.Z - right);
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="left">The scalar value to be subtracted from elements</param>
        /// <param name="right">The input index</param>
        /// <param name="result">The index with subtracted scalar for each element.</param>
        public static void Subtract(ref int left, ref Index3 right, out Index3 result)
        {
            result.X = left - right.X;
            result.Y = left - right.Y;
            result.Z = left - right.Z;
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="left">The scalar value to be subtracted from elements</param>
        /// <param name="right">The input index</param>
        /// <returns>The index with subtracted scalar for each element.</returns>
        public static Index3 Subtract(int left, in Index3 right)
        {
            return new Index3(left - right.X, left - right.Y, left - right.Z);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to index the index.</param>
        /// <param name="result">When the method completes, contains the scaled index.</param>
        public static void Multiply(ref Index3 value, int scale, out Index3 result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 Multiply(in Index3 value, int scale)
        {
            return new Index3(value.X * scale, value.Y * scale, value.Z * scale);
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first index to multiply.</param>
        /// <param name="right">The second index to multiply.</param>
        /// <param name="result">When the method completes, contains the multiplied index.</param>
        public static void Multiply(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X * right.X;
            result.Y = left.Y * right.Y;
            result.Z = left.Z * right.Z;
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first index to multiply.</param>
        /// <param name="right">The index index to multiply.</param>
        /// <returns>The multiplied index.</returns>
        public static Index3 Multiply(in Index3 left, in Index3 right)
        {
            return new Index3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to index the index.</param>
        /// <param name="result">When the method completes, contains the scaled index.</param>
        public static void Divide(ref Index3 value, int scale, out Index3 result)
        {
            result.X = value.X / scale;
            result.Y = value.Y / scale;
            result.Z = value.Z / scale;
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 Divide(in Index3 value, int scale)
        {
            return new Index3(value.X / scale, value.Y / scale, value.Z / scale);
        }

        /// <summary>
        ///     Scales a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first index to multiply.</param>
        /// <param name="right">The second index to multiply.</param>
        /// <param name="result">When the method completes, contains the multiplied index.</param>
        public static void Divide(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X / right.X;
            result.Y = left.Y / right.Y;
            result.Z = left.Z / right.Z;
        }

        /// <summary>
        ///     Scales a index with another by performing component-wise multiplication.
        /// </summary>
        /// <param name="left">The first index to multiply.</param>
        /// <param name="right">The index index to multiply.</param>
        /// <returns>The multiplied index.</returns>
        public static Index3 Divide(in Index3 left, in Index3 right)
        {
            return new Index3(left.X / right.X, left.Y / right.Y, left.Z / right.Z);
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value">The index to negate.</param>
        /// <param name="result">When the method completes, contains a index facing in the opposite direction.</param>
        public static void Negate(ref Index3 value, out Index3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value">The index to negate.</param>
        /// <returns>A index facing in the opposite direction.</returns>
        public static Index3 Negate(in Index3 value)
        {
            return new Index3(-value.X, -value.Y, -value.Z);
        }

        /// <summary>
        ///     Returns per component absolute value of a index
        /// </summary>
        /// <param name="value">Input index</param>
        /// <param name="result">
        ///     When the method completes, contains a index with each component being the absolute value of the
        ///     input component
        /// </param>
        public static void Abs(ref Index3 value, out Index3 result)
        {
            result.X = (value.X + (value.X >> 31)) ^ (value.X >> 31);
            result.Y = (value.Y + (value.Y >> 31)) ^ (value.Y >> 31);
            result.Z = (value.Z + (value.Z >> 31)) ^ (value.Z >> 31);
        }

        /// <summary>
        ///     Returns per component absolute value of a index
        /// </summary>
        /// <param name="value">Input index</param>
        /// <returns>A index with each component being the absolute value of the input component</returns>
        public static Index3 Abs(in Index3 value)
        {
            return new Index3(
                (value.X + (value.X >> 31)) ^ (value.X >> 31),
                (value.Y + (value.Y >> 31)) ^ (value.Y >> 31),
                (value.Z + (value.Z >> 31)) ^ (value.Z >> 31));
        }

        /// <summary>
        ///     Calculates the distance between two indices.
        /// </summary>
        /// <param name="value1">The first index.</param>
        /// <param name="value2">The second index.</param>
        /// <param name="result">When the method completes, contains the distance between the two indices.</param>
        public static void Distance(ref Index3 value1, ref Index3 value2, out double result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            result = Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        ///     Calculates the distance between two indices.
        /// </summary>
        /// <param name="value1">The first index.</param>
        /// <param name="value2">The second index.</param>
        /// <returns>The distance between the two indices.</returns>
        public static double Distance(in Index3 value1, in Index3 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            return Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        /// <summary>
        ///     Calculates the squared distance between two indices.
        /// </summary>
        /// <param name="value1">The first index.</param>
        /// <param name="value2">The second index</param>
        /// <param name="result">When the method completes, contains the squared distance between the two indices.</param>
        public static void DistanceSquared(ref Index3 value1, ref Index3 value2, out int result)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            result = (x * x) + (y * y) + (z * z);
        }

        /// <summary>
        ///     Calculates the squared distance between two indices.
        /// </summary>
        /// <param name="value1">The first index.</param>
        /// <param name="value2">The second index.</param>
        /// <returns>The squared distance between the two indices.</returns>
        public static int DistanceSquared(in Index3 value1, in Index3 value2)
        {
            int x = value1.X - value2.X;
            int y = value1.Y - value2.Y;
            int z = value1.Z - value2.Z;

            return (x * x) + (y * y) + (z * z);
        }

        /// <summary>
        ///     Calculates the dot product of two indices.
        /// </summary>
        /// <param name="left">First source index.</param>
        /// <param name="right">Second source index.</param>
        /// <param name="result">When the method completes, contains the dot product of the two indices.</param>
        public static void Dot(ref Index3 left, ref Index3 right, out int result)
        {
            result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        /// <summary>
        ///     Calculates the dot product of two indices.
        /// </summary>
        /// <param name="left">First source index.</param>
        /// <param name="right">Second source index.</param>
        /// <returns>The dot product of the two indices.</returns>
        public static int Dot(in Index3 left, in Index3 right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        /// <summary>
        ///     Returns a index containing the largest components of the specified indices.
        /// </summary>
        /// <param name="left">The first source index.</param>
        /// <param name="right">The second source index.</param>
        /// <param name="result">
        ///     When the method completes, contains an new index composed of the largest components of the source
        ///     indices.
        /// </param>
        public static void Max(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X > right.X ? left.X : right.X;
            result.Y = left.Y > right.Y ? left.Y : right.Y;
            result.Z = left.Z > right.Z ? left.Z : right.Z;
        }

        /// <summary>
        ///     Returns a index containing the largest components of the specified indices.
        /// </summary>
        /// <param name="left">The first source index.</param>
        /// <param name="right">The second source index.</param>
        /// <returns>A index containing the largest components of the source indices.</returns>
        public static Index3 Max(in Index3 left, in Index3 right)
        {
            return new Index3(
                left.X > right.X ? left.X : right.X,
                left.Y > right.Y ? left.Y : right.Y,
                left.Z > right.Z ? left.Z : right.Z);
        }

        /// <summary>
        ///     Returns a index containing the smallest components of the specified indices.
        /// </summary>
        /// <param name="left">The first source index.</param>
        /// <param name="right">The second source index.</param>
        /// <param name="result">
        ///     When the method completes, contains an new index composed of the smallest components of the source
        ///     indices.
        /// </param>
        public static void Min(ref Index3 left, ref Index3 right, out Index3 result)
        {
            result.X = left.X < right.X ? left.X : right.X;
            result.Y = left.Y < right.Y ? left.Y : right.Y;
            result.Z = left.Z < right.Z ? left.Z : right.Z;
        }

        /// <summary>
        ///     Returns a index containing the smallest components of the specified indices.
        /// </summary>
        /// <param name="left">The first source index.</param>
        /// <param name="right">The second source index.</param>
        /// <returns>A index containing the smallest components of the source indices.</returns>
        public static Index3 Min(in Index3 left, in Index3 right)
        {
            return new Index3(
                left.X < right.X ? left.X : right.X,
                left.Y < right.Y ? left.Y : right.Y,
                left.Z < right.Z ? left.Z : right.Z);
        }

        /// <summary>
        ///     Adds two indices.
        /// </summary>
        /// <param name="left">The first index to add.</param>
        /// <param name="right">The second index to add.</param>
        /// <returns>The sum of the two indices.</returns>
        public static Index3 operator +(in Index3 left, in Index3 right)
        {
            return new Index3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        /// <summary>
        ///     Perform a component-wise addition
        /// </summary>
        /// <param name="value">The input index.</param>
        /// <param name="scalar">The scalar value to be added on elements</param>
        /// <returns>The index with added scalar for each element.</returns>
        public static Index3 operator +(in Index3 value, int scalar)
        {
            return new Index3(value.X + scalar, value.Y + scalar, value.Z + scalar);
        }

        /// <summary>
        ///     Perform a component-wise addition
        /// </summary>
        /// <param name="value">The input index.</param>
        /// <param name="scalar">The scalar value to be added on elements</param>
        /// <returns>The index with added scalar for each element.</returns>
        public static Index3 operator +(int scalar, in Index3 value)
        {
            return new Index3(scalar + value.X, scalar + value.Y, scalar + value.Z);
        }

        /// <summary>
        ///     Assert a index (return it unchanged).
        /// </summary>
        /// <param name="value">The index to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) index.</returns>
        public static Index3 operator +(in Index3 value)
        {
            return value;
        }

        /// <summary>
        ///     Subtract two indices.
        /// </summary>
        /// <param name="left">The first index be subtracted from.</param>
        /// <param name="right">The second index to subtract.</param>
        /// <returns>The sum of the two indices.</returns>
        public static Index3 operator -(in Index3 left, in Index3 right)
        {
            return new Index3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="value">The input index.</param>
        /// <param name="scalar">The scalar value to be subtracted from elements</param>
        /// <returns>The index with subtracted scalar from each element.</returns>
        public static Index3 operator -(in Index3 value, int scalar)
        {
            return new Index3(value.X - scalar, value.Y - scalar, value.Z - scalar);
        }

        /// <summary>
        ///     Perform a component-wise subtraction
        /// </summary>
        /// <param name="value">The input index.</param>
        /// <param name="scalar">The scalar value to be subtracted from elements</param>
        /// <returns>The index with subtracted scalar from each element.</returns>
        public static Index3 operator -(int scalar, in Index3 value)
        {
            return new Index3(scalar - value.X, scalar - value.Y, scalar - value.Z);
        }

        /// <summary>
        ///     Reverses the direction of a given index.
        /// </summary>
        /// <param name="value">The index to negate.</param>
        /// <returns>A index facing in the opposite direction.</returns>
        public static Index3 operator -(in Index3 value)
        {
            return new Index3(-value.X, -value.Y, -value.Z);
        }

        /// <summary>
        ///     Multiplies a index with another by performing component-wise multiplication equivalent to
        ///     <see cref="Multiply(ref Index3, ref Index3, out Index3)" />.
        /// </summary>
        /// <param name="left">The first index to multiply.</param>
        /// <param name="right">The second index to multiply.</param>
        /// <returns>The multiplication of the two indices.</returns>
        public static Index3 operator *(in Index3 left, in Index3 right)
        {
            return new Index3(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 operator *(int scale, in Index3 value)
        {
            return new Index3(value.X * scale, value.Y * scale, value.Z * scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 operator *(in Index3 value, int scale)
        {
            return new Index3(value.X * scale, value.Y * scale, value.Z * scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 operator /(in Index3 value, int scale)
        {
            return new Index3(value.X / scale, value.Y / scale, value.Z / scale);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <param name="value">The index to scale.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 operator /(int scale, in Index3 value)
        {
            return new Index3(scale / value.X, scale / value.Y, scale / value.Z);
        }

        /// <summary>
        ///     Scales a index by the given value.
        /// </summary>
        /// <param name="value">The index to scale.</param>
        /// <param name="scale">The amount by which to scale the index.</param>
        /// <returns>The scaled index.</returns>
        public static Index3 operator /(in Index3 value, in Index3 scale)
        {
            return new Index3(value.X / scale.X, value.Y / scale.Y, value.Z / scale.Z);
        }

        /// <summary>
        ///     Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> has the same value as <paramref name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in Index3 left, in Index3 right)
        {
            return left.Equals(in right);
        }

        /// <summary>
        ///     Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> has a different value than <paramref name="right" />; otherwise,
        ///     <c>false</c>.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in Index3 left, in Index3 right)
        {
            return !left.Equals(in right);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="int" /> to <see cref="Index3" />.
        ///     equal to <see cref="Index3" /> (value)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Index3(int value)
        {
            return new Index3(value, value, value);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="int" /> to <see cref="Index3" />.
        ///     equal to <see cref="Index3" /> (value.x, value.y, 0)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Index3(in Index2 value)
        {
            return new Index3(value.X, value.Y, 0);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Index3" /> to <see cref="Vector3" />.
        ///     equal to <see cref="Vector3" /> (value.x, value.y, value.z)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector3(in Index3 value)
        {
            return new Vector3(value.X, value.Y, value.Z);
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="Vector3" /> to <see cref="Index3" />.
        ///     equal to <see cref="Index3" /> ((int)value.x, (int)value.y, (int)value.z)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Index3(in Vector3 value)
        {
            return new Index3((int)value.X, (int)value.Y, (int)value.Z);
        }
    }
}
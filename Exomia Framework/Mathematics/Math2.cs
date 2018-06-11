#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Mathematics
{
    /// <summary>
    ///     Math2 static class
    /// </summary>
    public static class Math2
    {
        #region Variables

        private const long L_OFFSET_MAX = int.MaxValue + 1L;

        #endregion

        #region Methods

        /// <summary>
        ///     Maps a value from l1 to u1 to l2 to u2
        /// </summary>
        /// <param name="v">Value </param>
        /// <param name="l1">Lower 1</param>
        /// <param name="u1">Upper 1</param>
        /// <param name="l2">Lower 2</param>
        /// <param name="u2">Upper 2</param>
        /// <returns>maped value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Map(float v, float l1, float u1, float l2, float u2)
        {
            return (v - l1) / (u1 - l1) * (u2 - l2) + l2;
        }

        /// <summary>
        ///     Maps a value from l1 to u1 to l2 to u2
        /// </summary>
        /// <param name="v">Value </param>
        /// <param name="l1">Lower 1</param>
        /// <param name="u1">Upper 1</param>
        /// <param name="l2">Lower 2</param>
        /// <param name="u2">Upper 2</param>
        /// <returns>maped value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Map(double v, double l1, double u1, double l2, double u2)
        {
            return (v - l1) / (u1 - l1) * (u2 - l2) + l2;
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte Lerp(byte a, byte b, double t)
        {
            return (byte)(a + t * (b - a));
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, double t)
        {
            return (float)(a + t * (b - a));
        }

        /// <summary>
        ///     Interpolates between two values using a linear function by a given amount.
        /// </summary>
        /// <param name="a">Value to interpolate from.</param>
        /// <param name="b">Value to interpolate to.</param>
        /// <param name="t">Interpolation amount.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Lerp(double a, double b, double t)
        {
            return a + t * (b - a);
        }

        /// <summary>
        ///     LinearInterpolate
        /// </summary>
        /// <param name="a">a</param>
        /// <param name="b">b</param>
        /// <param name="t">t</param>
        /// <returns>new Vector2</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Lerp(Vector2 a, Vector2 b, double t)
        {
            return new Vector2(Lerp(a.X, b.X, t), Lerp(a.Y, b.Y, t));
        }

        /// <summary>
        ///     raise b to the power of e
        /// </summary>
        /// <param name="b">base</param>
        /// <param name="e">exponent</param>
        /// <returns>b^e</returns>
        public static double Pow(double b, int e)
        {
            if (e < 0) { throw new ArgumentException("e must be positive", nameof(e)); }

            while (true)
            {
                if (e == 0) { return 1; }
                if (b == 0) { return 0; }
                if ((e & 1) == 0)
                {
                    b = b * b;
                    e = e >> 1;
                    continue;
                }
                return b * Pow(b * b, e >> 1);
            }
        }

        /// <summary>
        ///     raise b to the power of e
        /// </summary>
        /// <param name="b">base</param>
        /// <param name="e">exponent</param>
        /// <returns>b^e</returns>
        public static float Pow(float b, int e)
        {
            if (e < 0) { throw new ArgumentException("e must be positive", nameof(e)); }

            while (true)
            {
                if (e == 0) { return 1; }
                if (b == 0) { return 0; }
                if ((e & 1) == 0)
                {
                    b = b * b;
                    e = e >> 1;
                    continue;
                }
                return b * Pow(b * b, e >> 1);
            }
        }

        /// <summary>
        ///     calculates the absolute value of x
        /// </summary>
        /// <param name="x">value</param>
        /// <returns>positive x</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(int x)
        {
            return (x + (x >> 31)) ^ (x >> 31);
        }

        /// <summary>
        ///     Returns the largest integer less than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="f">A floating-point number with single precision</param>
        /// <returns>The largest integer, which is less than or equal to f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Floor(double f)
        {
            return (int)((long)(f + L_OFFSET_MAX) - L_OFFSET_MAX);
        }

        /// <summary>
        ///     Returns the smallest integer greater than or equal to the specified floating-point number.
        /// </summary>
        /// <param name="f">A floating-point number with single precision</param>
        /// <returns>The smalles integer, which is greater than or equal to f.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Ceiling(double f)
        {
            return (int)(L_OFFSET_MAX - (long)(L_OFFSET_MAX - f));
        }

        /// <summary>
        ///     creates an axis aligned bounding box
        /// </summary>
        /// <param name="transform">transform</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <param name="aabb">out aabb</param>
        public static void CreateAABB(in Matrix transform, float width, float height, out RectangleF aabb)
        {
            Vector2 leftTop = Vector2.Zero.Transform(transform);
            Vector2 rightTop = new Vector2(width, 0).Transform(transform);
            Vector2 leftBottom = new Vector2(0, height).Transform(transform);
            Vector2 rightBottom = new Vector2(width, height).Transform(transform);

            Vector2 min = new Vector2(
                Math.Min(leftTop.X, Math.Min(rightTop.X, Math.Min(leftBottom.X, rightBottom.X))),
                Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y))));
            Vector2 max = new Vector2(
                Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X))),
                Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y))));

            aabb = new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        /// <summary>
        ///     creates an axis aligned bounding box
        /// </summary>
        /// <param name="transform">transform</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns>axis aligned bounding box</returns>
        public static RectangleF CreateAABB(in Matrix transform, float width, float height)
        {
            Vector2 leftTop = Vector2.Zero.Transform(transform);
            Vector2 rightTop = new Vector2(width, 0).Transform(transform);
            Vector2 leftBottom = new Vector2(0, height).Transform(transform);
            Vector2 rightBottom = new Vector2(width, height).Transform(transform);

            Vector2 min = new Vector2(
                Math.Min(leftTop.X, Math.Min(rightTop.X, Math.Min(leftBottom.X, rightBottom.X))),
                Math.Min(leftTop.Y, Math.Min(rightTop.Y, Math.Min(leftBottom.Y, rightBottom.Y))));
            Vector2 max = new Vector2(
                Math.Max(leftTop.X, Math.Max(rightTop.X, Math.Max(leftBottom.X, rightBottom.X))),
                Math.Max(leftTop.Y, Math.Max(rightTop.Y, Math.Max(leftBottom.Y, rightBottom.Y))));

            return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        /// <summary>
        ///     creates an axis aligned bounding box
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="origin">origin</param>
        /// <param name="scale">scale</param>
        /// <param name="rotation">rotation</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        /// <returns>axis aligned bounding box</returns>
        public static RectangleF CreateAABB(in Vector2 position, in Vector2 origin, in Vector2 scale, float rotation,
            float width, float height)
        {
            return CreateAABB(CalculateTransformMatrix(position, origin, scale, rotation), width, height);
        }

        /// <summary>
        ///     calculates a transform matrix
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="origin">origin</param>
        /// <param name="scale">scale</param>
        /// <param name="rotation">rotation</param>
        /// <param name="transform">out transform matrix</param>
        public static void CalculateTransformMatrix(in Vector2 position, in Vector2 origin, in Vector2 scale,
            float rotation, out Matrix transform)
        {
            transform = Matrix.Translation(-origin.X, -origin.Y, 0) *
                        Matrix.RotationZ(rotation) *
                        Matrix.Scaling(scale.X, scale.Y, 0.0f) *
                        Matrix.Translation(position.X, position.Y, 0);
        }

        /// <summary>
        ///     calculates a transform matrix
        /// </summary>
        /// <param name="position">position</param>
        /// <param name="origin">origin</param>
        /// <param name="scale">scale</param>
        /// <param name="rotation">rotation</param>
        /// <returns>transform matrix</returns>
        public static Matrix CalculateTransformMatrix(in Vector2 position, in Vector2 origin, in Vector2 scale,
            float rotation)
        {
            return Matrix.Translation(-origin.X, -origin.Y, 0) *
                   Matrix.RotationZ(rotation) *
                   Matrix.Scaling(scale.X, scale.Y, 0.0f) *
                   Matrix.Translation(position.X, position.Y, 0);
        }

        #endregion

        #region Curves

        /// <summary>
        ///     Hermite Curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CurveHermite(float t)
        {
            return t * t * (3 - 2 * t);
        }

        /// <summary>
        ///     Quintic Curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CurveQuintic(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        ///     Hermite Curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CurveHermite(double t)
        {
            return t * t * (3 - 2 * t);
        }

        /// <summary>
        ///     Quintic Curve
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CurveQuintic(double t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, it is returned unchanged.
        ///     If it is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RoundUpToPowerOfTwo(int value)
        {
            return value <= 0 ? 0 : (int)RoundUpToPowerOfTwo((uint)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, or zero, it is returned unchanged.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint RoundUpToPowerOfTwo(uint value)
        {
            if (value > 0x80000000)
            {
                throw new ArgumentOutOfRangeException();
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            return value + 1;
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, it is returned unchanged.
        ///     If it is negative or zero, zero is returned.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long RoundUpToPowerOfTwo(long value)
        {
            return value <= 0 ? 0 : (long)RoundUpToPowerOfTwo((ulong)value);
        }

        /// <summary>
        ///     Rounds the given value up to a power of two.
        ///     If it is already a power of two, or zero, it is returned unchanged.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong RoundUpToPowerOfTwo(ulong value)
        {
            if (value > 0x8000000000000000)
            {
                throw new ArgumentOutOfRangeException();
            }
            value--;
            value |= value >> 1;
            value |= value >> 2;
            value |= value >> 4;
            value |= value >> 8;
            value |= value >> 16;
            value |= value >> 32;
            return value + 1;
        }

        #endregion
    }
}
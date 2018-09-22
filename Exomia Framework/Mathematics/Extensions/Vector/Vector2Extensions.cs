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

namespace Exomia.Framework.Mathematics.Extensions.Vector
{
    /// <summary>
    ///     Vector2Extensions static class
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        ///     calculate the angle between two vectors
        /// </summary>
        /// <param name="vec1">this vec1</param>
        /// <param name="vec2">vec2</param>
        /// <returns>angle between the two vectors in radians</returns>
        public static double AngleBetween(this Vector2 vec1, in Vector2 vec2)
        {
            float scalar = vec1.X * vec2.X + vec1.Y * vec2.Y;
            float length = vec1.Length() * vec2.Length();
            return Math.Cos(scalar / length);
        }

        /// <summary>
        ///     calculates the horizontal angle of a vector2
        /// </summary>
        /// <param name="vec">this vec</param>
        /// <returns>angle horizontal</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngleHorizontal(this Vector2 vec)
        {
            return Math.Atan2(vec.Y, vec.X);
        }

        /// <summary>
        ///     calculates the vertical angle of a vector2
        /// </summary>
        /// <param name="vec">this vec</param>
        /// <returns>angle vertical</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngleVertical(this Vector2 vec)
        {
            return Math.Atan2(vec.X, vec.Y);
        }

        /// <summary>
        ///     rotate a vector by an angle (in radian)
        /// </summary>
        /// <param name="vec">this vec</param>
        /// <param name="angle">angle</param>
        /// <returns>new rotated vector2</returns>
        public static Vector2 Rotate(this Vector2 vec, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return new Vector2((float)(vec.X * cos - vec.Y * sin), (float)(vec.X * sin + vec.Y * cos));
        }

        /// <summary>
        ///     transforms the vector with a transform matrix
        /// </summary>
        /// <param name="vec">this vec</param>
        /// <param name="transform">transform</param>
        /// <returns>new vector2</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(this Vector2 vec, in Matrix transform)
        {
            return new Vector2(
                vec.X * transform.M11 + vec.Y * transform.M21 + transform.M41,
                vec.X * transform.M12 + vec.Y * transform.M22 + transform.M42);
        }
    }
}
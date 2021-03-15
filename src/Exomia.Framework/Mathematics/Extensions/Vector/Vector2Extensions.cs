#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Mathematics.Extensions.Vector
{
    /// <summary>
    ///     A vector 2 extensions class.
    /// </summary>
    public static class Vector2Extensions
    {
        /// <summary>
        ///     Calculate the angle from the anchor point to another point vector.
        /// </summary>
        /// <param name="anchor"> This anchor <see cref="Vector2" />. </param>
        /// <param name="point"> The point <see cref="Vector2" />. </param>
        /// <returns>
        ///     The angle from anchor vector to the point vector in radians.
        /// </returns>
        public static double AngleTo(this Vector2 anchor, in Vector2 point)
        {
            return Math.Atan2(point.Y - anchor.Y, point.X - anchor.X);
        }

        /// <summary>
        ///     Calculate the angle between two vectors.
        /// </summary>
        /// <param name="vec1"> This <see cref="Vector2" />. </param>
        /// <param name="vec2"> The <see cref="Vector2" />. </param>
        /// <returns>
        ///     The angle between the two vectors in radians.
        /// </returns>
        public static double AngleBetween(this Vector2 vec1, in Vector2 vec2)
        {
            return Math.Atan2(
                (vec1.X * vec2.Y) - (vec2.X * vec1.Y),
                (vec1.X * vec2.X) + (vec1.Y * vec2.Y));
        }

        /// <summary>
        ///     Calculates the horizontal angle of a <see cref="Vector2" />.
        /// </summary>
        /// <param name="vec"> This <see cref="Vector2" />. </param>
        /// <returns>
        ///     The angle horizontal.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngleHorizontal(this Vector2 vec)
        {
            return Math.Atan2(vec.Y, vec.X);
        }

        /// <summary>
        ///     Calculates the vertical angle of a <see cref="Vector2" />.
        /// </summary>
        /// <param name="vec"> This <see cref="Vector2" />. </param>
        /// <returns>
        ///     The angle vertical.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double AngleVertical(this Vector2 vec)
        {
            return Math.Atan2(vec.X, vec.Y);
        }

        /// <summary>
        ///     Rotate a <see cref="Vector2" /> by an angle (in radian)
        /// </summary>
        /// <param name="vec">   This <see cref="Vector2" />. </param>
        /// <param name="angle"> angle. </param>
        /// <returns>
        ///     The new rotated <see cref="Vector2" />.
        /// </returns>
        public static Vector2 Rotate(this Vector2 vec, double angle)
        {
            double sin = Math.Sin(angle);
            double cos = Math.Cos(angle);
            return new Vector2((float)((vec.X * cos) - (vec.Y * sin)), (float)((vec.X * sin) + (vec.Y * cos)));
        }

        /// <summary>
        ///     Transforms the <see cref="Vector2" /> with a transform <see cref="Matrix4x4" />.
        /// </summary>
        /// <param name="vec">       this vec. </param>
        /// <param name="transform"> transform. </param>
        /// <returns>
        ///     the new <see cref="Vector2" />.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Transform(this Vector2 vec, in Matrix4x4 transform)
        {
            return new Vector2(
                (vec.X * transform.M11) + (vec.Y * transform.M21) + transform.M41,
                (vec.X * transform.M12) + (vec.Y * transform.M22) + transform.M42);
        }
    }
}
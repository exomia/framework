#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

namespace Exomia.Framework.Mathematics
{
    /// <content>
    ///     The mathematics 2.
    /// </content>
    public static partial class Math2
    {
        /// <summary>
        ///     Hermite Curve.
        /// </summary>
        /// <param name="t"> t. </param>
        /// <returns>
        ///     A double.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CurveHermite(float t)
        {
            return t * t * (3 - (2 * t));
        }

        /// <summary>
        ///     Hermite Curve.
        /// </summary>
        /// <param name="t"> t. </param>
        /// <returns>
        ///     A double.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CurveHermite(double t)
        {
            return t * t * (3 - (2 * t));
        }

        /// <summary>
        ///     Quintic Curve.
        /// </summary>
        /// <param name="t"> t. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float CurveQuintic(float t)
        {
            return t * t * t * ((t * ((t * 6) - 15)) + 10);
        }

        /// <summary>
        ///     Quintic Curve.
        /// </summary>
        /// <param name="t"> t. </param>
        /// <returns>
        ///     A double.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double CurveQuintic(double t)
        {
            return t * t * t * ((t * ((t * 6) - 15)) + 10);
        }
    }
}
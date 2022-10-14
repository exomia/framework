﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Mathematics.Extensions.Numeric;

/// <summary> adds extensions for numerical types. </summary>
public static class NumericExtensions
{
    private const double PI_OVER_180_D = Math.PI / 180.0;
    private const float  PI_OVER_180_F = (float)(Math.PI / 180.0);
    private const double I80_OVER_PI_D = 180.0 / Math.PI;
    private const float  I80_OVER_PI_F = (float)(180.0 / Math.PI);

    /// <summary> Convert to Degrees. </summary>
    /// <param name="value"> The value to convert to degrees. </param>
    /// <returns> The value in degrees. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDegree(this double value)
    {
        return value * I80_OVER_PI_D;
    }

    /// <summary> Convert to Degrees. </summary>
    /// <param name="value"> The value to convert to degrees. </param>
    /// <returns> The value in degrees. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegreeF(this float value)
    {
        return value * I80_OVER_PI_F;
    }

    /// <summary> Convert to Degrees. </summary>
    /// <param name="value"> The value to convert to degrees. </param>
    /// <returns> The value in degrees. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToDegree(this long value)
    {
        return value * I80_OVER_PI_D;
    }
    
    /// <summary> Convert to Degrees. </summary>
    /// <param name="value"> The value to convert to degrees. </param>
    /// <returns> The value in degrees. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegreeF(this int value)
    {
        return value * I80_OVER_PI_F;
    }

    /// <summary> Convert to Radians. </summary>
    /// <param name="value"> The value to convert to radians. </param>
    /// <returns> The value in radians. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToRadians(this double value)
    {
        return PI_OVER_180_D * value;
    }

    /// <summary> Convert to Radians. </summary>
    /// <param name="value"> The value to convert to radians. </param>
    /// <returns> The value in radians. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadiansF(this float value)
    {
        return PI_OVER_180_F * value;
    }

    /// <summary> Convert to Radians. </summary>
    /// <param name="value"> The value to convert to radians. </param>
    /// <returns> The value in radians. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToRadians(this long value)
    {
        return PI_OVER_180_D * value;
    }
    
    /// <summary> Convert to Radians. </summary>
    /// <param name="value"> The value to convert to radians. </param>
    /// <returns> The value in radians. </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadiansF(this int value)
    {
        return PI_OVER_180_F * value;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Noise;

/// <summary>
///     An interface for calculating 1d, 2d and 3d noise maps.
/// </summary>
public interface INoise
{
    /// <summary>
    ///     generates a 1-dimensional noise array in range from -1 to 1.
    /// </summary>
    /// <param name="x"> The x coordinate. </param>
    /// <param name="xMax"> The x maximum. </param>
    /// <returns>
    ///     1-dimensional noise array in range from -1 to 1.
    /// </returns>
    float[] GenerateNoise1D(int x, int xMax);

    /// <summary>
    ///     generates a 2-dimensional noise array in range from -1 to 1.
    /// </summary>
    /// <param name="x"> The x coordinate. </param>
    /// <param name="y"> The y coordinate. </param>
    /// <param name="xMax"> The x maximum. </param>
    /// <param name="yMax"> The y maximum. </param>
    /// <returns>
    ///     2-dimensional noise array in range from -1 to 1.
    /// </returns>
    float[,] GenerateNoise2D(int x, int y, int xMax, int yMax);

    /// <summary>
    ///     generates a 3-dimensional noise array in range from -1 to 1.
    /// </summary>
    /// <param name="x"> The x coordinate. </param>
    /// <param name="y"> The y coordinate. </param>
    /// <param name="z"> The z coordinate. </param>
    /// <param name="xMax"> The x maximum. </param>
    /// <param name="yMax"> The y maximum. </param>
    /// <param name="zMax"> The z maximum. </param>
    /// <returns>
    ///     3-dimensional noise array in range from -1 to 1.
    /// </returns>
    float[,,] GenerateNoise3D(int x, int y, int z, int xMax, int yMax, int zMax);

    /// <summary>
    ///     generates a noise.
    /// </summary>
    /// <param name="x"> . </param>
    /// <returns>
    ///     noise in range from -1 to 1.
    /// </returns>
    float Noise1D(int x);

    /// <summary>
    ///     generates a noise.
    /// </summary>
    /// <param name="x"> The x coordinate. </param>
    /// <param name="y"> The y coordinate. </param>
    /// <returns>
    ///     noise in range from -1 to 1.
    /// </returns>
    float Noise2D(int x, int y);

    /// <summary>
    ///     generates a noise.
    /// </summary>
    /// <param name="x"> The x coordinate. </param>
    /// <param name="y"> The y coordinate. </param>
    /// <param name="z"> The z coordinate. </param>
    /// <returns>
    ///     noise in range from -1 to 1.
    /// </returns>
    float Noise3D(int x, int y, int z);
}
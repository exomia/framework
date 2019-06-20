#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Noise
{
    /// <summary>
    ///     NoiseInterpolationType enum.
    /// </summary>
    public enum NoiseInterpolationType
    {
        /// <summary>
        ///     Linear
        /// </summary>
        Linear,

        /// <summary>
        ///     Hermite
        /// </summary>
        Hermite,

        /// <summary>
        ///     Quintic
        /// </summary>
        Quintic
    }

    /// <summary>
    ///     NoiseFractalType enum.
    /// </summary>
    public enum NoiseFractalType
    {
        /// <summary>
        ///     NONE
        /// </summary>
        None,

        /// <summary>
        ///     BrownianMotion
        /// </summary>
        BrownianMotion,

        /// <summary>
        ///     Billow
        /// </summary>
        Billow,

        /// <summary>
        ///     RigidMulti
        /// </summary>
        RigidMulti
    }

    /// <summary>
    ///     An interface for calculating 1d, 2d and 3d noise maps.
    /// </summary>
    public interface INoise
    {
        /// <summary>
        ///     generates a 1-dimensional noise array in range from -1 to 1.
        /// </summary>
        /// <param name="x">    . </param>
        /// <param name="xMax"> . </param>
        /// <returns>
        ///     1-dimensional noise array in range from -1 to 1.
        /// </returns>
        float[] GenerateNoise1D(int x, int xMax);

        /// <summary>
        ///     generates a 2-dimensional noise array in range from -1 to 1.
        /// </summary>
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        /// <param name="xMax"> . </param>
        /// <param name="yMax"> . </param>
        /// <returns>
        ///     2-dimensional noise array in range from -1 to 1.
        /// </returns>
        float[,] GenerateNoise2D(int x, int y, int xMax, int yMax);

        /// <summary>
        ///     generates a 3-dimensional noise array in range from -1 to 1.
        /// </summary>
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        /// <param name="z">    . </param>
        /// <param name="xMax"> . </param>
        /// <param name="yMax"> . </param>
        /// <param name="zMax"> . </param>
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
        /// <param name="x"> . </param>
        /// <param name="y"> . </param>
        /// <returns>
        ///     noise in range from -1 to 1.
        /// </returns>
        float Noise2D(int x, int y);

        /// <summary>
        ///     generates a noise.
        /// </summary>
        /// <param name="x"> . </param>
        /// <param name="y"> . </param>
        /// <param name="z"> . </param>
        /// <returns>
        ///     noise in range from -1 to 1.
        /// </returns>
        float Noise3D(int x, int y, int z);
    }
}
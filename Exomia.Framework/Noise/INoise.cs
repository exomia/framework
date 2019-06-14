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
namespace Exomia.Framework.Noise
{
    /// <summary>
    ///     NoiseInterpolationType enum
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
    ///     NoiseFractalType enum
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
    ///     An interface for calculating 1d, 2d and 3d noise maps
    /// </summary>
    public interface INoise
    {
        /// <summary>
        ///     generates a 1-dimensional noise array in range from -1 to 1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="xmax"></param>
        /// <returns>1-dimensional noise array in range from -1 to 1</returns>
        float[] GenerateNoise1D(int x, int xmax);

        /// <summary>
        ///     generates a noise
        /// </summary>
        /// <param name="x"></param>
        /// <returns>noise in range from -1 to 1</returns>
        float Noise1D(int x);

        /// <summary>
        ///     generates a 2-dimensional noise array in range from -1 to 1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <returns>2-dimensional noise array in range from -1 to 1</returns>
        float[,] GenerateNoise2D(int x, int y, int xmax, int ymax);

        /// <summary>
        ///     generates a noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>noise in range from -1 to 1</returns>
        float Noise2D(int x, int y);

        /// <summary>
        ///     generates a 3-dimensional noise array in range from -1 to 1
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="xmax"></param>
        /// <param name="ymax"></param>
        /// <param name="zmax"></param>
        /// <returns>3-dimensional noise array in range from -1 to 1</returns>
        float[,,] GenerateNoise3D(int x, int y, int z, int xmax, int ymax, int zmax);

        /// <summary>
        ///     generates a noise
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns>noise in range from -1 to 1</returns>
        float Noise3D(int x, int y, int z);
    }
}
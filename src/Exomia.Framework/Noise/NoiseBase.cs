#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Noise
{
    /// <summary>
    ///     A noise base.
    /// </summary>
    public abstract class NoiseBase : INoise
    {
        /// <summary>
        ///     The prime.
        /// </summary>
        protected const int X_PRIME = 1103;

        /// <summary>
        ///     The prime.
        /// </summary>
        protected const int Y_PRIME = 29401;

        /// <summary>
        ///     The prime.
        /// </summary>
        protected const int Z_PRIME = 6833;

        /// <summary>
        ///     The prime.
        /// </summary>
        protected const int W_PRIME = 1259;

        /// <summary>
        ///     The prime.
        /// </summary>
        protected const int U_PRIME = 58193;

        /// <summary>
        ///     The graduated 1 d.
        /// </summary>
        protected static readonly float[] Grad_1D = { -1.0f, 1.0f };

        /// <summary>
        ///     The graduated 2 d.
        /// </summary>
        protected static readonly Vector2[] Grad_2D =
        {
            new Vector2(-1.0f, -1.0f), new Vector2(1.0f, -1.0f), new Vector2(-1.0f, 1.0f), new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, -1.0f), new Vector2(-1.0f, 0.0f), new Vector2(0.0f, 1.0f), new Vector2(1.0f, 0.0f)
        };

        /// <summary>
        ///     The graduated 3 d.
        /// </summary>
        protected static readonly Vector3[] Grad_3D =
        {
            new Vector3(1.0f, 1.0f, 0.0f), new Vector3(-1.0f, 1.0f, 0.0f), new Vector3(1.0f, -1.0f, 0.0f),
            new Vector3(-1.0f, -1.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f), new Vector3(-1.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, -1.0f), new Vector3(-1.0f, 0.0f, -1.0f), new Vector3(0.0f, 1.0f, 1.0f),
            new Vector3(0.0f, -1.0f, 1.0f), new Vector3(0.0f, 1.0f, -1.0f), new Vector3(0.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 0.0f), new Vector3(0.0f, -1.0f, 1.0f), new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, -1.0f, -1.0f)
        };

        /// <summary>
        ///     The fractal bounding.
        /// </summary>
        protected float _fractalBounding;

        /// <summary>
        ///     The frequency.
        /// </summary>
        protected double _frequency;

        /// <summary>
        ///     The gain.
        /// </summary>
        protected float _gain;

        /// <summary>
        ///     The lacunarity.
        /// </summary>
        protected double _lacunarity;

        /// <summary>
        ///     Type of the noise fractal.
        /// </summary>
        protected NoiseFractalType _noiseFractalType;

        /// <summary>
        ///     Type of the noise interpolation.
        /// </summary>
        protected NoiseInterpolationType _noiseInterpolationType;

        /// <summary>
        ///     The octaves.
        /// </summary>
        protected int _octaves;

        /// <summary>
        ///     The seed.
        /// </summary>
        protected int _seed;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoiseBase" /> class.
        /// </summary>
        protected NoiseBase(int                    seed,
                            float                  frequency,
                            int                    octaves,
                            NoiseInterpolationType noiseInterpolationType,
                            NoiseFractalType       noiseFractalType)
            : this(seed, frequency, octaves, 2.0f, 0.5f, noiseInterpolationType, noiseFractalType) { }

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="NoiseBase" /> class.
        /// </summary>
        protected NoiseBase(int                    seed,
                            float                  frequency,
                            int                    octaves,
                            float                  lacunarity,
                            float                  gain,
                            NoiseInterpolationType noiseInterpolationType,
                            NoiseFractalType       noiseFractalType)
        {
            _seed                   = seed;
            _frequency              = frequency;
            _octaves                = octaves;
            _lacunarity             = lacunarity;
            _gain                   = gain;
            _noiseInterpolationType = noiseInterpolationType;
            _noiseFractalType       = noiseFractalType;

            CalculateFractalBounding();
        }

        /// <inheritdoc />
        public float[] GenerateNoise1D(int x, int xMax)
        {
            float[] noise = new float[xMax];
            for (int dx = 0; dx < xMax; dx++)
            {
                noise[dx] = Noise1D(dx + x);
            }
            return noise;
        }

        /// <inheritdoc />
        public float[,] GenerateNoise2D(int x, int y, int xMax, int yMax)
        {
            float[,] noise = new float[xMax, yMax];
            for (int dy = 0; dy < yMax; dy++)
            {
                for (int dx = 0; dx < xMax; dx++)
                {
                    noise[dx, dy] = Noise2D(dx + x, dy + y);
                }
            }
            return noise;
        }

        /// <inheritdoc />
        public float[,,] GenerateNoise3D(int x, int y, int z, int xMax, int yMax, int zMax)
        {
            float[,,] noise = new float[xMax, yMax, zMax];
            for (int dz = 0; dz < zMax; dz++)
            {
                for (int dy = 0; dy < yMax; dy++)
                {
                    for (int dx = 0; dx < xMax; dx++)
                    {
                        noise[dx, dy, dz] = Noise3D(dx + x, dy + y, dz + z);
                    }
                }
            }
            return noise;
        }

        /// <summary>
        ///     Calculates the fractal bounding.
        /// </summary>
        private void CalculateFractalBounding()
        {
            float amp        = _gain;
            float ampFractal = 1.0f;
            for (int i = 1; i < _octaves; i++)
            {
                ampFractal += amp;
                amp        *= _gain;
            }
            _fractalBounding = 1.0f / ampFractal;
        }

        #region Hasing

        /// <summary>
        ///     Hash 1 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int Hash1D(int seed, int x)
        {
            int hash = seed;
            hash ^= X_PRIME * x;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            return hash;
        }

        /// <summary>
        ///     Hash 2 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int Hash2D(int seed, int x, int y)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            return hash;
        }

        /// <summary>
        ///     Hash 3 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int Hash3D(int seed, int x, int y, int z)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;
            hash ^= Z_PRIME * z;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            return hash;
        }

        /// <summary>
        ///     Hash 4 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <param name="w">    The width. </param>
        /// <returns>
        ///     An int.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int Hash4D(int seed, int x, int y, int z, int w)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;
            hash ^= Z_PRIME * z;
            hash ^= W_PRIME * w;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            return hash;
        }

        /// <summary>
        ///     Value coordinate 1 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord1D(int seed, int x)
        {
            int n = seed;
            n ^= X_PRIME * x;
            return (n * n * n * U_PRIME) / 2147483648.0f;
        }

        /// <summary>
        ///     Value coordinate 2 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord2D(int seed, int x, int y)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;

            return (n * n * n * U_PRIME) / 2147483648.0f;
        }

        /// <summary>
        ///     Value coordinate 3 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord3D(int seed, int x, int y, int z)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;
            n ^= Z_PRIME * z;

            return (n * n * n * U_PRIME) / 2147483648.0f;
        }

        /// <summary>
        ///     Value coordinate 4 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <param name="w">    The width. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord4D(int seed, int x, int y, int z, int w)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;
            n ^= Z_PRIME * z;
            n ^= W_PRIME * w;

            return (n * n * n * U_PRIME) / 2147483648.0f;
        }

        /// <summary>
        ///     Graduated coordinate 1 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="xd">   The xd. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord1D(int seed, int x, double xd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            float g = Grad_1D[hash & 1];

            return (float)(xd * g);
        }

        /// <summary>
        ///     Graduated coordinate 2 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="xd">   The xd. </param>
        /// <param name="yd">   The yd. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord2D(int seed, int x, int y, double xd, double yd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            Vector2 g = Grad_2D[hash & 7];

            return (float)((xd * g.X) + (yd * g.Y));
        }

        /// <summary>
        ///     Graduated coordinate 3 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <param name="xd">   The xd. </param>
        /// <param name="yd">   The yd. </param>
        /// <param name="zd">   The zd. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord3D(int seed, int x, int y, int z, double xd, double yd, double zd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;
            hash ^= Z_PRIME * z;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            Vector3 g = Grad_3D[hash & 15];

            return (float)((xd * g.X) + (yd * g.Y) + (zd * g.Z));
        }

        /// <summary>
        ///     Graduated coordinate 4 d.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <param name="w">    The width. </param>
        /// <param name="xd">   The xd. </param>
        /// <param name="yd">   The yd. </param>
        /// <param name="zd">   The zd. </param>
        /// <param name="wd">   The wd. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord4D(int    seed,
                                           int    x,
                                           int    y,
                                           int    z,
                                           int    w,
                                           double xd,
                                           double yd,
                                           double zd,
                                           double wd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;
            hash ^= Z_PRIME * z;
            hash ^= W_PRIME * w;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            hash &= 31;
            double a = yd, b = zd, c = wd;
            switch (hash >> 3)
            {
                case 1:
                    a = wd;
                    b = xd;
                    c = yd;
                    break;
                case 2:
                    a = zd;
                    b = wd;
                    c = xd;
                    break;
                case 3:
                    a = yd;
                    b = zd;
                    c = wd;
                    break;
            }
            return (float)(((hash & 4) == 0 ? -a : a) + ((hash & 2) == 0 ? -b : b) + ((hash & 1) == 0 ? -c : c));
        }

        #endregion

        #region 1D

        /// <inheritdoc />
        public float Noise1D(int x)
        {
            switch (_noiseFractalType)
            {
                case NoiseFractalType.BrownianMotion:
                    return SingleFractalBrownianMotion(x * _frequency);
                case NoiseFractalType.Billow:
                    return SingleFractalBillow(x * _frequency);
                case NoiseFractalType.RigidMulti:
                    return SingleFractalRigidMulti(x * _frequency);
                default:
                    return Single(_seed, x * _frequency);
            }
        }

        /// <summary>
        ///     Single fractal brownian motion.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBrownianMotion(double x)
        {
            int   seed = _seed;
            float sum  = Single(seed, x);
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum += Single(++seed, x) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal billow.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBillow(double x)
        {
            int   seed = _seed;
            float sum  = (Math.Abs(Single(seed, x)) * 2) - 1;
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum += ((Math.Abs(Single(++seed, x)) * 2) - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal rigid multi.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalRigidMulti(double x)
        {
            int   seed = _seed;
            float sum  = 1 - Math.Abs(Single(seed, x));
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum -= (1 - Math.Abs(Single(++seed, x))) * amp;
            }

            return sum;
        }

        /// <summary>
        ///     Singles.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        protected abstract float Single(int seed, double x);

        #endregion

        #region 2D

        /// <inheritdoc />
        public float Noise2D(int x, int y)
        {
            switch (_noiseFractalType)
            {
                case NoiseFractalType.BrownianMotion:
                    return SingleFractalBrownianMotion(x * _frequency, y * _frequency);
                case NoiseFractalType.Billow:
                    return SingleFractalBillow(x * _frequency, y * _frequency);
                case NoiseFractalType.RigidMulti:
                    return SingleFractalRigidMulti(x * _frequency, y * _frequency);
                default:
                    return Single(_seed, x * _frequency, y * _frequency);
            }
        }

        /// <summary>
        ///     Single fractal brownian motion.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBrownianMotion(double x, double y)
        {
            int   seed = _seed;
            float sum  = Single(seed, x, y);
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum += Single(++seed, x, y) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal billow.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBillow(double x, double y)
        {
            int   seed = _seed;
            float sum  = (Math.Abs(Single(seed, x, y)) * 2) - 1;
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum += ((Math.Abs(Single(++seed, x, y)) * 2) - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal rigid multi.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalRigidMulti(double x, double y)
        {
            int   seed = _seed;
            float sum  = 1 - Math.Abs(Single(seed, x, y));
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum -= (1 - Math.Abs(Single(++seed, x, y))) * amp;
            }

            return sum;
        }

        /// <summary>
        ///     Singles.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        protected abstract float Single(int seed, double x, double y);

        #endregion

        #region 3D

        /// <inheritdoc />
        public float Noise3D(int x, int y, int z)
        {
            switch (_noiseFractalType)
            {
                case NoiseFractalType.BrownianMotion:
                    return SingleFractalBrownianMotion(x * _frequency, y * _frequency, z * _frequency);
                case NoiseFractalType.Billow:
                    return SingleFractalBillow(x * _frequency, y * _frequency, z * _frequency);
                case NoiseFractalType.RigidMulti:
                    return SingleFractalRigidMulti(x * _frequency, y * _frequency, z * _frequency);
                default:
                    return Single(_seed, x * _frequency, y * _frequency, z * _frequency);
            }
        }

        /// <summary>
        ///     Single fractal brownian motion.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <param name="z"> The z coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBrownianMotion(double x, double y, double z)
        {
            int   seed = _seed;
            float sum  = Single(seed, x, y, z);
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;
                z *= _lacunarity;

                amp *= _gain;
                sum += Single(++seed, x, y, z) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal billow.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <param name="z"> The z coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalBillow(double x, double y, double z)
        {
            int   seed = _seed;
            float sum  = (Math.Abs(Single(seed, x, y, z)) * 2) - 1;
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;
                z *= _lacunarity;

                amp *= _gain;
                sum += ((Math.Abs(Single(++seed, x, y, z)) * 2) - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        /// <summary>
        ///     Single fractal rigid multi.
        /// </summary>
        /// <param name="x"> The x coordinate. </param>
        /// <param name="y"> The y coordinate. </param>
        /// <param name="z"> The z coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        private float SingleFractalRigidMulti(double x, double y, double z)
        {
            int   seed = _seed;
            float sum  = 1 - Math.Abs(Single(seed, x, y, z));
            float amp  = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;
                z *= _lacunarity;

                amp *= _gain;
                sum -= (1 - Math.Abs(Single(++seed, x, y, z))) * amp;
            }

            return sum;
        }

        /// <summary>
        ///     Singles.
        /// </summary>
        /// <param name="seed"> The seed. </param>
        /// <param name="x">    The x coordinate. </param>
        /// <param name="y">    The y coordinate. </param>
        /// <param name="z">    The z coordinate. </param>
        /// <returns>
        ///     A float.
        /// </returns>
        protected abstract float Single(int seed, double x, double y, double z);

        #endregion
    }
}
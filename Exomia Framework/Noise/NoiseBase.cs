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

#pragma warning disable 1591

using System;
using System.Runtime.CompilerServices;
using SharpDX;

namespace Exomia.Framework.Noise
{
    public abstract class NoiseBase : INoise
    {
        protected const int X_PRIME = 1103;
        protected const int Y_PRIME = 29401;
        protected const int Z_PRIME = 6833;
        protected const int W_PRIME = 1259;

        protected const int U_PRIME = 58193;

        protected static readonly float[] Grad_1D = { -1.0f, 1.0f };

        protected static readonly Vector2[] Grad_2D =
        {
            new Vector2(-1.0f, -1.0f),
            new Vector2(1.0f, -1.0f),
            new Vector2(-1.0f, 1.0f),
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, -1.0f),
            new Vector2(-1.0f, 0.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 0.0f)
        };

        protected static readonly Vector3[] Grad_3D =
        {
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, -1.0f, 0.0f),
            new Vector3(-1.0f, -1.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(-1.0f, 0.0f, 1.0f),
            new Vector3(1.0f, 0.0f, -1.0f),
            new Vector3(-1.0f, 0.0f, -1.0f),
            new Vector3(0.0f, 1.0f, 1.0f),
            new Vector3(0.0f, -1.0f, 1.0f),
            new Vector3(0.0f, 1.0f, -1.0f),
            new Vector3(0.0f, -1.0f, -1.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, -1.0f, 1.0f),
            new Vector3(-1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, -1.0f, -1.0f)
        };

        protected float _fractalBounding;
        protected double _frequency;

        protected float _gain;

        protected double _lacunarity;
        protected NoiseFractalType _noiseFractalType;

        protected NoiseInterpolationType _noiseInterpolationType;
        protected int _octaves;

        protected int _seed;

        protected NoiseBase(int seed, float frequency)
            : this(seed, frequency, 0, 0f, 0f, NoiseInterpolationType.Linear, NoiseFractalType.None) { }

        protected NoiseBase(int seed, float frequency, int octaves, NoiseInterpolationType noiseInterpolationType,
            NoiseFractalType noiseFractalType)
            : this(seed, frequency, octaves, 2.0f, 0.5f, noiseInterpolationType, noiseFractalType) { }

        protected NoiseBase(int seed, float frequency, int octaves, float lacunarity,
            NoiseInterpolationType noiseInterpolationType, NoiseFractalType noiseFractalType)
            : this(seed, frequency, octaves, lacunarity, 0.5f, noiseInterpolationType, noiseFractalType) { }

        protected NoiseBase(int seed, float frequency, int octaves, float lacunarity, float gain,
            NoiseInterpolationType noiseInterpolationType, NoiseFractalType noiseFractalType)
        {
            _seed = seed;
            _frequency = frequency;
            _octaves = octaves;
            _lacunarity = lacunarity;
            _gain = gain;
            _noiseInterpolationType = noiseInterpolationType;
            _noiseFractalType = noiseFractalType;

            CalculateFractalBounding();
        }

        /// <inheritdoc />
        public float[] GenerateNoise1D(int x, int xmax)
        {
            float[] noise = new float[xmax];
            for (int dx = 0; dx < xmax; dx++)
            {
                noise[dx] = Noise1D(dx + x);
            }
            return noise;
        }

        /// <inheritdoc />
        public float[,] GenerateNoise2D(int x, int y, int xmax, int ymax)
        {
            float[,] noise = new float[xmax, ymax];
            for (int dy = 0; dy < ymax; dy++)
            {
                for (int dx = 0; dx < xmax; dx++)
                {
                    noise[dx, dy] = Noise2D(dx + x, dy + y);
                }
            }
            return noise;
        }

        /// <inheritdoc />
        public float[,,] GenerateNoise3D(int x, int y, int z, int xmax, int ymax, int zmax)
        {
            float[,,] noise = new float[xmax, ymax, zmax];
            for (int dz = 0; dz < zmax; dz++)
            {
                for (int dy = 0; dy < ymax; dy++)
                {
                    for (int dx = 0; dx < xmax; dx++)
                    {
                        noise[dx, dy, dz] = Noise3D(dx + x, dy + y, dz + z);
                    }
                }
            }
            return noise;
        }

        private void CalculateFractalBounding()
        {
            float amp = _gain;
            float ampFractal = 1.0f;
            for (int i = 1; i < _octaves; i++)
            {
                ampFractal += amp;
                amp *= _gain;
            }
            _fractalBounding = 1.0f / ampFractal;
        }

        #region Hasing

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static int Hash1D(int seed, int x)
        {
            int hash = seed;
            hash ^= X_PRIME * x;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            return hash;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord1D(int seed, int x)
        {
            int n = seed;
            n ^= X_PRIME * x;
            return n * n * n * U_PRIME / 2147483648.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord2D(int seed, int x, int y)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;

            return n * n * n * U_PRIME / 2147483648.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord3D(int seed, int x, int y, int z)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;
            n ^= Z_PRIME * z;

            return n * n * n * U_PRIME / 2147483648.0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float ValueCoord4D(int seed, int x, int y, int z, int w)
        {
            int n = seed;
            n ^= X_PRIME * x;
            n ^= Y_PRIME * y;
            n ^= Z_PRIME * z;
            n ^= W_PRIME * w;

            return n * n * n * U_PRIME / 2147483648.0f;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord2D(int seed, int x, int y, double xd, double yd)
        {
            int hash = seed;
            hash ^= X_PRIME * x;
            hash ^= Y_PRIME * y;

            hash = hash * hash * hash * U_PRIME;
            hash = (hash >> 13) ^ hash;

            Vector2 g = Grad_2D[hash & 7];

            return (float)(xd * g.X + yd * g.Y);
        }

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

            return (float)(xd * g.X + yd * g.Y + zd * g.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static float GradCoord4D(int seed, int x, int y, int z, int w, double xd, double yd, double zd,
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

        private float SingleFractalBrownianMotion(double x)
        {
            int seed = _seed;
            float sum = Single(seed, x);
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum += Single(++seed, x) * amp;
            }

            return sum * _fractalBounding;
        }

        private float SingleFractalBillow(double x)
        {
            int seed = _seed;
            float sum = Math.Abs(Single(seed, x)) * 2 - 1;
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum += (Math.Abs(Single(++seed, x)) * 2 - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        private float SingleFractalRigidMulti(double x)
        {
            int seed = _seed;
            float sum = 1 - Math.Abs(Single(seed, x));
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;

                amp *= _gain;
                sum -= (1 - Math.Abs(Single(++seed, x))) * amp;
            }

            return sum;
        }

        protected abstract float Single(int seed, double x);

        #endregion

        #region 2D

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

        private float SingleFractalBrownianMotion(double x, double y)
        {
            int seed = _seed;
            float sum = Single(seed, x, y);
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum += Single(++seed, x, y) * amp;
            }

            return sum * _fractalBounding;
        }

        private float SingleFractalBillow(double x, double y)
        {
            int seed = _seed;
            float sum = Math.Abs(Single(seed, x, y)) * 2 - 1;
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum += (Math.Abs(Single(++seed, x, y)) * 2 - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        private float SingleFractalRigidMulti(double x, double y)
        {
            int seed = _seed;
            float sum = 1 - Math.Abs(Single(seed, x, y));
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;

                amp *= _gain;
                sum -= (1 - Math.Abs(Single(++seed, x, y))) * amp;
            }

            return sum;
        }

        protected abstract float Single(int seed, double x, double y);

        #endregion

        #region 3D

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

        private float SingleFractalBrownianMotion(double x, double y, double z)
        {
            int seed = _seed;
            float sum = Single(seed, x, y, z);
            float amp = 1;

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

        private float SingleFractalBillow(double x, double y, double z)
        {
            int seed = _seed;
            float sum = Math.Abs(Single(seed, x, y, z)) * 2 - 1;
            float amp = 1;

            for (int i = 1; i < _octaves; i++)
            {
                x *= _lacunarity;
                y *= _lacunarity;
                z *= _lacunarity;

                amp *= _gain;
                sum += (Math.Abs(Single(++seed, x, y, z)) * 2 - 1) * amp;
            }

            return sum * _fractalBounding;
        }

        private float SingleFractalRigidMulti(double x, double y, double z)
        {
            int seed = _seed;
            float sum = 1 - Math.Abs(Single(seed, x, y, z));
            float amp = 1;

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

        protected abstract float Single(int seed, double x, double y, double z);

        #endregion
    }
}
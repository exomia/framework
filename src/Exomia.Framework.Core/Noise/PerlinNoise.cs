#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Noise
{
    /// <summary>
    ///     A perlin noise. This class cannot be inherited.
    /// </summary>
    public sealed class PerlinNoise : NoiseBase
    {
        /// <inheritdoc />
        public PerlinNoise(int                    seed,
                           float                  frequency,
                           int                    octaves,
                           NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
                           NoiseFractalType       noiseFractalType       = NoiseFractalType.BrownianMotion)
            : base(
                seed, frequency, octaves,
                noiseInterpolationType, noiseFractalType) { }

        /// <inheritdoc />
        public PerlinNoise(int                    seed,
                           float                  frequency,
                           int                    octaves,
                           float                  lacunarity,
                           float                  gain,
                           NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
                           NoiseFractalType       noiseFractalType       = NoiseFractalType.BrownianMotion)
            : base(
                seed, frequency, octaves, lacunarity, gain,
                noiseInterpolationType, noiseFractalType) { }

        /// <inheritdoc />
        protected override float Single(int seed, double x)
        {
            int x0 = Math2.Floor(x);

            double xd0 = x - x0;
            double xs;
            switch (_noiseInterpolationType)
            {
                default:
                case NoiseInterpolationType.Linear:
                    xs = xd0;
                    break;
                case NoiseInterpolationType.Hermite:
                    xs = Math2.CurveHermite(xd0);
                    break;
                case NoiseInterpolationType.Quintic:
                    xs = Math2.CurveQuintic(xd0);
                    break;
            }

            return Math2.Lerp(
                GradCoord1D(seed, x0,     xd0),
                GradCoord1D(seed, x0 + 1, xd0 - 1.0),
                xs);
        }

        /// <inheritdoc />
        protected override float Single(int seed, double x, double y)
        {
            int x0 = Math2.Floor(x);
            int y0 = Math2.Floor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double xd0 = x - x0;
            double yd0 = y - y0;
            double xs, ys;
            switch (_noiseInterpolationType)
            {
                default:
                case NoiseInterpolationType.Linear:
                    xs = xd0;
                    ys = yd0;
                    break;
                case NoiseInterpolationType.Hermite:
                    xs = Math2.CurveHermite(xd0);
                    ys = Math2.CurveHermite(yd0);
                    break;
                case NoiseInterpolationType.Quintic:
                    xs = Math2.CurveQuintic(xd0);
                    ys = Math2.CurveQuintic(yd0);
                    break;
            }

            double xd1 = xd0 - 1.0f;
            double yd1 = yd0 - 1.0f;

            return Math2.Lerp(
                Math2.Lerp(
                    GradCoord2D(seed, x0, y0, xd0, yd0),
                    GradCoord2D(seed, x1, y0, xd1, yd0),
                    xs),
                Math2.Lerp(
                    GradCoord2D(seed, x0, y1, xd0, yd1),
                    GradCoord2D(seed, x1, y1, xd1, yd1),
                    xs),
                ys);
        }

        /// <inheritdoc />
        protected override float Single(int seed, double x, double y, double z)
        {
            int x0 = Math2.Floor(x);
            int y0 = Math2.Floor(y);
            int z0 = Math2.Floor(z);
            int x1 = x0 + 1;
            int y1 = y0 + 1;
            int z1 = z0 + 1;

            double xd0 = x - x0;
            double yd0 = y - y0;
            double zd0 = z - z0;
            double xs, ys, zs;
            switch (_noiseInterpolationType)
            {
                default:
                case NoiseInterpolationType.Linear:
                    xs = x - x0;
                    ys = y - y0;
                    zs = z - z0;
                    break;
                case NoiseInterpolationType.Hermite:
                    xs = Math2.CurveHermite(x - x0);
                    ys = Math2.CurveHermite(y - y0);
                    zs = Math2.CurveHermite(z - z0);
                    break;
                case NoiseInterpolationType.Quintic:
                    xs = Math2.CurveQuintic(x - x0);
                    ys = Math2.CurveQuintic(y - y0);
                    zs = Math2.CurveQuintic(z - z0);
                    break;
            }

            double xd1 = xd0 - 1.0f;
            double yd1 = yd0 - 1.0f;
            double zd1 = zd0 - 1.0f;

            return Math2.Lerp(
                Math2.Lerp(
                    Math2.Lerp(
                        GradCoord3D(seed, x0, y0, z0, xd0, yd0, zd0),
                        GradCoord3D(seed, x1, y0, z0, xd1, yd0, zd0),
                        xs),
                    Math2.Lerp(
                        GradCoord3D(seed, x0, y1, z0, xd0, yd1, zd0),
                        GradCoord3D(seed, x1, y1, z0, xd1, yd1, zd0),
                        xs),
                    ys),
                Math2.Lerp(
                    Math2.Lerp(
                        GradCoord3D(seed, x0, y0, z1, xd0, yd0, zd1),
                        GradCoord3D(seed, x1, y0, z1, xd1, yd0, zd1),
                        xs),
                    Math2.Lerp(
                        GradCoord3D(seed, x0, y1, z1, xd0, yd1, zd1),
                        GradCoord3D(seed, x1, y1, z1, xd1, yd1, zd1),
                        xs),
                    ys),
                zs);
        }
    }
}
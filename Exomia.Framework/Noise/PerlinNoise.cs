﻿#region MIT License

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

using Exomia.Framework.Mathematics;

namespace Exomia.Framework.Noise
{
    public class PerlinNoise : NoiseBase
    {
        public PerlinNoise(int seed, float frequency, int octaves,
            NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
            NoiseFractalType noiseFractalType = NoiseFractalType.BrownianMotion)
            : base(seed, frequency, octaves, noiseInterpolationType, noiseFractalType) { }

        public PerlinNoise(int seed, float frequency, int octaves, float lacunarity, float gain,
            NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
            NoiseFractalType noiseFractalType = NoiseFractalType.BrownianMotion)
            : base(seed, frequency, octaves, lacunarity, gain, noiseInterpolationType, noiseFractalType) { }

        protected override float Single(int seed, double x)
        {
            int x0 = Math2.Floor(x);
            int x1 = x0 + 1;

            double xs;
            switch (_noiseInterpolationType)
            {
                default:
                case NoiseInterpolationType.Linear:
                    xs = x - x0;
                    break;
                case NoiseInterpolationType.Hermite:
                    xs = Math2.CurveHermite(x - x0);
                    break;
                case NoiseInterpolationType.Quintic:
                    xs = Math2.CurveQuintic(x - x0);
                    break;
            }

            double xd0 = x - x0;
            double xd1 = xd0 - 1.0;

            return Math2.Lerp(GradCoord1D(seed, x0, xd0), GradCoord1D(seed, x1, xd1), xs);
        }

        protected override float Single(int seed, double x, double y)
        {
            int x0 = Math2.Floor(x);
            int y0 = Math2.Floor(y);
            int x1 = x0 + 1;
            int y1 = y0 + 1;

            double xs, ys;
            switch (_noiseInterpolationType)
            {
                default:
                case NoiseInterpolationType.Linear:
                    xs = x - x0;
                    ys = y - y0;
                    break;
                case NoiseInterpolationType.Hermite:
                    xs = Math2.CurveHermite(x - x0);
                    ys = Math2.CurveHermite(y - y0);
                    break;
                case NoiseInterpolationType.Quintic:
                    xs = Math2.CurveQuintic(x - x0);
                    ys = Math2.CurveQuintic(y - y0);
                    break;
            }

            double xd0 = x - x0;
            double yd0 = y - y0;
            double xd1 = xd0 - 1.0f;
            double yd1 = yd0 - 1.0f;

            float xf0 = Math2.Lerp(GradCoord2D(seed, x0, y0, xd0, yd0), GradCoord2D(seed, x1, y0, xd1, yd0), xs);
            float xf1 = Math2.Lerp(GradCoord2D(seed, x0, y1, xd0, yd1), GradCoord2D(seed, x1, y1, xd1, yd1), xs);

            return Math2.Lerp(xf0, xf1, ys);
        }

        protected override float Single(int seed, double x, double y, double z)
        {
            int x0 = Math2.Floor(x);
            int y0 = Math2.Floor(y);
            int z0 = Math2.Floor(z);
            int x1 = x0 + 1;
            int y1 = y0 + 1;
            int z1 = z0 + 1;

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

            double xd0 = x - x0;
            double yd0 = y - y0;
            double zd0 = z - z0;
            double xd1 = xd0 - 1.0f;
            double yd1 = yd0 - 1.0f;
            double zd1 = zd0 - 1.0f;

            float xf00 = Math2.Lerp(
                GradCoord3D(seed, x0, y0, z0, xd0, yd0, zd0), GradCoord3D(seed, x1, y0, z0, xd1, yd0, zd0), xs);
            float xf10 = Math2.Lerp(
                GradCoord3D(seed, x0, y1, z0, xd0, yd1, zd0), GradCoord3D(seed, x1, y1, z0, xd1, yd1, zd0), xs);
            float xf01 = Math2.Lerp(
                GradCoord3D(seed, x0, y0, z1, xd0, yd0, zd1), GradCoord3D(seed, x1, y0, z1, xd1, yd0, zd1), xs);
            float xf11 = Math2.Lerp(
                GradCoord3D(seed, x0, y1, z1, xd0, yd1, zd1), GradCoord3D(seed, x1, y1, z1, xd1, yd1, zd1), xs);

            float yf0 = Math2.Lerp(xf00, xf10, ys);
            float yf1 = Math2.Lerp(xf01, xf11, ys);

            return Math2.Lerp(yf0, yf1, zs);
        }
    }
}
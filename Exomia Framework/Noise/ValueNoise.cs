#pragma warning disable 1591

using Exomia.Framework.Mathematics;

namespace Exomia.Framework.Noise
{
    public class ValueNoise : NoiseBase
    {
        public ValueNoise(int seed, float frequency, int octaves,
            NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
            NoiseFractalType noiseFractalType = NoiseFractalType.BrownianMotion)
            : base(seed, frequency, octaves, noiseInterpolationType, noiseFractalType) { }

        public ValueNoise(int seed, float frequency, int octaves, float lacunarity, float gain,
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

            return Math2.Lerp(ValueCoord1D(seed, x0), ValueCoord1D(seed, x1), xs);
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

            float xf0 = Math2.Lerp(ValueCoord2D(seed, x0, y0), ValueCoord2D(seed, x1, y0), xs);
            float xf1 = Math2.Lerp(ValueCoord2D(seed, x0, y1), ValueCoord2D(seed, x1, y1), xs);

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

            float xf00 = Math2.Lerp(ValueCoord3D(seed, x0, y0, z0), ValueCoord3D(seed, x1, y0, z0), xs);
            float xf10 = Math2.Lerp(ValueCoord3D(seed, x0, y1, z0), ValueCoord3D(seed, x1, y1, z0), xs);
            float xf01 = Math2.Lerp(ValueCoord3D(seed, x0, y0, z1), ValueCoord3D(seed, x1, y0, z1), xs);
            float xf11 = Math2.Lerp(ValueCoord3D(seed, x0, y1, z1), ValueCoord3D(seed, x1, y1, z1), xs);

            float yf0 = Math2.Lerp(xf00, xf10, ys);
            float yf1 = Math2.Lerp(xf01, xf11, ys);

            return Math2.Lerp(yf0, yf1, zs);
        }
    }
}
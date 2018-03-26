#pragma warning disable 1591

using Exomia.Framework.Mathematics;

namespace Exomia.Framework.Noise
{
    public class SimplexNoise : NoiseBase
    {
        private const float F2 = 1.0f / 2.0f;
        private const float G2 = 1.0f / 4.0f;

        private const float F3 = 1.0f / 3.0f;
        private const float G3 = 1.0f / 6.0f;
        private const float G33 = G3 * 3f - 1f;

        public SimplexNoise(int seed, float frequency, int octaves,
            NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
            NoiseFractalType noiseFractalType = NoiseFractalType.BrownianMotion)
            : base(seed, frequency, octaves, noiseInterpolationType, noiseFractalType) { }

        public SimplexNoise(int seed, float frequency, int octaves, float lacunarity, float gain,
            NoiseInterpolationType noiseInterpolationType = NoiseInterpolationType.Linear,
            NoiseFractalType noiseFractalType = NoiseFractalType.BrownianMotion)
            : base(seed, frequency, octaves, lacunarity, gain, noiseInterpolationType, noiseFractalType) { }

        //TODO: TEST!
        protected override float Single(int seed, double x)
        {
            int i0 = Math2.Floor(x);
            int i1 = i0 + 1;
            double x0 = x - i0;
            double x1 = x0 - 1.0f;

            double n0, n1;

            double t0 = 1.0f - x0 * x0;
            t0 *= t0;
            n0 = t0 * t0 * GradCoord1D(seed, i0, x0);

            double t1 = 1.0f - x1 * x1;
            t1 *= t1;
            n1 = t1 * t1 * GradCoord1D(seed, i1, x1);

            return (float)(0.395 * (n0 + n1));
        }

        protected override float Single(int seed, double x, double y)
        {
            double t = (x + y) * F2;
            int i = Math2.Floor(x + t);
            int j = Math2.Floor(y + t);

            t = (i + j) * G2;

            double x0 = x - (i - t);
            double y0 = y - (j - t);

            int i1 = 0, j1 = 1;
            if (x0 > y0)
            {
                i1 = 1;
                j1 = 0;
            }

            double x1 = x0 - i1 + G2;
            double y1 = y0 - j1 + G2;
            double x2 = x0 - 1 + F2;
            double y2 = y0 - 1 + F2;

            double n0, n1, n2;

            t = 0.5f - x0 * x0 - y0 * y0;
            if (t < 0) { n0 = 0; }
            else
            {
                t *= t;
                n0 = t * t * GradCoord2D(seed, i, j, x0, y0);
            }

            t = 0.5f - x1 * x1 - y1 * y1;
            if (t < 0) { n1 = 0; }
            else
            {
                t *= t;
                n1 = t * t * GradCoord2D(seed, i + i1, j + j1, x1, y1);
            }

            t = 0.5f - x2 * x2 - y2 * y2;
            if (t < 0) { n2 = 0; }
            else
            {
                t *= t;
                n2 = t * t * GradCoord2D(seed, i + 1, j + 1, x2, y2);
            }

            return (float)(50.0 * (n0 + n1 + n2));
        }

        protected override float Single(int seed, double x, double y, double z)
        {
            double t = (x + y + z) * F3;
            int i = Math2.Floor(x + t);
            int j = Math2.Floor(y + t);
            int k = Math2.Floor(z + t);

            t = (i + j + k) * G3;
            double x0 = x - (i - t);
            double y0 = y - (j - t);
            double z0 = z - (k - t);

            int i1, j1, k1;
            int i2, j2, k2;

            if (x0 >= y0)
            {
                if (y0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
                else if (x0 >= z0)
                {
                    i1 = 1;
                    j1 = 0;
                    k1 = 0;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 1;
                    j2 = 0;
                    k2 = 1;
                }
            }
            else
            {
                if (y0 < z0)
                {
                    i1 = 0;
                    j1 = 0;
                    k1 = 1;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else if (x0 < z0)
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 0;
                    j2 = 1;
                    k2 = 1;
                }
                else
                {
                    i1 = 0;
                    j1 = 1;
                    k1 = 0;
                    i2 = 1;
                    j2 = 1;
                    k2 = 0;
                }
            }

            double x1 = x0 - i1 + G3;
            double y1 = y0 - j1 + G3;
            double z1 = z0 - k1 + G3;
            double x2 = x0 - i2 + F3;
            double y2 = y0 - j2 + F3;
            double z2 = z0 - k2 + F3;
            double x3 = x0 + G33;
            double y3 = y0 + G33;
            double z3 = z0 + G33;

            double n0, n1, n2, n3;

            t = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t < 0) { n0 = 0; }
            else
            {
                t *= t;
                n0 = t * t * GradCoord3D(seed, i, j, k, x0, y0, z0);
            }

            t = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t < 0) { n1 = 0; }
            else
            {
                t *= t;
                n1 = t * t * GradCoord3D(seed, i + i1, j + j1, k + k1, x1, y1, z1);
            }

            t = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t < 0) { n2 = 0; }
            else
            {
                t *= t;
                n2 = t * t * GradCoord3D(seed, i + i2, j + j2, k + k2, x2, y2, z2);
            }

            t = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t < 0) { n3 = 0; }
            else
            {
                t *= t;
                n3 = t * t * GradCoord3D(seed, i + 1, j + 1, k + 1, x3, y3, z3);
            }

            return (float)(32.0 * (n0 + n1 + n2 + n3));
        }
    }
}
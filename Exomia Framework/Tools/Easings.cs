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

namespace Exomia.Framework.Tools
{
    /// <summary>
    ///     EasingFunction delegate
    /// </summary>
    /// <param name="t">time (0 - duration)</param>
    /// <param name="b">initial value</param>
    /// <param name="c">change in value or difference between final and initial value</param>
    /// <param name="d">duration</param>
    /// <returns>float value</returns>
    public delegate float EasingFunction(float t, float b, float c, float d);

    public static class Easing
    {
        public static class Bounce
        {
            private const float B1 = 1.0f / 2.75f;
            private const float B2 = 1.5f / 2.75f;
            private const float B3 = 2.0f / 2.75f;
            private const float B4 = 2.25f / 2.75f;
            private const float B5 = 2.5f / 2.75f;
            private const float B6 = 2.625f / 2.75f;

            public static float EaseIn(float t, float b, float c, float d)
            {
                return c - EaseOut(d - t, 0, c, d) + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if (t < d / 2) { return EaseIn(t * 2, 0, c, d) * .5f + b; }
                return EaseOut(t * 2 - d, 0, c, d) * .5f + c * .5f + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                if ((t /= d) < B1)
                {
                    return c * (7.5625f * t * t) + b;
                }
                if (t < B3)
                {
                    return c * (7.5625f * (t -= B2) * t + .75f) + b;
                }
                if (t < B5)
                {
                    return c * (7.5625f * (t -= B4) * t + .9375f) + b;
                }
                return c * (7.5625f * (t -= B6) * t + .984375f) + b;
            }
        }

        public static class Circle
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return -c * ((float)Math.Sqrt(1 - (t /= d) * t) - 1) + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if ((t /= d / 2) < 1) { return -c / 2 * ((float)Math.Sqrt(1 - t * t) - 1) + b; }
                return c / 2 * ((float)Math.Sqrt(1 - (t -= 2) * t) + 1) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return c * (float)Math.Sqrt(1 - (t = t / d - 1) * t) + b;
            }
        }

        public static class Cubic
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return c * (t /= d) * t * t + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if ((t /= d / 2) < 1) { return c / 2 * t * t * t + b; }
                return c / 2 * ((t -= 2) * t * t + 2) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return c * ((t = t / d - 1) * t * t + 1) + b;
            }
        }

        public static class Expo
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return t == 0 ? b : c * (float)Math.Pow(2, 10 * (t / d - 1)) + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if (t == 0) { return b; }
                if (t == d) { return b + c; }
                if ((t /= d / 2) < 1) { return c / 2 * (float)Math.Pow(2, 10 * (t - 1)) + b; }
                return c / 2 * (-(float)Math.Pow(2, -10 * --t) + 2) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return t == d ? b + c : c * (-(float)Math.Pow(2, -10 * t / d) + 1) + b;
            }
        }

        public static class Linear
        {
            public static float EaseNone(float t, float b, float c, float d)
            {
                return c * t / d + b;
            }
        }

        public static class Quad
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return c * (t /= d) * t + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if ((t /= d / 2) < 1) { return c / 2 * t * t + b; }
                return -c / 2 * (--t * (t - 2) - 1) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return -c * (t /= d) * (t - 2) + b;
            }
        }

        public static class Quart
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return c * (t /= d) * t * t * t + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if ((t /= d / 2) < 1) { return c / 2 * t * t * t * t + b; }
                return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return -c * ((t = t / d - 1) * t * t * t - 1) + b;
            }
        }

        public static class Quint
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return c * (t /= d) * t * t * t * t + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                if ((t /= d / 2) < 1) { return c / 2 * t * t * t * t * t + b; }
                return c / 2 * ((t -= 2) * t * t * t * t + 2) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return c * ((t = t / d - 1) * t * t * t * t + 1) + b;
            }
        }

        public static class Sine
        {
            public static float EaseIn(float t, float b, float c, float d)
            {
                return -c * (float)Math.Cos(t / d * (Math.PI / 2)) + c + b;
            }

            public static float EaseInOut(float t, float b, float c, float d)
            {
                return -c / 2 * ((float)Math.Cos(Math.PI * t / d) - 1) + b;
            }

            public static float EaseOut(float t, float b, float c, float d)
            {
                return c * (float)Math.Sin(t / d * (Math.PI / 2)) + b;
            }
        }
    }
}
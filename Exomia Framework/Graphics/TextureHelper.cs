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

using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Resource = SharpDX.Direct3D11.Resource;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     TextureHelper class
    /// </summary>
    public static class TextureHelper
    {
        private static readonly ImagingFactory s_imgfactory = new ImagingFactory();

        /// <summary>
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static BitmapSource LoadBitmap(Stream stream)
        {
            BitmapDecoder bitmapDecoder = new BitmapDecoder(s_imgfactory, stream, DecodeOptions.CacheOnDemand);
            FormatConverter formatConverter = new FormatConverter(s_imgfactory);
            formatConverter.Initialize(
                bitmapDecoder.GetFrame(0),
                PixelFormat.Format32bppPRGBA,
                BitmapDitherType.None,
                null,
                0.0f,
                BitmapPaletteType.Custom);
            return formatConverter;
        }

        /// <summary>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Texture2D LoadTexture2D(Device5 device, Stream stream)
        {
            lock (device)
            {
                using (BitmapSource bitmapSource = LoadBitmap(stream))
                {
                    int stride = bitmapSource.Size.Width * 4;
                    using (DataStream buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
                    {
                        bitmapSource.CopyPixels(stride, buffer);
                        return new Texture2D(
                            device, new Texture2DDescription
                            {
                                Width = bitmapSource.Size.Width,
                                Height = bitmapSource.Size.Height,
                                ArraySize = 1,
                                BindFlags = BindFlags.ShaderResource,
                                Usage = ResourceUsage.Immutable,
                                CpuAccessFlags = CpuAccessFlags.None,
                                Format = Format.R8G8B8A8_UNorm,
                                MipLevels = 1,
                                OptionFlags = ResourceOptionFlags.None,
                                SampleDescription = new SampleDescription(1, 0)
                            }, new DataRectangle(buffer.DataPointer, stride));
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="device"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static Texture2D CreateTexture(Device5 device, int width, int height,
            Format format = Format.B8G8R8A8_UNorm)
        {
            lock (device)
            {
                return new Texture2D(
                    device, new Texture2DDescription
                    {
                        Width = width,
                        Height = height,
                        ArraySize = 1,
                        BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                        Usage = ResourceUsage.Default,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = format,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0)
                    });
            }
        }

        internal static Texture2D ToTexture2DArray(Device5 device, BitmapSource[] bitmapSources)
        {
            int width = bitmapSources[0].Size.Width;
            int height = bitmapSources[0].Size.Height;

            lock (device)
            {
                Texture2D texArray = new Texture2D(
                    device, new Texture2DDescription
                    {
                        Width = width,
                        Height = height,
                        ArraySize = bitmapSources.Length,
                        BindFlags = BindFlags.ShaderResource,
                        Usage = ResourceUsage.Default,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0)
                    });

                int stride = width * 4;
                for (int i = 0; i < bitmapSources.Length; i++)
                {
                    using (BitmapSource bitmap = bitmapSources[i])
                    {
                        using (DataStream buffer = new DataStream(height * stride, true, true))
                        {
                            bitmap.CopyPixels(stride, buffer);
                            DataBox box = new DataBox(buffer.DataPointer, stride, 1);
                            device.ImmediateContext.UpdateSubresource(
                                box, texArray, Resource.CalculateSubResourceIndex(0, i, 1));
                        }
                    }
                }
                return texArray;
            }
        }
    }
}
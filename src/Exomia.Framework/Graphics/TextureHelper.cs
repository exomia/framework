#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
    ///     A texture helper.
    /// </summary>
    public static class TextureHelper
    {
        private static readonly ImagingFactory s_imgFactory = new ImagingFactory();

        /// <summary>
        ///     Creates a texture.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="format"> (Optional) Describes the format to use. </param>
        /// <returns>
        ///     The new texture.
        /// </returns>
        public static Texture2D CreateTexture(Device5 device,
                                              int     width,
                                              int     height,
                                              Format  format = Format.B8G8R8A8_UNorm)
        {
            return new Texture2D(
                device,
                new Texture2DDescription
                {
                    Width             = width,
                    Height            = height,
                    ArraySize         = 1,
                    BindFlags         = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Usage             = ResourceUsage.Default,
                    CpuAccessFlags    = CpuAccessFlags.None,
                    Format            = format,
                    MipLevels         = 1,
                    OptionFlags       = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0)
                });
        }

        /// <summary>
        ///     Loads a bitmap.
        /// </summary>
        /// <param name="stream"> The stream. </param>
        /// <returns>
        ///     The bitmap.
        /// </returns>
        public static BitmapSource LoadBitmap(Stream stream)
        {
            BitmapDecoder   bitmapDecoder   = new BitmapDecoder(s_imgFactory, stream, DecodeOptions.CacheOnDemand);
            FormatConverter formatConverter = new FormatConverter(s_imgFactory);
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
        ///     Loads texture 2 d.
        /// </summary>
        /// <param name="device"> The device. </param>
        /// <param name="stream"> The stream. </param>
        /// <returns>
        ///     The texture 2 d.
        /// </returns>
        public static Texture2D LoadTexture2D(Device5 device, Stream stream)
        {
            using (BitmapSource bitmapSource = LoadBitmap(stream))
            {
                int stride = bitmapSource.Size.Width * 4;
                using (DataStream buffer = new DataStream(bitmapSource.Size.Height * stride, true, true))
                {
                    bitmapSource.CopyPixels(stride, buffer);
                    return new Texture2D(
                        device,
                        new Texture2DDescription
                        {
                            Width             = bitmapSource.Size.Width,
                            Height            = bitmapSource.Size.Height,
                            ArraySize         = 1,
                            BindFlags         = BindFlags.ShaderResource,
                            Usage             = ResourceUsage.Immutable,
                            CpuAccessFlags    = CpuAccessFlags.None,
                            Format            = Format.R8G8B8A8_UNorm,
                            MipLevels         = 1,
                            OptionFlags       = ResourceOptionFlags.None,
                            SampleDescription = new SampleDescription(1, 0)
                        }, new DataRectangle(buffer.DataPointer, stride));
                }
            }
        }

        /// <summary>
        ///     Converts this object to a texture 2 d array.
        /// </summary>
        /// <param name="device">        The device. </param>
        /// <param name="bitmapSources"> The bitmap sources. </param>
        /// <returns>
        ///     The given data converted to a Texture2D.
        /// </returns>
        internal static Texture2D ToTexture2DArray(Device5 device, BitmapSource[] bitmapSources)
        {
            int width  = bitmapSources[0].Size.Width;
            int height = bitmapSources[0].Size.Height;

            Texture2D texArray = new Texture2D(
                device,
                new Texture2DDescription
                {
                    Width             = width,
                    Height            = height,
                    ArraySize         = bitmapSources.Length,
                    BindFlags         = BindFlags.ShaderResource,
                    Usage             = ResourceUsage.Default,
                    CpuAccessFlags    = CpuAccessFlags.None,
                    Format            = Format.R8G8B8A8_UNorm,
                    MipLevels         = 1,
                    OptionFlags       = ResourceOptionFlags.None,
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
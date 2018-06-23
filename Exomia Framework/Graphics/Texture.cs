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

using System;
using System.IO;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <inheritdoc />
    /// <summary>
    ///     Texture class
    /// </summary>
    [ContentReadable(typeof(TextureContentReader))]
    public sealed class Texture : IDisposable
    {
        private ShaderResourceView1 _textureView;

        /// <summary>
        ///     ShaderResourceView1
        /// </summary>
        public ShaderResourceView1 TextureView
        {
            get { return _textureView; }
        }

        /// <summary>
        ///     Width
        /// </summary>
        public int Width { get; }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height { get; }

        /// <summary>
        ///     TextureView1.NativePointer
        /// </summary>
        public IntPtr TexturePointer
        {
            get { return _textureView.NativePointer; }
        }

        /// <summary>
        ///     Texture construcor
        /// </summary>
        /// <param name="textureView">ShaderResourceView1</param>
        /// <param name="width">width</param>
        /// <param name="height">height</param>
        public Texture(ShaderResourceView1 textureView, int width, int height)
        {
            _textureView = textureView;
            Width = width;
            Height = height;
        }

        /// <summary>
        ///     Texture destructor
        /// </summary>
        ~Texture()
        {
            Dispose(false);
        }

        /// <summary>
        ///     load a Texture from a given source stream
        /// </summary>
        /// <param name="device">device</param>
        /// <param name="stream">data stream</param>
        /// <returns>new texture</returns>
        public static Texture Load(Device5 device, Stream stream)
        {
            try
            {
                using (Texture2D texture2D = TextureHelper.LoadTexture2D(device, stream))
                {
                    return new Texture(
                        new ShaderResourceView1(device, texture2D), texture2D.Description.Width,
                        texture2D.Description.Height);
                }
            }
            catch { return null; }
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref _textureView);
                }

                _disposed = true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <inheritdoc />
    /// <summary>
    ///     Texture2 class
    /// </summary>
    [ContentReadable(typeof(Texture2ContentReader))]
    public sealed class Texture2 : IDisposable
    {
        /// <summary>
        ///     AtlasIndex
        /// </summary>
        public int AtlasIndex { get; }

        /// <summary>
        ///     AssetName
        /// </summary>
        public string AssetName { get; }

        /// <summary>
        ///     SourceRectangle
        /// </summary>
        public Rectangle SourceRectangle { get; private set; }

        /// <summary>
        ///     Width
        /// </summary>
        public int Width
        {
            get { return SourceRectangle.Width; }
        }

        /// <summary>
        ///     Height
        /// </summary>
        public int Height
        {
            get { return SourceRectangle.Height; }
        }

        internal Texture2(int atlasIndex, string assetName, Rectangle sourceRectangle)
        {
            AtlasIndex = atlasIndex;
            AssetName = assetName;
            SourceRectangle = sourceRectangle;
        }

        /// <summary>
        ///     Texture2 destructor
        /// </summary>
        ~Texture2()
        {
            Dispose(false);
        }

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    SourceRectangle = Rectangle.Empty;
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
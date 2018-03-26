using System;
using System.IO;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     Texture class
    /// </summary>
    [ContentReadable(typeof(TextureContentReader))]
    public sealed class Texture : IDisposable
    {
        #region Variables

        #region Statics

        #endregion

        private ShaderResourceView1 _textureView;

        #endregion

        #region Constants

        #endregion

        #region Properties

        #region Statics

        #endregion

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

        #endregion

        #region Constructors

        #region Statics

        #endregion

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

        #endregion

        #region Methods

        #region Statics

        #endregion

        #endregion

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

    /// <summary>
    ///     Texture2 class
    /// </summary>
    [ContentReadable(typeof(Texture2ContentReader))]
    public sealed class Texture2 : IDisposable
    {
        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        #endregion

        #region Properties

        #region Statics

        #endregion

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

        #endregion

        #region Constructors

        #region Statics

        #endregion

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

        #endregion

        #region Methods

        #region Statics

        #endregion

        #endregion

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
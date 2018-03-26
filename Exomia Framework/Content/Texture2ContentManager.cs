#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using Exomia.Framework.Graphics;
using Exomia.Framework.Security;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;

namespace Exomia.Framework.Content
{
    /// <inheritdoc />
    public sealed class Texture2ContentManager : ITexture2ContentManager
    {
        #region Properties

        #region Statics

        #endregion

        /// <summary>
        ///     <see cref="ITexture2ContentManager.IsTextureInvalid" />
        /// </summary>
        public bool IsTextureInvalid { get; private set; }

        #endregion

        #region Constructors

        #region Statics

        #endregion

        ~Texture2ContentManager()
        {
            Dispose(false);
        }

        #endregion

        #region Constants

        private const int INITIAL_QUEUE_SIZE = 8;
        private const int ATLAS_WIDTH = 1024 * 8;
        private const int ATLAS_HEIGHT = 1024 * 8;

        #endregion

        #region Variables

        #region Statics

        #endregion

        private readonly Dictionary<int, SpriteBatchAtlas> _atlases =
            new Dictionary<int, SpriteBatchAtlas>(INITIAL_QUEUE_SIZE);

        private readonly Dictionary<string, Texture2> _atlasesKeys =
            new Dictionary<string, Texture2>(INITIAL_QUEUE_SIZE);

        private int _atlasesIndex;
        private readonly object _lockAtlas = new object();

        private Texture _texture;

        #endregion

        #region Methods

        #region Statics

        #endregion

        private void AddAtlas()
        {
            lock (_lockAtlas)
            {
                _atlases.Add(_atlasesIndex++, new SpriteBatchAtlas(ATLAS_WIDTH, ATLAS_HEIGHT));
            }
        }

        /// <inheritdoc />
        public Texture2 AddTexture(Stream stream, string assetName, int startIndex = 0)
        {
            Texture2 texture = null;
            lock (_lockAtlas)
            {
                if (_atlasesKeys.TryGetValue(assetName, out texture))
                {
                    return texture;
                }
            }

            lock (_lockAtlas)
            {
                for (int i = startIndex; i < _atlasesIndex; ++i)
                {
                    if (_atlases[i].AddTexture(stream, assetName, out Rectangle sourceRect))
                    {
                        texture = new Texture2(i, assetName, sourceRect);
                        _atlasesKeys.Add(assetName, texture);
                        IsTextureInvalid = true;
                        return texture;
                    }
                }
            }
            AddAtlas();

            return AddTexture(stream, assetName, _atlasesIndex - 1);
        }
        /// <inheritdoc />
        public Texture2 AddTexture(string assetName, int startIndex = 0)
        {
            using (FileStream fs = new FileStream(assetName, FileMode.Open, FileAccess.Read))
            {
                return AddTexture(fs, assetName, startIndex);
            }
        }

        /// <inheritdoc />
        public Texture2 AddTexture(Stream stream, out string assetName, int startIndex)
        {
            assetName = DateTime.Now.Ticks.ToMD5();
            return AddTexture(stream, assetName, startIndex);
        }

        /// <inheritdoc />
        public bool GenerateTexture2DArray(Device5 device, out Texture texture)
        {
            if (!IsTextureInvalid)
            {
                texture = _texture;
                return true;
            }

            try
            {
                BitmapSource[] bitmapSources = new BitmapSource[_atlasesIndex];
                lock (_lockAtlas)
                {
                    for (int i = 0; i < _atlasesIndex; i++)
                    {
                        bitmapSources[i] = _atlases[i].GenerateBitmapSource();
                    }
                }

                using (Texture2D tex2D = TextureHelper.ToTexture2DArray(device, bitmapSources))
                {
                    texture = _texture = new Texture(
                        new ShaderResourceView1(device, tex2D), tex2D.Description.Width, tex2D.Description.Height);
                }
                IsTextureInvalid = false;
                return true;
            }
            catch
            {
                texture = null;
                return false;
            }
        }

        /// <inheritdoc />
        public void Reset()
        {
            _atlases.Clear();
            _atlasesKeys.Clear();
            _atlasesIndex = 0;
            IsTextureInvalid = true;
            _texture?.Dispose();
            _texture = null;
        }

        #endregion

        #region IDisposable Support

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    /* USER CODE */
                    Reset();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
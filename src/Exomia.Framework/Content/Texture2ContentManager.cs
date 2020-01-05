#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using Exomia.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;

namespace Exomia.Framework.Content
{
    //TODO: redesign/remove this file
    /// <summary>
    ///     Manager for texture 2 contents. This class cannot be inherited.
    /// </summary>
    public sealed class Texture2ContentManager : ITexture2ContentManager
    {
        /// <summary>
        ///     Initial size of the queue.
        /// </summary>
        private const int INITIAL_QUEUE_SIZE = 8;

        /// <summary>
        ///     Width of the atlas.
        /// </summary>
        private const int ATLAS_WIDTH = 1024 * 8;

        /// <summary>
        ///     Height of the atlas.
        /// </summary>
        private const int ATLAS_HEIGHT = 1024 * 8;

        /// <summary>
        ///     The atlases.
        /// </summary>
        private readonly Dictionary<int, SpriteBatchAtlas> _atlases =
            new Dictionary<int, SpriteBatchAtlas>(INITIAL_QUEUE_SIZE);

        /// <summary>
        ///     The atlases keys.
        /// </summary>
        private readonly Dictionary<string, Texture2> _atlasesKeys =
            new Dictionary<string, Texture2>(INITIAL_QUEUE_SIZE);

        /// <summary>
        ///     The lock atlas.
        /// </summary>
        private readonly object _lockAtlas = new object();

        /// <summary>
        ///     Zero-based index of the atlases.
        /// </summary>
        private int _atlasesIndex;

        /// <summary>
        ///     The texture.
        /// </summary>
        private Texture _texture;

        /// <inheritdoc />
        public bool IsTextureInvalid { get; private set; }

        /// <summary>
        ///     Finalizes an instance of the <see cref="Texture2ContentManager" /> class.
        /// </summary>
        ~Texture2ContentManager()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public Texture2 AddTexture(Stream stream, string assetName, int startIndex = 0)
        {
            lock (_lockAtlas)
            {
                if (_atlasesKeys.TryGetValue(assetName, out Texture2 texture))
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
                        Texture2 texture = new Texture2(i, assetName, sourceRect);
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
            _atlasesIndex    = 0;
            IsTextureInvalid = true;
            _texture?.Dispose();
            _texture = null;
        }

        /// <summary>
        ///     Adds atlas.
        /// </summary>
        private void AddAtlas()
        {
            lock (_lockAtlas)
            {
                _atlases.Add(_atlasesIndex++, new SpriteBatchAtlas(ATLAS_WIDTH, ATLAS_HEIGHT));
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
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

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
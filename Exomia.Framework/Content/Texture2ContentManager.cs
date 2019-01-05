﻿#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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
using System.Collections.Generic;
using System.IO;
using Exomia.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.WIC;

namespace Exomia.Framework.Content
{
    //TODO: redesign/remove this file
    /// <inheritdoc />
    public sealed class Texture2ContentManager : ITexture2ContentManager
    {
        private const int INITIAL_QUEUE_SIZE = 8;
        private const int ATLAS_WIDTH = 1024 * 8;
        private const int ATLAS_HEIGHT = 1024 * 8;

        private readonly Dictionary<int, SpriteBatchAtlas> _atlases =
            new Dictionary<int, SpriteBatchAtlas>(INITIAL_QUEUE_SIZE);

        private readonly Dictionary<string, Texture2> _atlasesKeys =
            new Dictionary<string, Texture2>(INITIAL_QUEUE_SIZE);

        private readonly object _lockAtlas = new object();

        private int _atlasesIndex;

        private Texture _texture;

        /// <inheritdoc />
        public bool IsTextureInvalid { get; private set; }

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

        private void AddAtlas()
        {
            lock (_lockAtlas)
            {
                _atlases.Add(_atlasesIndex++, new SpriteBatchAtlas(ATLAS_WIDTH, ATLAS_HEIGHT));
            }
        }

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
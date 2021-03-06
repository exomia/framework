﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A texture. This class cannot be inherited.
    /// </summary>
    [ContentReadable(typeof(TextureContentReader))]
    public sealed class Texture : IDisposable
    {
        private ShaderResourceView1 _textureView;

        /// <summary>
        ///     Gets the empty.
        /// </summary>
        /// <value>
        ///     The empty.
        /// </value>
        public static Texture Empty { get; } = new Texture(null!, 0, 0);

        /// <summary>
        ///     Height.
        /// </summary>
        /// <value>
        ///     The height.
        /// </value>
        public int Height { get; }

        /// <summary>
        ///     TextureView1.NativePointer.
        /// </summary>
        /// <value>
        ///     The texture pointer.
        /// </value>
        public IntPtr TexturePointer
        {
            get { return _textureView.NativePointer; }
        }

        /// <summary>
        ///     ShaderResourceView1.
        /// </summary>
        /// <value>
        ///     The texture view.
        /// </value>
        public ShaderResourceView1 TextureView
        {
            get { return _textureView; }
        }

        /// <summary>
        ///     Width.
        /// </summary>
        /// <value>
        ///     The width.
        /// </value>
        public int Width { get; }

        /// <summary>
        ///     Texture constructor.
        /// </summary>
        /// <param name="textureView"> ShaderResourceView1. </param>
        /// <param name="width">       width. </param>
        /// <param name="height">      height. </param>
        public Texture(ShaderResourceView1 textureView, int width, int height)
        {
            _textureView = textureView;
            Width        = width;
            Height       = height;
        }

        /// <summary>
        ///     Texture destructor.
        /// </summary>
        ~Texture()
        {
            Dispose(false);
        }

        /// <summary>
        ///     load a Texture from a given source stream.
        /// </summary>
        /// <param name="device"> device. </param>
        /// <param name="stream"> data stream. </param>
        /// <returns>
        ///     new texture.
        /// </returns>
        public static Texture? Load(Device5 device, Stream stream)
        {
            try
            {
                using Texture2D texture2D = TextureHelper.LoadTexture2D(device, stream);
                return new Texture(
                    new ShaderResourceView1(device, texture2D),
                    texture2D.Description.Width, texture2D.Description.Height);
            }
            catch { return null; }
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.Texture and
        ///     optionally releases the managed resources.
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
                    Utilities.Dispose(ref _textureView);
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
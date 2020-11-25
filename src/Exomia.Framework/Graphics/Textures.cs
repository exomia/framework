#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     The built-in textures. This class cannot be inherited.
    /// </summary>
    public class Textures : IDisposable
    {
        private const string WHITE_TEXTURE_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+ip1sAAAAASUVORK5CYII=";

        private readonly Texture _white;

        /// <summary>
        ///     Built-in white texture object with size of 1x1 px.
        /// </summary>
        /// <value>
        ///     The white texture.
        /// </value>
        public Texture White
        {
            get { return _white!; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Textures" /> class.
        /// </summary>
        /// <param name="graphicsDevice"> The graphics device. </param>
        /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
        internal Textures(IGraphicsDevice graphicsDevice)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(WHITE_TEXTURE_BASE64)))
            {
                _white = Texture.Load(graphicsDevice.Device, ms) ??
                         throw new NullReferenceException($"{nameof(White)}");
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
                    _white.Dispose();
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~Textures()
        {
            Dispose(false);
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
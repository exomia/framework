#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     A texture. This class cannot be inherited.
    /// </summary>
    public sealed class Texture : IDisposable
    {
        /// <summary>
        ///     Gets the empty.
        /// </summary>
        /// <value>
        ///     The empty.
        /// </value>
        public static Texture Empty { get; } = new Texture(0, 0);

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
            get { return IntPtr.Zero; }
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
        /// <param name="width">       width. </param>
        /// <param name="height">      height. </param>
        public Texture(int width, int height)
        {
            Width  = width;
            Height = height;
        }

        #region IDisposable Support

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
                if (disposing) { }

                _disposed = true;
            }
        }

        /// <summary>
        ///     Texture destructor.
        /// </summary>
        ~Texture()
        {
            Dispose(false);
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
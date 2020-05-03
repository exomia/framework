#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SharpDX;
using SharpDX.WIC;
using Bitmap = System.Drawing.Bitmap;
using Color = System.Drawing.Color;
using Rectangle = SharpDX.Rectangle;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite batch atlas. This class cannot be inherited.
    /// </summary>
    sealed class SpriteBatchAtlas : IDisposable
    {
        private const int MIN_ATLAS_WIDTH  = 2048;
        private const int MIN_ATLAS_HEIGHT = 2048;
        private const int MAX_ATLAS_WIDTH  = 8192;
        private const int MAX_ATLAS_HEIGHT = 8192;

        private readonly int                           _height;
        private readonly object                        _lockAtlas = new object();
        private readonly Dictionary<string, Rectangle> _sourceRectangles;
        private readonly int                           _width;
        private          Bitmap                        _atlas;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SpriteBatchAtlas" /> class.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        public SpriteBatchAtlas(int width, int height)
        {
            if (width < MIN_ATLAS_WIDTH) { width    = MIN_ATLAS_WIDTH; }
            if (height < MIN_ATLAS_HEIGHT) { height = MIN_ATLAS_HEIGHT; }

            if (width > MAX_ATLAS_WIDTH) { width    = MAX_ATLAS_WIDTH; }
            if (height > MAX_ATLAS_HEIGHT) { height = MAX_ATLAS_HEIGHT; }

            _width  = width;
            _height = height;

            _sourceRectangles = new Dictionary<string, Rectangle>(16);

            _atlas = new Bitmap(width, height);
        }

        /// <summary>
        ///     Adds a texture.
        /// </summary>
        /// <exception cref="OverflowException"> Thrown when an arithmetic overflow occurs. </exception>
        /// <param name="stream">          The stream. </param>
        /// <param name="assetName">       Name of the asset. </param>
        /// <param name="sourceRectangle"> [out] Source rectangle. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal bool AddTexture(Stream stream, string assetName, out Rectangle sourceRectangle)
        {
            sourceRectangle = Rectangle.Empty;

            if (_sourceRectangles.TryGetValue(assetName, out sourceRectangle))
            {
                return true;
            }

            using (Image img = Image.FromStream(stream))
            {
                if (img.Width > _width) { throw new OverflowException("the image size is to big!"); }
                if (img.Height > _height) { throw new OverflowException("the image size is to big!"); }

                if (GetFreeLocation(img.Width, img.Height, out int x, out int y))
                {
                    sourceRectangle = new Rectangle(x, y, img.Width, img.Height);
                    _sourceRectangles.Add(assetName, sourceRectangle);
                    lock (_lockAtlas)
                    {
                        using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_atlas))
                        {
                            g.DrawImage(img, new System.Drawing.Rectangle(x, y, img.Width, img.Height));
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Generates a bitmap source.
        /// </summary>
        /// <returns>
        ///     The bitmap source.
        /// </returns>
        internal BitmapSource GenerateBitmapSource()
        {
            lock (_lockAtlas)
            {
                MemoryStream ms = new MemoryStream();
                _atlas.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                return TextureHelper.LoadBitmap(ms);
            }
        }

        /// <summary>
        ///     Removes the texture described by assetName.
        /// </summary>
        /// <param name="assetName"> Name of the asset. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal bool RemoveTexture(string assetName)
        {
            if (!_sourceRectangles.TryGetValue(assetName, out Rectangle sourceRectangle))
            {
                return true;
            }
            lock (_lockAtlas)
            {
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(_atlas))
                {
                    g.FillRectangle(
                        new SolidBrush(Color.Transparent),
                        new System.Drawing.Rectangle(
                            sourceRectangle.X, sourceRectangle.Y, sourceRectangle.Width, sourceRectangle.Height));
                }
            }

            return _sourceRectangles.Remove(assetName);
        }

        /// <summary>
        ///     Attempts to get source rectangle a Rectangle from the given string.
        /// </summary>
        /// <param name="assetName">       Name of the asset. </param>
        /// <param name="sourceRectangle"> [out] Source rectangle. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal bool TryGetSourceRectangle(string assetName, out Rectangle sourceRectangle)
        {
            return _sourceRectangles.TryGetValue(assetName, out sourceRectangle);
        }

        /// <summary>
        ///     Gets free location.
        /// </summary>
        /// <param name="width">  The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="x">      [out] The out int to process. </param>
        /// <param name="y">      [out] The out int to process. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        private bool GetFreeLocation(int width, int height, out int x, out int y)
        {
            x = 0;
            y = 0;
            for (y = 1; y < _height - 1; y++)
            {
                int ymin = _height - 1;
                for (x = 1; x < _width - 1; x++)
                {
                    if (x + width > _width - 1)
                    {
                        y = ymin;
                        break;
                    }
                    Rectangle sharpSf = new Rectangle(x, y, width, height);

                    bool intersects = false;
                    foreach (Rectangle sRect in _sourceRectangles.Values)
                    {
                        if (sRect.Intersects(sharpSf))
                        {
                            x = sRect.X + sRect.Width;

                            if (x + width < _width - 1)
                            {
                                if (ymin > sRect.Y + sRect.Height)
                                {
                                    ymin = sRect.Y + sRect.Height;
                                }
                            }

                            intersects = true;
                            break;
                        }
                    }

                    if (!intersects && x + width <= _width - 1 && y + height <= _height - 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Releases the unmanaged resources used by the Exomia.Framework.Graphics.SpriteBatchAtlas
        ///     and optionally releases the managed resources.
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
                    lock (_lockAtlas)
                    {
                        Utilities.Dispose(ref _atlas);
                    }
                    _sourceRectangles.Clear();
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="SpriteBatchAtlas" /> class.
        /// </summary>
        ~SpriteBatchAtlas()
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
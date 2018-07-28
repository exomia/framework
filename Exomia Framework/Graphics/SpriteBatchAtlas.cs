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
    sealed class SpriteBatchAtlas : IDisposable
    {
        private const int MIN_ATLAS_WIDTH = 2048;
        private const int MIN_ATLAS_HEIGHT = 2048;

        private const int MAX_ATLAS_WIDTH = 8192;
        private const int MAX_ATLAS_HEIGHT = 8192;
        private readonly int _height;

        private readonly object _lockAtlas = new object();

        private readonly Dictionary<string, Rectangle> _sourceRectangles;

        private readonly int _width;

        private Bitmap _atlas;

        public SpriteBatchAtlas(int width, int height)
        {
            if (width < MIN_ATLAS_WIDTH) { width = MIN_ATLAS_WIDTH; }
            if (height < MIN_ATLAS_HEIGHT) { height = MIN_ATLAS_HEIGHT; }

            if (width > MAX_ATLAS_WIDTH) { width = MAX_ATLAS_WIDTH; }
            if (height > MAX_ATLAS_HEIGHT) { height = MAX_ATLAS_HEIGHT; }

            _width = width;
            _height = height;

            _sourceRectangles = new Dictionary<string, Rectangle>(16);

            _atlas = new Bitmap(width, height);
        }

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

        internal bool TryGetSourceRectangle(string assetName, out Rectangle sourceRectangle)
        {
            return _sourceRectangles.TryGetValue(assetName, out sourceRectangle);
        }

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
                    Rectangle sharpSF = new Rectangle(x, y, width, height);

                    bool intersects = false;
                    foreach (Rectangle sRect in _sourceRectangles.Values)
                    {
                        if (sRect.Intersects(sharpSF))
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

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    /* USER CODE */
                    Utilities.Dispose(ref _atlas);
                    _sourceRectangles.Clear();
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
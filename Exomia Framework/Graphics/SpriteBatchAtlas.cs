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
    internal sealed class SpriteBatchAtlas : IDisposable
    {
        #region Constructors

        #region Statics

        #endregion

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

        #endregion

        #region Constants

        private const int MIN_ATLAS_WIDTH = 2048;
        private const int MIN_ATLAS_HEIGHT = 2048;

        private const int MAX_ATLAS_WIDTH = 8192;
        private const int MAX_ATLAS_HEIGHT = 8192;

        #endregion

        #region Variables

        #region Statics

        #endregion

        private readonly int _width;
        private readonly int _height;

        private readonly Dictionary<string, Rectangle> _sourceRectangles;

        private Bitmap _atlas;

        private readonly object _lockAtlas = new object();

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

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
            Rectangle sourceRectangle = Rectangle.Empty;

            if (!_sourceRectangles.TryGetValue(assetName, out sourceRectangle))
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
            Rectangle sharpSF;
            x = 0;
            y = 0;
            int ymin = 1;
            for (y = 1; y < _height - 1; y++)
            {
                ymin = _height - 1;
                for (x = 1; x < _width - 1; x++)
                {
                    if (x + width > _width - 1)
                    {
                        y = ymin;
                        break;
                    }
                    sharpSF = new Rectangle(x, y, width, height);

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
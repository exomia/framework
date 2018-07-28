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
using System.IO;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.Game;

namespace Exomia.Framework.Graphics
{
    sealed class SpriteFontContentReader : IContentReader
    {
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            SpriteFont font = ContentSerializer.Read<SpriteFont>(parameters.Stream);
            if (font?.ImageData == null)
            {
                return null;
            }

            IGraphicsDevice graphicsDevice = contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            if (graphicsDevice == null)
            {
                throw new InvalidOperationException("Unable to retrieve a IGraphicsDevice");
            }

            try
            {
                using (MemoryStream ms = new MemoryStream(font.ImageData))
                {
                    ms.Position = 0;
                    font.Texture = Texture.Load(graphicsDevice.Device, ms);
                }
            }
            catch { return null; }

            return font;
        }
    }

    sealed class SpriteFont2ContentReader : IContentReader
    {
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            SpriteFont2 font = ContentSerializer.Read<SpriteFont2>(parameters.Stream);

            if (font?.ImageData == null)
            {
                return null;
            }

            ITexture2ContentManager manager =
                contentManager.ServiceRegistry.GetService<ITexture2ContentManager>();
            if (manager == null)
            {
                throw new InvalidOperationException("Unable to retrieve a ITextureContentManager");
            }

            try
            {
                using (MemoryStream ms = new MemoryStream(font.ImageData))
                {
                    ms.Position = 0;
                    font.Texture2 = manager.AddTexture(ms, parameters.AssetName);
                }
            }
            catch { return null; }

            return font;
        }
    }
}
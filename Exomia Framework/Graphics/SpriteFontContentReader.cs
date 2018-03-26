using System;
using System.IO;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.Game;

namespace Exomia.Framework.Graphics
{
    internal sealed class SpriteFontContentReader : IContentReader
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

    internal sealed class SpriteFont2ContentReader : IContentReader
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
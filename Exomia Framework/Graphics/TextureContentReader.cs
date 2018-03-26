using System;
using Exomia.Framework.Content;
using Exomia.Framework.Game;

namespace Exomia.Framework.Graphics
{
    internal sealed class TextureContentReader : IContentReader
    {
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            IGraphicsDevice graphicsDevice = contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            if (graphicsDevice == null) { throw new InvalidOperationException("Unable to retrieve a IGraphicsDevice"); }
            return Texture.Load(graphicsDevice.Device, parameters.Stream);
        }
    }

    internal sealed class Texture2ContentReader : IContentReader
    {
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            ITexture2ContentManager manager = contentManager.ServiceRegistry.GetService<ITexture2ContentManager>();
            if (manager == null) { throw new InvalidOperationException("Unable to retrieve a ITextureContentManager"); }
            try
            {
                return manager.AddTexture(parameters.Stream, parameters.AssetName);
            }
            catch { return null; }
        }
    }
}
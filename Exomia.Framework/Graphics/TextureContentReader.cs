#region MIT License

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

using System;
using Exomia.Framework.Content;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A texture content reader. This class cannot be inherited.
    /// </summary>
    sealed class TextureContentReader : IContentReader
    {
        /// <inheritdoc />
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            IGraphicsDevice graphicsDevice = contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            if (graphicsDevice == null)
            {
                throw new InvalidOperationException($"Unable to retrieve a {nameof(IGraphicsDevice)}");
            }
            return Texture.Load(graphicsDevice.Device, parameters.Stream);
        }
    }

    /// <summary>
    ///     A texture 2 content reader. This class cannot be inherited.
    /// </summary>
    sealed class Texture2ContentReader : IContentReader
    {
        /// <inheritdoc />
        public object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            ITexture2ContentManager manager = contentManager.ServiceRegistry.GetService<ITexture2ContentManager>();
            if (manager == null)
            {
                throw new InvalidOperationException($"Unable to retrieve a {nameof(ITexture2ContentManager)}");
            }
            try
            {
                return manager.AddTexture(parameters.Stream, parameters.AssetName);
            }
            catch { return null; }
        }
    }
}
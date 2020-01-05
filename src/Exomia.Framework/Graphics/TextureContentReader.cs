#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
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
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
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
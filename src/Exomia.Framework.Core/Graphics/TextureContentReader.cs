#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content;

namespace Exomia.Framework.Core.Graphics
{
    /// <summary>
    ///     A texture content reader. This class cannot be inherited.
    /// </summary>
    sealed class TextureContentReader : IContentReader
    {
        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            IGraphicsDevice graphicsDevice =
                contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            return Texture.Load(graphicsDevice.Device, parameters.Stream);
        }
    }
}
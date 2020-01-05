#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A sprite font content reader. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFontContentReader : IContentReader
    {
        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            SpriteFont font = ContentSerializer.Read<SpriteFont>(parameters.Stream);
            if (font?.ImageData == null)
            {
                return null;
            }

            IGraphicsDevice graphicsDevice = contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            if (graphicsDevice == null)
            {
                throw new InvalidOperationException($"Unable to retrieve a {nameof(IGraphicsDevice)}");
            }

            try
            {
                using MemoryStream ms = new MemoryStream(font.ImageData) { Position = 0 };
                font.Texture = Texture.Load(graphicsDevice.Device, ms) ??
                               throw new NullReferenceException($"{nameof(font.Texture)}");
            }
            catch { return null; }

            return font;
        }
    }

    /// <summary>
    ///     A sprite font 2 content reader. This class cannot be inherited.
    /// </summary>
    sealed class SpriteFont2ContentReader : IContentReader
    {
        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
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
                throw new InvalidOperationException($"Unable to retrieve a {nameof(ITexture2ContentManager)}");
            }

            try
            {
                using MemoryStream ms = new MemoryStream(font.ImageData) { Position = 0 };
                font.Texture2 = manager.AddTexture(ms, parameters.AssetName);
            }
            catch { return null; }

            return font;
        }
    }
}
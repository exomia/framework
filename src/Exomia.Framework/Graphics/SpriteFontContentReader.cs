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
    sealed class SpriteFontContentReader : IContentReader
    {
        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            SpriteFont font = ContentSerializer.Read<SpriteFont>(parameters.Stream);
            if (font.ImageData.Length <= 0)
            {
                return null;
            }

            IGraphicsDevice graphicsDevice =
                contentManager.ServiceRegistry.GetService<IGraphicsDevice>();

            try
            {
                using (MemoryStream ms = new MemoryStream(font.ImageData) { Position = 0 })
                {
                    font.Texture = Texture.Load(graphicsDevice.Device, ms) ??
                                   throw new NullReferenceException($"{nameof(font.Texture)}");
                }
            }
            catch { return null; }

            return font;
        }
    }
}
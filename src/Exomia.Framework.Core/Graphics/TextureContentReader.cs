#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Resolver;
using Exomia.Framework.Core.Vulkan;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Graphics;

sealed class TextureContentReader : IContentReader
{
    /// <inheritdoc />
    public unsafe object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        byte[] buffer = new byte[E1.TextureMagicHeader.Length];
        if (parameters.Stream.Read(buffer, 0, E1.TextureMagicHeader.Length) != E1.TextureMagicHeader.Length ||
            !E1.TextureMagicHeader.SequenceEqual(buffer))
        {
            return null;
        }

        using Stream       stream = ContentCompressor.DecompressStream(parameters.Stream);
        using BinaryReader br     = new BinaryReader(stream);

        int width  = br.ReadInt32();
        int height = br.ReadInt32();

        if (width == 0 || height == 0)
        {
            return null;
        }

        byte[] data = br.ReadBytes(width * height * 4 /* RGBA */);

        VkContext* vkContext = contentManager.ServiceProvider.GetRequiredService<Vulkan.Vulkan>().Context;

        return Texture.Create(vkContext, (uint)width, (uint)height, data);
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Vulkan;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Content.E1.ContentReader;

sealed class E1TextureContentReader : IContentReader
{
    /// <inheritdoc />
    public unsafe object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        byte[] buffer = new byte[E1Protocol.TextureMagicHeader.Length];
        if (parameters.Stream.Read(buffer, 0, E1Protocol.TextureMagicHeader.Length) != E1Protocol.TextureMagicHeader.Length ||
            !E1Protocol.TextureMagicHeader.SequenceEqual(buffer))
        {
            //reset the stream position
            parameters.Stream.Seek(-E1Protocol.TextureMagicHeader.Length, SeekOrigin.Current);
            return null;
        }

        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use

        using Stream       stream = ContentCompressor.DecompressStream(parameters.Stream);
        using BinaryReader br     = new BinaryReader(stream);

        int width  = br.ReadInt32();
        int height = br.ReadInt32();

        if (width == 0 || height == 0)
        {
            throw new NotSupportedException($"Invalid texture size of {width}x{height}; The texture must be at least 1x1!");
        }

        byte[] data = br.ReadBytes(width * height * 4 /* RGBA */);

        VkContext* vkContext = contentManager.ServiceProvider.GetRequiredService<Vulkan.Vulkan>().Context;

        return Texture.Create(vkContext, (uint)width, (uint)height, data);
    }
}
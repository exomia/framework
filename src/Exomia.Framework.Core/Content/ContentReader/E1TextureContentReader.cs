#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Protocols;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Vulkan;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Content.ContentReader;

sealed unsafe class E1TextureContentReader : IContentReader
{
    /// <inheritdoc />
    public Type ProtocolType
    {
        get { return typeof(E1Protocol); }
    }

    private static object? ReadContentV10(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
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

    /// <inheritdoc />
    public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        long startPosition = parameters.Stream.Position;

        byte[] buffer = new byte[E1Protocol.Texture.MagicHeader.Length];

        if (parameters.Stream.Read(buffer, 0, E1Protocol.Texture.MagicHeader.Length) != E1Protocol.Texture.MagicHeader.Length ||
            !buffer.AsSpan().SequenceEqual(E1Protocol.Texture.MagicHeader))
        {
            //reset the stream position
            parameters.Stream.Seek(startPosition, SeekOrigin.Begin);
            return null;
        }

        if (parameters.Stream.Read(buffer, 0, E1Protocol.TYPE_PROTOCOL_VERSION_LENGHT) != E1Protocol.TYPE_PROTOCOL_VERSION_LENGHT)
        {
            //reset the stream position
            parameters.Stream.Seek(startPosition, SeekOrigin.Begin);
            return null;
        }

        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use

        if (buffer.AsSpan().StartsWith(E1Protocol.Texture.Version10))
        {
            return ReadContentV10(contentManager, ref parameters);
        }

        //reset the stream position
        parameters.Stream.Seek(startPosition, SeekOrigin.Begin);
        return null;
    }
}
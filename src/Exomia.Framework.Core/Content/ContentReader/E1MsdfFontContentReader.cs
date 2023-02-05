#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Protocols;
using Exomia.Framework.Core.Extensions;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Content.ContentReader;

sealed unsafe class E1MsdfFontContentReader : IContentReader
{
    /// <inheritdoc />
    public Type ProtocolType
    {
        get { return typeof(E1Protocol); }
    }

    /// <inheritdoc />
    public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        if (!parameters.Stream.SequenceEqual(E1Protocol.MsdfFont.MagicHeader))
        {
            return null;
        }

        for (int i = 0; i < E1Protocol.TYPE_RESERVED_BYTES_LENGHT; i++) //reserved for future use
        {
            parameters.Stream.ReadByte(); //reserved for future use
        }

        Span<byte> protocolVersion = stackalloc byte[E1Protocol.TYPE_PROTOCOL_VERSION_LENGHT];
        if (parameters.Stream.Read(protocolVersion) != E1Protocol.TYPE_PROTOCOL_VERSION_LENGHT)
        {
            return null;
        }

        if (protocolVersion.SequenceEqual(E1Protocol.MsdfFont.Version10))
        {
            return ReadContentV10(contentManager, ref parameters);
        }

        return null;
    }

    private static object? ReadContentV10(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        using Stream stream = new MemoryStream();
        ContentCompressor.DecompressStream(parameters.Stream, stream);
        stream.Seek(0, SeekOrigin.Begin);

        using BinaryReader br = new BinaryReader(stream);

        string name = br.ReadString();

        MsdfFont.AtlasInfo atlas = new MsdfFont.AtlasInfo
        {
            Type          = br.ReadString(),
            DistanceRange = br.ReadInt32(),
            Size          = br.ReadDouble(),
            Width         = br.ReadInt32(),
            Height        = br.ReadInt32(),
            YOrigin       = br.ReadString()
        };

        if (atlas.Width == 0 || atlas.Height == 0)
        {
            throw new NotSupportedException($"Invalid texture size of {atlas.Width}x{atlas.Height}; The texture must be at least 1x1!");
        }

        MsdfFont.MetricsData metrics = new MsdfFont.MetricsData
        {
            EmSize             = br.ReadInt32(),
            LineHeight         = br.ReadDouble(),
            Ascender           = br.ReadDouble(),
            Descender          = br.ReadDouble(),
            UnderlineY         = br.ReadDouble(),
            UnderlineThickness = br.ReadDouble()
        };

        bool ignoreUnknownCharacters = br.ReadBoolean();
        int  defaultUnicode          = br.ReadInt32();

        Dictionary<int, MsdfFont.Glyph> glyphs     = new();
        int                             glyphCount = br.ReadInt32();
        for (int i = 0; i < glyphCount; i++)
        {
            glyphs.Add(br.ReadInt32(), new MsdfFont.Glyph
            {
                Advance = br.ReadDouble(),
                PlaneBounds = br.ReadBoolean()
                    ? new RectangleD(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble())
                    : RectangleD.Empty,
                AtlasBounds = br.ReadBoolean()
                    ? new RectangleD(br.ReadDouble(), br.ReadDouble(), br.ReadDouble(), br.ReadDouble())
                    : RectangleD.Empty,
            });
        }

        Dictionary<int, double> kernings      = new();
        int                     kerningsCount = br.ReadInt32();
        for (int i = 0; i < kerningsCount; i++)
        {
            kernings.Add((br.ReadInt32() << 16) | br.ReadInt32(), br.ReadDouble());
        }

        byte[] data = br.ReadBytes(atlas.Width * atlas.Height * 4 /* RGBA */);

        VkContext* vkContext = contentManager.ServiceProvider.GetRequiredService<IVkContextAccessor>().Context;
        Texture    texture   = Texture.Create(vkContext, (uint)atlas.Width, (uint)atlas.Height, data);
        return new MsdfFont(name, atlas, metrics, glyphs, kernings, texture, defaultUnicode)
        {
            IgnoreUnknownCharacters = ignoreUnknownCharacters
        };
    }
}
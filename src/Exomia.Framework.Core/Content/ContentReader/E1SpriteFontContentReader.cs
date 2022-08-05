#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.E1;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Vulkan;
using Microsoft.Extensions.DependencyInjection;

namespace Exomia.Framework.Core.Content.ContentReader;

sealed unsafe class E1SpriteFontContentReader : IContentReader
{
    /// <inheritdoc />
    public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
    {
        byte[] buffer = new byte[E1Protocol.SpritefontMagicHeader.Length];
        if (parameters.Stream.Read(buffer, 0, E1Protocol.SpritefontMagicHeader.Length) != E1Protocol.SpritefontMagicHeader.Length ||
            !E1Protocol.SpritefontMagicHeader.SequenceEqual(buffer))
        {
            //reset the stream position
            parameters.Stream.Seek(-E1Protocol.SpritefontMagicHeader.Length, SeekOrigin.Current);
            return null;
        }

        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use
        parameters.Stream.ReadByte(); //reserved for future use

        using Stream       stream = ContentCompressor.DecompressStream(parameters.Stream);
        using BinaryReader br     = new BinaryReader(stream);

        string face                    = br.ReadString();
        int    size                    = br.ReadInt32();
        int    lineHeight              = br.ReadInt32();
        int    defaultCharacter        = br.ReadInt32();
        bool   ignoreUnknownCharacters = br.ReadBoolean();
        bool   bold                    = br.ReadBoolean();
        bool   italic                  = br.ReadBoolean();

        Dictionary<int, SpriteFont.Glyph> glyphs     = new();
        int                               glyphCount = br.ReadInt32();
        for (int i = 0; i < glyphCount; i++)
        {
            glyphs.Add(br.ReadInt32(), new SpriteFont.Glyph
            {
                X        = br.ReadInt32(),
                Y        = br.ReadInt32(),
                Width    = br.ReadInt32(),
                Height   = br.ReadInt32(),
                OffsetX  = br.ReadInt32(),
                OffsetY  = br.ReadInt32(),
                XAdvance = br.ReadInt32()
            });
        }

        Dictionary<int, int> kernings      = new();
        int                  kerningsCount = br.ReadInt32();
        for (int i = 0; i < kerningsCount; i++)
        {
            kernings.Add((br.ReadInt32() << 16) | br.ReadInt32(), br.ReadInt32());
        }

        int width  = br.ReadInt32();
        int height = br.ReadInt32();

        if (width == 0 || height == 0)
        {
            throw new NotSupportedException($"Invalid texture size of {width}x{height}; The texture must be at least 1x1!");
        }

        byte[] data = br.ReadBytes(width * height * 4 /* RGBA */);

        VkContext* vkContext = contentManager.ServiceProvider.GetRequiredService<Vulkan.Vulkan>().Context;
        Texture    texture   = Texture.Create(vkContext, (uint)width, (uint)height, data);
        return new SpriteFont(
            face,
            size,
            lineHeight,
            glyphs,
            kernings,
            texture,
            defaultCharacter)
        {
            IgnoreUnknownCharacters = ignoreUnknownCharacters,
            Bold                    = bold,
            Italic                  = italic
        };
    }
}
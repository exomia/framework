#pragma warning disable 1591

using System;
using System.IO;
using Exomia.Framework.Content;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.Graphics;

namespace Exomia.Framework.Security
{
    public static class DecryptionHelper
    {
        public static SpriteFont2 ToSpriteFont2(this Stream stream, ITexture2ContentManager manager)
        {
            if (manager == null) { throw new InvalidOperationException("Unable to retrieve a ITextureContentManager"); }

            if (!ExomiaCryptography.Decrypt(stream, out Stream outStream))
            {
                throw new IOException("resource 'font' failed to decrypt");
            }

            SpriteFont font = ContentSerializer.Read<SpriteFont>(outStream);

            if (font?.ImageData == null) { return null; }

            SpriteFont2 font2 = new SpriteFont2
            {
                Bold = font.Bold,
                DefaultCharacter = font.DefaultCharacter,
                DefaultGlyph = font.DefaultGlyph,
                Face = font.Face,
                Glyphs = font.Glyphs,
                IgnoreUnknownCharacters = font.IgnoreUnknownCharacters,
                ImageData = font.ImageData,
                Italic = font.Italic,
                Kernings = font.Kernings,
                LineSpacing = font.LineSpacing,
                Size = font.Size,
                SpacingX = font.SpacingX,
                SpacingY = font.SpacingY
            };

            try
            {
                using (MemoryStream ms = new MemoryStream(font2.ImageData))
                {
                    ms.Position = 0;
                    font2.Texture2 = manager.AddTexture(ms, font2.Face + font.Size);
                }
            }
            catch { return null; }

            return font2;
        }
    }
}
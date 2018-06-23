#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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
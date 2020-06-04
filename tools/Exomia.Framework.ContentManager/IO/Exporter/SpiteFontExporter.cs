#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Data;
using System.IO;
using Exomia.Framework.ContentManager.Fonts.BMFont;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.ContentSerialization.Compression;
using Exomia.Framework.Graphics;
using SharpDX;

namespace Exomia.Framework.ContentManager.IO.Exporter
{
    /// <summary>
    ///     A bm font exporter. This class cannot be inherited.
    /// </summary>
    [Exporter("SpiteFont Exporter")]
    sealed class SpiteFontExporter : Exporter<FontFile>
    {
        public static SpiteFontExporter Default { get; } = new SpiteFontExporter();

        private SpiteFontExporter() { }

        /// <inheritdoc />
        public override bool Export(FontFile fontFile, ExporterContext context)
        {
            byte[] imageData;
            using (FileStream fs = new FileStream(
                fontFile.Pages[0].File, FileMode.Open, FileAccess.Read))
            {
                imageData = new byte[fs.Length];
                fs.Read(imageData, 0, imageData.Length);
            }
            SpriteFont font = new SpriteFont
            {
                ImageData        = imageData,
                Face             = fontFile.Info.Face,
                Size             = fontFile.Info.Size,
                Bold             = fontFile.Info.Bold != 0,
                Italic           = fontFile.Info.Italic != 0,
                LineSpacing      = fontFile.Common.LineHeight,
                DefaultCharacter = -1
            };

            for (int i = 0; i < fontFile.Chars.Count; i++)
            {
                FontChar c = fontFile.Chars[i];
                font.Glyphs.Add(
                    c.ID,
                    new SpriteFont.Glyph
                    {
                        Character = c.ID,
                        OffsetX   = c.XOffset,
                        OffsetY   = c.YOffset,
                        XAdvance  = c.XAdvance,
                        Subrect   = new Rectangle(c.X, c.Y, c.Width, c.Height)
                    });
            }
            for (int i = 0; i < fontFile.Kernings.Count; i++)
            {
                FontKerning k = fontFile.Kernings[i];
                font.Kernings.Add(
                    (k.First << 16) | k.Second,
                    new SpriteFont.Kerning { First = k.First, Second = k.Second, Offset = k.Amount });
            }

            string outputFile = Path.Combine(
                context.OutputFolder, context.VirtualPath,
                Path.GetFileNameWithoutExtension(context.ItemName));

            string assetName1 = outputFile + ".min" + ContentSerializer.DEFAULT_EXTENSION;
            string assetName2 = outputFile + ContentCompressor.DEFAULT_COMPRESSED_EXTENSION;

            if (!Directory.Exists(Path.GetDirectoryName(assetName1)))
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(assetName1) ?? throw new NoNullAllowedException());
            }

            ContentSerializer.Write(assetName1, font, true);

            using (FileStream fs = new FileStream(assetName1, FileMode.Open, FileAccess.Read))
            {
                if (ContentCompressor.CompressStream(
                    fs, out Stream compressedStream))
                {
                    using (FileStream fs1 = new FileStream(
                        assetName2, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[1024];
                        int    write;
                        while ((write = compressedStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            fs1.Write(buffer, 0, write);
                        }
                    }

                    context.AddMessage("{0:green} {1}", "successful created", assetName2);
                }
            }

            return true;
        }
    }
}
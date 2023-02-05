#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Data;
using Exomia.Framework.ContentManager.Fonts.MSDFFont;
using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Protocols;
using FontFile = Exomia.Framework.ContentManager.Fonts.MSDFFont.FontFile;

namespace Exomia.Framework.ContentManager.IO.Exporter;

[Exporter("MSDF Font Exporter")]
sealed class MsdfFontExporter : Exporter<FontFile>
{
    /// <inheritdoc />
    public override bool Export(FontFile fontFile, ExporterContext context)
    {
        string outputFile = Path.Combine(
            context.OutputFolder, context.VirtualPath,
            Path.GetFileNameWithoutExtension(context.ItemName));

        string assetName = outputFile + E1Protocol.EXTENSION_NAME;

        if (!Directory.Exists(Path.GetDirectoryName(assetName)))
        {
            Directory.CreateDirectory(
                Path.GetDirectoryName(assetName) ?? throw new NoNullAllowedException());
        }

        using (Stream staging = new MemoryStream())
        using (BinaryWriter bw = new BinaryWriter(staging))
        {
            bw.Write(fontFile.name);
            
            bw.Write(fontFile.atlas.type);
            bw.Write(fontFile.atlas.distanceRange);
            bw.Write(fontFile.atlas.size);
            bw.Write(fontFile.atlas.width);
            bw.Write(fontFile.atlas.height);
            bw.Write(fontFile.atlas.yOrigin);
            
            bw.Write(fontFile.metrics.emSize);
            bw.Write(fontFile.metrics.lineHeight);
            bw.Write(fontFile.metrics.ascender);
            bw.Write(fontFile.metrics.descender);
            bw.Write(fontFile.metrics.underlineY);
            bw.Write(fontFile.metrics.underlineThickness);
            
            bw.Write(fontFile.IgnoreUnknownCharacters);
            bw.Write(fontFile.DefaultGlyph);

            bw.Write(fontFile.glyphs.Count);
            for (int i = 0; i < fontFile.glyphs.Count; i++)
            {
                Glyph g = fontFile.glyphs[i];
                bw.Write(g.unicode);
                bw.Write(g.advance);
                bw.Write(g.planeBounds != null);
                if (g.planeBounds != null)
                {
                    bw.Write(g.planeBounds.left);
                    bw.Write(g.planeBounds.top);
                    bw.Write(g.planeBounds.right);
                    bw.Write(g.planeBounds.bottom);
                } 
                bw.Write(g.atlasBounds != null);
                if (g.atlasBounds != null)
                {
                    bw.Write(g.atlasBounds.left);
                    bw.Write(g.atlasBounds.top);
                    bw.Write(g.atlasBounds.right);
                    bw.Write(g.atlasBounds.bottom);
                }
            }
            
            bw.Write(fontFile.kerning.Count);
            for (int i = 0; i < fontFile.kerning.Count; i++)
            { 
                Kerning k = fontFile.kerning[i];
                bw.Write(k.unicode1);
                bw.Write(k.unicode2);
                bw.Write(k.advance);
            }

            using (FileStream fs = new FileStream(
                       fontFile.ImageDataFileName ?? throw new NullReferenceException(), FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[2048];
                int    read   = 0;
                while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    bw.Write(buffer, 0, read);
                }
            }

            staging.Seek(0, SeekOrigin.Begin);

            using (FileStream fs = new FileStream(assetName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(E1Protocol.MagicHeader,          0, E1Protocol.MagicHeader.Length);
                fs.Write(E1Protocol.MsdfFont.MagicHeader, 0, E1Protocol.MsdfFont.MagicHeader.Length);
                for (int i = 0; i < E1Protocol.TYPE_RESERVED_BYTES_LENGHT; i++) //reserved for future use
                {
                    fs.WriteByte(0);
                }
                fs.Write(E1Protocol.MsdfFont.Version10, 0, E1Protocol.MsdfFont.Version10.Length);

                ContentCompressor.CompressStream(staging, fs);
            }
        }

        context.AddMessage("{0:green} {1}", "successful created", assetName);

        return true;
    }
}
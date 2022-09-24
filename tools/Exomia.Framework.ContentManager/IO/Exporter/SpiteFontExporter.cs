#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Data;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
using Exomia.Framework.ContentManager.Fonts.BMFont;
using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Protocols;

namespace Exomia.Framework.ContentManager.IO.Exporter;

[Exporter("SpiteFont Exporter")]
sealed class SpiteFontExporter : Exporter<FontFile>
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
            bw.Write(fontFile.Info!.Face!);
            bw.Write(fontFile.Info.Size);
            bw.Write(fontFile.Common!.LineHeight);
            bw.Write(-1);
            bw.Write(false);
            bw.Write(fontFile.Info.Bold   != 0);
            bw.Write(fontFile.Info.Italic != 0);

            bw.Write(fontFile.Chars!.Count);
            for (int i = 0; i < fontFile.Chars!.Count; i++)
            {
                FontChar c = fontFile.Chars[i];
                bw.Write(c.ID);
                bw.Write(c.X);
                bw.Write(c.Y);
                bw.Write(c.Width);
                bw.Write(c.Height);
                bw.Write(c.XOffset);
                bw.Write(c.YOffset);
                bw.Write(c.XAdvance);
            }

            bw.Write(fontFile.Kernings!.Count);
            for (int i = 0; i < fontFile.Kernings!.Count; i++)
            {
                FontKerning k = fontFile.Kernings[i];
                bw.Write(k.First);
                bw.Write(k.Second);
                bw.Write(k.Amount);
            }

            using (FileStream fs = new FileStream(
                       fontFile.Pages?[0].File ?? throw new NullReferenceException(), FileMode.Open, FileAccess.Read))
            {
                using Bitmap bitmap = new Bitmap(fs);
                BitmapData   data   = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    byte[] bytes = new byte[data.Stride * data.Height];

                    unsafe
                    {
                        fixed (byte* dst = bytes)
                        {
                            Unsafe.CopyBlock(dst, data.Scan0.ToPointer(), (uint)bytes.Length);
                        }
                    }

                    bw.Write(bitmap.Width);
                    bw.Write(bitmap.Height);
                    bw.Write(bytes);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }

            staging.Seek(0, SeekOrigin.Begin);

            using (FileStream fs = new FileStream(assetName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(E1Protocol.MagicHeader,            0, E1Protocol.MagicHeader.Length);
                fs.Write(E1Protocol.Spritefont.MagicHeader, 0, E1Protocol.Spritefont.MagicHeader.Length);
                for (int i = 0; i < E1Protocol.TYPE_RESERVED_BYTES_LENGHT; i++) //reserved for future use
                {
                    fs.WriteByte(0);
                }
                fs.Write(E1Protocol.Spritefont.Version10, 0, E1Protocol.Spritefont.Version10.Length);

                ContentCompressor.CompressStream(staging, fs);
            }
        }

        context.AddMessage("{0:green} {1}", "successful created", assetName);

        return true;
    }
}
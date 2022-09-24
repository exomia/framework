#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Data;
using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Protocols;

namespace Exomia.Framework.ContentManager.IO.Exporter;

[Exporter("Texture Exporter")]
sealed class TextureExporter : Exporter<Texture.Texture>
{
    public override bool Export(Texture.Texture obj, ExporterContext context)
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
            bw.Write(obj.Width);
            bw.Write(obj.Height);
            if (obj.Data != null)
            {
                bw.Write(obj.Data);
            }
            staging.Seek(0, SeekOrigin.Begin);

            using (FileStream fs = new FileStream(assetName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(E1Protocol.MagicHeader,         0, E1Protocol.MagicHeader.Length);
                fs.Write(E1Protocol.Texture.MagicHeader, 0, E1Protocol.Texture.MagicHeader.Length);
                for (int i = 0; i < E1Protocol.TYPE_RESERVED_BYTES_LENGHT; i++) //reserved for future use
                {
                    fs.WriteByte(0); 
                }
                fs.Write(E1Protocol.Texture.Version10,   0, E1Protocol.Texture.Version10.Length);

                ContentCompressor.CompressStream(staging, fs);
            }
        }

        context.AddMessage("{0:green} {1}", "successful created", assetName);

        return true;
    }
}
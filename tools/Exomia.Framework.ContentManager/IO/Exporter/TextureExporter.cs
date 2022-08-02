using System.Data;
using Exomia.Framework.Core.Content.Compression;
using Exomia.Framework.Core.Content.Resolver;

namespace Exomia.Framework.ContentManager.IO.Exporter;

[Exporter("Texture Exporter")]
sealed class TextureExporter : Exporter<Texture.Texture>
{
    public override bool Export(Texture.Texture obj, ExporterContext context)
    {
        string outputFile = Path.Combine(
            context.OutputFolder, context.VirtualPath,
            Path.GetFileNameWithoutExtension(context.ItemName));

        string assetName = outputFile + E1.EXTENSION_NAME;

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
                fs.Write(E1.MagicHeader,        0, E1.MagicHeader.Length);
                fs.Write(E1.TextureMagicHeader, 0, E1.TextureMagicHeader.Length);
                ContentCompressor.CompressStream(staging, fs);
            }
        }

        context.AddMessage("{0:green} {1}", "successful created", assetName);

        return true;
    }
}
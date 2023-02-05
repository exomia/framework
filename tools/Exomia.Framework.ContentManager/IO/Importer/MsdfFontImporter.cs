#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Exomia.Framework.ContentManager.Extensions;
using Exomia.Framework.ContentManager.Fonts.MSDFFont;

namespace Exomia.Framework.ContentManager.IO.Importer;

[Importer("Msdf Font Importer", ".ttf", ".otf")]
sealed class MsdfFontImporter : Importer<FontFile>
{
    private const string TEMP_FILE_DIR = "temp";

    private readonly string _msdfAtlasGenExeLocation;

    public MsdfFontImporter()
    {
        if (!File.Exists(_msdfAtlasGenExeLocation = Path.Combine("third-party", "tools", "msdf-atlas-gen", "msdf-atlas-gen.exe")))
        {
            throw new FileNotFoundException("The 'msdf-atlas-gen.exe' is missing!", _msdfAtlasGenExeLocation);
        }
    }

    /// <inheritdoc />
    public override object? CreateImporterSettings()
    {
        return new Settings();
    }

    protected override async Task<FontFile?> ImportAsync(ImporterContext context, CancellationToken cancellationToken)
    {
        if (context.ImporterSettings is not Settings settings)
        {
            context.AddMessage("Invalid importer settings!");
            return null;
        }

        return await Task.Run(
            () =>
            {
                string tempFileName =
                    Path.Combine(TEMP_FILE_DIR, Path.GetRandomFileName());

                StringBuilder sb = new StringBuilder($"-font \"{context.FileName}\" -fontname \"{context.ItemName}\" -imageout \"{tempFileName}.bin\" -json \"{tempFileName}.json\" -format bin");

                // ReSharper disable HeapView.BoxingAllocation
                sb.AppendFormatIfNotNull(" -charset \"{0}\"",  settings.CharSet);
                sb.AppendFormatIfNotNull(" -glyphset \"{0}\"", settings.Glyphset);
                sb.AppendFormatIfNotNull(" -fontscale {0}",    settings.FontScale);
                sb.AppendFormat(" -type  {0:G}", settings.AtlasType);
                sb.AppendFormat(" -{0:G}",       settings.AtlasDimension);
                sb.AppendFormatIfNotNull(" -size  {0}",    settings.Size);
                sb.AppendFormatIfNotNull(" -minsize  {0}", settings.Minsize);
                sb.AppendFormatIfNotNull(" -emrange {0}",  settings.EmRange);
                sb.AppendFormat(" -pxrange {0}", settings.PxRange);
                // ReSharper enable HeapView.BoxingAllocation


                using (Process p = Process.Start(
                           new ProcessStartInfo(_msdfAtlasGenExeLocation, sb.ToString())
                           {
                               UseShellExecute = false, CreateNoWindow = false
                           })!)
                {
                    if (!p.HasExited)
                    {
                        int i = 0;
                        while (!p.WaitForExit(1_000)                      &&
                               !cancellationToken.IsCancellationRequested &&
                               i++ < 45) { }
                        if (!p.HasExited)
                        {
                            try
                            {
                                p.Kill();
                            }
                            catch
                            {
                                /* IGNORE*/
                            }
                        }
                    }

                    if (p.ExitCode == 0)
                    {
                        FontFile? fontFile = Json.Deserialize<FontFile>($"{tempFileName}.json");
                        if (fontFile == null)
                        {
                            context.AddMessage(
                                "Can't import the font '{0:OrangeRed}'! " +
                                "The font file can't be deserialized!", $"{tempFileName}.json");
                            return null;
                        }
                        
                        fontFile.DefaultGlyph            = settings.DefaultGlyph;
                        fontFile.IgnoreUnknownCharacters = settings.IgnoreUnknownCharacters;
                        fontFile.ImageDataFileName       = $"{tempFileName}.bin";
                        return fontFile;
                    }

                    context.AddMessage("Msdf Font Importer exited with code {0}!", p.ExitCode);
                    return null;
                }
            }, cancellationToken);
    }

    sealed class Settings
    {
        [Category("Input")]
        [Description("sets the character set. The ASCII charset will be used if not specified.")]
        public string? CharSet { get; set; } = null;

        [Category("Misc")]
        [Description("true to ignore unknown characters. The default is false.")]
        public bool IgnoreUnknownCharacters { get; set; } = false;

        [Category("Misc")]
        [Description("The default glyph to use for unknown characters. The default is '?'.")]
        public int DefaultGlyph { get; set; } = '?';

        [Category("Input")]
        [Description("sets the set of input glyphs using their indices within the font file.")]
        public string? Glyphset { get; set; } = null;

        [Category("Input")]
        [Description("applies a scaling transformation to the font's glyphs. Mainly to be used to generate multiple sizes in a single atlas, otherwise use -size.")]
        public int? FontScale { get; set; } = null;

        [Category("Bitmap atlas type")]
        public BitmapAtlasType AtlasType { get; set; } = BitmapAtlasType.mtsdf;

        public enum BitmapAtlasType
        {
            hardmask,
            softmask,
            sdf,
            psdf,
            msdf,
            mtsdf
        }

        [Category("Atlas dimensions")]
        [Description("sets fixed atlas dimensions")]
        public BitmapAtlasDimensions AtlasDimension { get; set; } = BitmapAtlasDimensions.square4;

        public enum BitmapAtlasDimensions
        {
            pots,
            potr,
            square,
            square2,
            square4,
        }

        [Category("Glyph configuration")]
        [Description("sets the size of the glyphs in the atlas in pixels per EM.")]
        public int? Size { get; set; } = null;

        [Category("Glyph configuration")]
        [Description("sets the minimum size. The largest possible size that fits the same atlas dimensions will be used.")]
        public int? Minsize { get; set; } = null;

        [Category("Glyph configuration")]
        [Description("sets the distance field range in EM's.")]
        public int? EmRange { get; set; } = null;

        [Category("Glyph configuration")]
        [Description("(default = 2) – sets the distance field range in output pixels.")]
        public int PxRange { get; set; } = 2;
    }
}
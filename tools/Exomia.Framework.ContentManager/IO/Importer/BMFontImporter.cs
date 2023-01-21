#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable InconsistentNaming

using System.Diagnostics;
using Exomia.Framework.ContentManager.Fonts;
using Exomia.Framework.ContentManager.Fonts.BMFont;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.ContentManager.IO.Importer;

[Importer("BMFont Importer", ".fnt")]
sealed class BMFontImporter : Importer<FontFile>
{
    private const string TEMP_FILE_DIR = "temp";

    private readonly string _bmFontExeLocation;

    public BMFontImporter()
    {
        if (!File.Exists(_bmFontExeLocation = Path.Combine("third-party", "tools", "bmfont", "bmfont64.exe")))
        {
            throw new FileNotFoundException("The 'bmfont64.exe' is missing!", _bmFontExeLocation);
        }
    }

    private static int GetCharCount(string chars)
    {
        int      charCount = 1; //invalid glyph
        string[] groups    = chars.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string group in groups)
        {
            charCount++;
            string[] ranges = group.Split(new[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (ranges.Length == 2)
            {
                int a = int.Parse(ranges[0]);
                int b = int.Parse(ranges[1]);
                if (a > b)
                {
                    throw new ArgumentOutOfRangeException(group);
                }
                charCount += b - a;
            }
        }

        return charCount;
    }

    private static bool CheckFontImageFiles(FontFile fontFile, string fontDirectory)
    {
        return fontFile.Pages!.All(page => File.Exists(Path.Combine(fontDirectory, page.File!)));
    }

    private static string CreateConfig(FontDescription fontDescription,
                                       int             w,
                                       int             h)
    {
        return $@"
# AngelCode Bitmap Font Generator configuration file
fileVersion=1

# font settings
fontName={fontDescription.Name}
fontFile=
charSet=0
fontSize={fontDescription.Size}
aa={(fontDescription.AA ? 1 : 0)}
scaleH=100
useSmoothing=1
isBold={(fontDescription.IsBold ? 1 : 0)}
isItalic={(fontDescription.IsItalic ? 1 : 0)}
useUnicode=1
disableBoxChars=1
outputInvalidCharGlyph=1
dontIncludeKerningPairs=0
useHinting=1
renderFromOutline=0
useClearType=1
autoFitNumPages=0
autoFitFontSizeMin=0
autoFitFontSizeMax=0

# character alignment
paddingDown=0
paddingUp=0
paddingRight=0
paddingLeft=0
spacingHoriz=1
spacingVert=1
useFixedHeight=0
forceZero=0
widthPaddingFactor=0.00

# output file
outWidth={w}
outHeight={h}
outBitDepth=32
fontDescFormat=1
fourChnlPacked=0
textureFormat=png
textureCompression=0
alphaChnl=0
redChnl=4
greenChnl=4
blueChnl=4
invA=0
invR=0
invG=0
invB=0

# outline
outlineThickness=0

# selected chars
chars={fontDescription.Chars}

# imported icon images";
    }

    /// <inheritdoc />
    public override async Task<FontFile?> ImportAsync(Stream            stream,
                                                      ImporterContext   context,
                                                      CancellationToken cancellationToken)
    {
        return await Task.Run(
            () =>
            {
                FontDescription? description = Json.Deserialize<FontDescription>(stream);

                if (description == null)
                {
                    context.AddMessage("Importing item failed! Expected type {0}!", typeof(FontDescription));
                    return null;
                }

                FontStyle fs = FontStyle.Regular;
                if (description.IsBold)
                {
                    fs |= FontStyle.Bold;
                }
                if (description.IsItalic)
                {
                    fs |= FontStyle.Italic;
                }

                using (Font fnt = new Font(description.Name, description.Size, fs))
                {
                    if (string.Compare(
                            fnt.Name, description.Name,
                            StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        context.AddMessage(
                            "Can't import the font '{0:OrangeRed}'! " +
                            "The font doesn't exists on the current system!", description);
                        return null;
                    }
                }

                string tempFileNameLocation =
                    Path.Combine(TEMP_FILE_DIR, $"{description.Size}_{Path.GetRandomFileName()}.fnt");

                string configLocation =
                    Path.Combine("tools", $"config{Thread.CurrentThread.ManagedThreadId}.bmfc");

                int textureWidth =
                    Math2.RoundUpToPowerOfTwo(
                        (int)Math.Sqrt(GetCharCount(description.Chars) * description.Size));

                while (textureWidth <= 16384)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }
                    File.WriteAllText(
                        configLocation,
                        CreateConfig(description, textureWidth, textureWidth));

                    using (Process p = Process.Start(
                               new ProcessStartInfo(
                                   _bmFontExeLocation,
                                   $"-c {configLocation} -o {tempFileNameLocation}")
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
                            FontFile fontFile = FontLoader.Load(tempFileNameLocation);
                            if (fontFile.Common!.Pages == 1)
                            {
                                if (CheckFontImageFiles(fontFile, TEMP_FILE_DIR))
                                {
                                    fontFile.Pages![0].File =
                                        Path.GetFullPath(Path.Combine(TEMP_FILE_DIR, fontFile.Pages![0].File!));
                                    return fontFile;
                                }
                                context.AddMessage("Font page file '{1}' not found!", fontFile.Pages![0].File!);
                                return null;
                            }
                            foreach (string file in Directory.GetFiles(TEMP_FILE_DIR, $"{description.Size}*"))
                            {
                                File.Delete(file);
                            }
                            textureWidth <<= 1;
                            continue;
                        }
                        context.AddMessage("BMFont Importer exited with code {0}!", p.ExitCode);
                        return null;
                    }
                }

                context.AddMessage(
                    "Texture size of {0} exceeded the maximum size of {1}!",
                    textureWidth, 16384);
                return null;
            }, cancellationToken);
    }
}
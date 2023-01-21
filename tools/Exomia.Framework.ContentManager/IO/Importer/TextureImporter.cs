#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.ContentManager.IO.Importer;

[Importer("Texture Importer", ".png", ".jpg", ".jpeg")]
sealed class TextureImporter : StreamImporter<Texture.Texture>
{
    protected override async Task<Texture.Texture?> ImportAsync(Stream stream, ImporterContext context, CancellationToken cancellationToken)
    {
        return await Task.Run(
            () =>
            {
                using Bitmap bitmap = new Bitmap(stream);

                byte[] bytes = new byte[bitmap.Width * bitmap.Height * 4]; // RGBA

                for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    bytes[(x + (y * bitmap.Width)) * 4 + 0] = pixel.R;
                    bytes[(x + (y * bitmap.Width)) * 4 + 1] = pixel.G;
                    bytes[(x + (y * bitmap.Width)) * 4 + 2] = pixel.B;
                    bytes[(x + (y * bitmap.Width)) * 4 + 3] = pixel.A;
                }

                return new Texture.Texture
                {
                    Width  = bitmap.Width,
                    Height = bitmap.Height,
                    Data   = bytes
                };
            }, cancellationToken);
    }
}
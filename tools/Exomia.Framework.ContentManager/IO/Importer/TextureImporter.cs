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
sealed class TextureImporter : Importer<Texture.Texture>
{
    public override async Task<Texture.Texture?> ImportAsync(Stream stream, ImporterContext context, CancellationToken cancellationToken)
    {
        return await Task.Run(
            () =>
            {
                using Bitmap bitmap = new Bitmap(stream);
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
                    return new Texture.Texture
                    {
                        Width  = bitmap.Width,
                        Height = bitmap.Height,
                        Data   = bytes
                    };
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
            }, cancellationToken);
    }
}
﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO.Compression;

namespace Exomia.Framework.Core.Content.Compression;

public static partial class ContentCompressor
{
    private static void BrotliCompress(Stream src, Stream dst)
    {
        using (BrotliStream gs = new BrotliStream(dst, CompressionLevel.Optimal, true))
        {
            CopyStream(src, gs);
        }
    }

    private static void BrotliDecompress(Stream src, Stream dst)
    {
        using (BrotliStream gs = new BrotliStream(src, CompressionMode.Decompress, true))
        {
            CopyStream(gs, dst);
        }
    }
}
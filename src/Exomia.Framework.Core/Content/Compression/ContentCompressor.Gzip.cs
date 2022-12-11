#region License

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
    private static void GzipCompress(Stream src, Stream dst)
    {
        using (GZipStream gs = new GZipStream(dst, CompressionLevel.Optimal, true))
        {
            CopyStream(src, gs);
        }
    }

    private static void GzipDecompress(Stream src, Stream dst)
    {
        using (GZipStream gs = new GZipStream(src, CompressionMode.Decompress, true))
        {
            CopyStream(gs, dst);
        }
    }
}
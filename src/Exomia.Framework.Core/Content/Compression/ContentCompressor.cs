#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Buffers;
using System.IO.Compression;

namespace Exomia.Framework.Core.Content.Compression;

/// <summary> A content compressor. </summary>
public static class ContentCompressor
{
    private const int BUFFER_SIZE = 4096;

    /// <summary> Compress a given <paramref name="src"/> stream into a given <paramref name="dst"/> stream with the given compression mode. </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <param name="compressMode"> (Optional) the compression mode. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void CompressStream(
        Stream       src,
        Stream       dst,
        CompressMode compressMode = CompressMode.Gzip)
    {
        switch (compressMode)
        {
            case CompressMode.None: 
                dst.WriteByte((byte)compressMode);
                CopyStream(src, dst);
                break;
            case CompressMode.Gzip: 
                dst.WriteByte((byte)compressMode);
                GzipCompress(src, dst);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(compressMode), compressMode, "Invalid compression mode!");
        }
    }

    /// <summary>
    ///     Decompress a given <paramref name="src"/> stream into a given <paramref name="dst"/> stream,
    ///     deriving the compression mode from the first byte of the <paramref name="src"/> stream.
    /// </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void DecompressStream(Stream src, Stream dst)
    {
        CompressMode compressMode = (CompressMode)src.ReadByte();
        switch (compressMode)
        {
            case CompressMode.None:
                CopyStream(src, dst);
                break;
            case CompressMode.Gzip:
                GzipDecompress(src, dst);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(CompressMode), compressMode, "Invalid compression mode!");
        }
    }

    private static void CopyStream(Stream src, Stream dst)
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(BUFFER_SIZE);
        try
        {
            int count;
            while ((count = src.Read(buffer, 0, buffer.Length)) > 0)
            {
                dst.Write(buffer, 0, count);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    #region GZIP

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

    #endregion
}
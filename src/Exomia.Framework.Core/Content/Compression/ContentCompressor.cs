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

    /// <summary> Compress a given stream with the given compression mode. </summary>
    /// <param name="src"> the stream to compress. </param>
    /// <param name="dst"> True to dispose the stream. </param>
    /// <param name="compressMode"> (Optional) the compression mode. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void CompressStream(
        Stream       src,
        Stream       dst,
        CompressMode compressMode = CompressMode.Gzip)
    {
        dst.WriteByte((byte)compressMode);

        switch (compressMode)
        {
            case CompressMode.None:
                CopyStream(src, dst);
                break;
            case CompressMode.Gzip:
                GzipCompress(src, dst);
                break;
            default: throw new ArgumentException("No compression method found", nameof(compressMode));
        }
    }

    /// <summary> Decompress a given stream with the given compression mode. </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <returns> <c> true </c> if successfully decompressed the stream; <c> false </c> otherwise. </returns>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void DecompressStream(Stream src, Stream dst)
    {
        switch ((CompressMode)src.ReadByte())
        {
            case CompressMode.None:
                CopyStream(src, dst);
                break;
            case CompressMode.Gzip:
                GzipDecompress(src, dst);
                break;

            default: throw new ArgumentException("No compression method found", nameof(src));
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
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Buffers;

namespace Exomia.Framework.Core.Content.Compression;

/// <summary> A content compressor. </summary>
public static partial class ContentCompressor
{
    private const int BUFFER_SIZE = 4096;

    /// <summary> Compress a given <paramref name="src" /> stream into a given <paramref name="dst" /> stream with the given compression mode. </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <param name="compressMode"> (Optional) the compression mode. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void CompressStream(
        Stream       src,
        Stream       dst,
        CompressMode compressMode = CompressMode.Deflate)
    {
        switch (compressMode)
        {
            case CompressMode.Deflate:
                dst.WriteByte((byte)compressMode);
                DeflateCompress(src, dst);
                break;
            case CompressMode.Gzip:
                dst.WriteByte((byte)compressMode);
                GzipCompress(src, dst);
                break;
            case CompressMode.Brotli:
                dst.WriteByte((byte)compressMode);
                BrotliCompress(src, dst);
                break;
            case CompressMode.None:
                dst.WriteByte((byte)compressMode);
                CopyStream(src, dst);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(compressMode), compressMode, "Invalid compression mode!");
        }
    }

    /// <summary>
    ///     Decompress a given <paramref name="src" /> stream into a given <paramref name="dst" /> stream,
    ///     deriving the compression mode from the first byte of the <paramref name="src" /> stream.
    /// </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void DecompressStream(Stream src, Stream dst)
    {
        DecompressStream(src, dst, (CompressMode)src.ReadByte());
    }

    /// <summary>
    ///     Decompress a given <paramref name="src" /> stream into a given <paramref name="dst" /> stream,
    ///     deriving the compression mode from the first byte of the <paramref name="src" /> stream.
    /// </summary>
    /// <param name="src"> The source stream. </param>
    /// <param name="dst"> The destination stream. </param>
    /// <param name="compressMode"> The compress mode to use for decompression. </param>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static void DecompressStream(Stream src, Stream dst, CompressMode compressMode)
    {
        switch (compressMode)
        {
            case CompressMode.Deflate:
                DeflateDecompress(src, dst);
                break;
            case CompressMode.Gzip:
                GzipDecompress(src, dst);
                break;
            case CompressMode.Brotli:
                BrotliDecompress(src, dst);
                break;
            case CompressMode.None:
                CopyStream(src, dst);
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
}
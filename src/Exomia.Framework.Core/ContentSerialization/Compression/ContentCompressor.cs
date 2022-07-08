#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO.Compression;

namespace Exomia.Framework.Core.ContentSerialization.Compression;

/// <summary> A content compressor. </summary>
public static class ContentCompressor
{
    /// <summary> The default compressed e1 extension. </summary>
    public const string DEFAULT_COMPRESSED_EXTENSION = ".e1";

    private const           int    BUFFER_SIZE   = 2048;
    private static readonly byte[] s_magicHeader = { 64, 101, 120, 49 };

    /// <summary> Compress a given stream with the given compression mode. </summary>
    /// <param name="stream">       the stream to compress. </param>
    /// <param name="streamOut">    [out] than finished the compressed out stream. </param>
    /// <param name="compressMode"> (Optional) the compression mode. </param>
    /// <returns> <c>true</c> if successfully compressed the stream; <c>false</c> otherwise. </returns>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static bool CompressStream(Stream       stream,
                                      out Stream   streamOut,
                                      CompressMode compressMode = CompressMode.Gzip)
    {
        try
        {
            stream.Position = 0;

            streamOut = new MemoryStream();
            streamOut.Write(s_magicHeader, 0, s_magicHeader.Length);
            streamOut.WriteByte((byte)compressMode);

            switch (compressMode)
            {
                case CompressMode.Gzip:
                    GzipCompress(stream, streamOut);
                    break;
                default: throw new ArgumentException("No compression method found", nameof(compressMode));
            }
        }
        catch
        {
            streamOut = null!;
            return false;
        }
        return true;
    }

    /// <summary> Decompress a given stream with the given compression mode. </summary>
    /// <param name="stream">    the stream to compress. </param>
    /// <param name="streamOut"> [out] than finished the decompressed out stream. </param>
    /// <returns> <c>true</c> if successfully decompressed the stream; <c>false</c> otherwise. </returns>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static bool DecompressStream(Stream stream, out Stream streamOut)
    {
        try
        {
            stream.Position = 0;

            byte[] buffer = new byte[s_magicHeader.Length];
            if (stream.Read(buffer, 0, buffer.Length) != s_magicHeader.Length
                && !s_magicHeader.SequenceEqual(buffer))
            {
                streamOut = null!;
                return false;
            }

            switch ((CompressMode)stream.ReadByte())
            {
                case CompressMode.Gzip:
                    GzipDecompress(stream, out streamOut);
                    break;
                default: throw new ArgumentException("No compression method found", nameof(stream));
            }
        }
        catch
        {
            streamOut = null!;
            return false;
        }
        return true;
    }

    #region GZIP

    private static void GzipCompress(Stream stream, Stream streamOut)
    {
        using (GZipStream gs = new GZipStream(streamOut, CompressionLevel.Optimal, true))
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int    count;
            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                gs.Write(buffer, 0, count);
            }
        }
        streamOut.Position = 0;
    }

    private static void GzipDecompress(Stream stream, out Stream streamOut)
    {
        streamOut = new MemoryStream();
        using (GZipStream gs = new GZipStream(stream, CompressionMode.Decompress, true))
        {
            byte[] buffer = new byte[BUFFER_SIZE];
            int    count;
            while ((count = gs.Read(buffer, 0, buffer.Length)) > 0)
            {
                streamOut.Write(buffer, 0, count);
            }
        }
        streamOut.Position = 0;
    }

    #endregion
}
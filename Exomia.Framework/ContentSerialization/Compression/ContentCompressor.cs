#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

namespace Exomia.Framework.ContentSerialization.Compression
{
    /// <summary>
    ///     CompressMode
    /// </summary>
    public enum CompressMode : byte
    {
        /// <summary>
        ///     gzip (default)
        /// </summary>
        Gzip = 1
    }

    /// <summary>
    ///     ContentCompressor class
    /// </summary>
    public static class ContentCompressor
    {
        /// <summary>
        ///     the default compressed e1 extension
        /// </summary>
        public const string DEFAULT_COMPRESSED_EXTENSION = ".e1";

        private const int BUFFER_SIZE = 2048;

        /// <summary>
        ///     the magic header for the compressed e1 extension
        /// </summary>
        private static readonly byte[] s_magicHeader = { 64, 101, 120, 49 };

        /// <summary>
        ///     compress a given stream with the given compression mode
        /// </summary>
        /// <param name="stream">the stream to compress</param>
        /// <param name="streamOut">than finished the compressed out stream</param>
        /// <param name="compressMode">the compression mode</param>
        /// <returns><c>true</c> if successfully compressed the stream; <c>false</c> otherwise.</returns>
        public static bool CompressStream(Stream stream, out Stream streamOut,
            CompressMode compressMode = CompressMode.Gzip)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            streamOut = null;
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
                    default: throw new ArgumentException("no compression method found", nameof(compressMode));
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        ///     decompress a given stream with the given compression mode
        /// </summary>
        /// <param name="stream">the stream to compress</param>
        /// <param name="streamOut">than finished the decompressed out stream</param>
        /// <returns><c>true</c> if successfully decompressed the stream; <c>false</c> otherwise.</returns>
        public static bool DecompressStream(Stream stream, out Stream streamOut)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            streamOut = null;
            try
            {
                stream.Position = 0;

                byte[] buffer = new byte[s_magicHeader.Length];
                if (stream.Read(buffer, 0, buffer.Length) != s_magicHeader.Length
                    && !s_magicHeader.SequenceEqual(buffer)) { return false; }

                switch ((CompressMode)stream.ReadByte())
                {
                    case CompressMode.Gzip:
                        GzipDecompress(stream, out streamOut);
                        break;
                    default: throw new ArgumentException("no compression method found", nameof(stream));
                }
            }
            catch { return false; }
            return true;
        }

        #region GZIP

        private static void GzipCompress(Stream stream, Stream streamOut)
        {
            using (GZipStream gs = new GZipStream(streamOut, CompressionLevel.Optimal, true))
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int count;
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
                int count;
                while ((count = gs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    streamOut.Write(buffer, 0, count);
                }
            }
            streamOut.Position = 0;
        }

        #endregion
    }
}
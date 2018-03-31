#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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

using System;
using System.IO;
using System.IO.Compression;

namespace Exomia.Framework.ContentSerialization.Compression
{
    /// <summary>
    ///     CompressMode
    /// </summary>
    public enum CompressMode
    {
        /// <summary>
        ///     gzip (default)
        /// </summary>
        GZIP,

        /// <summary>
        ///     not supported
        /// </summary>
        TGC
    }

    /// <summary>
    ///     ContentCompressor class
    /// </summary>
    public static class ContentCompressor
    {
        #region Variables

        /// <summary>
        ///     the fefault compressed ds extension
        /// </summary>
        public const string DEFAULT_COMPRESSED_EXTENSION = ".ds2";

        private const int BUFFER_SIZE = 2048;

        #endregion

        #region Methods

        /// <summary>
        ///     compress a given stream with the given compression mode
        /// </summary>
        /// <param name="stream">the stream to compress</param>
        /// <param name="streamOut">than finished the compressed out stream</param>
        /// <param name="mode">the compression mode</param>
        /// <returns><c>true</c> if successfully compressed the stream; <c>false</c> otherwise.</returns>
        public static bool CompressStream(Stream stream, out Stream streamOut, CompressMode mode = CompressMode.GZIP)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            streamOut = null;
            try
            {
                stream.Position = 0;
                switch (mode)
                {
                    case CompressMode.GZIP:
                        GzipCompress(stream, out streamOut);
                        return true;
                    case CompressMode.TGC:
                        TgcCompress(stream, out streamOut);
                        return true;
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
        /// <param name="mode">the compression mode</param>
        /// <returns><c>true</c> if successfully decompressed the stream; <c>false</c> otherwise.</returns>
        public static bool DecompressStream(Stream stream, out Stream streamOut, CompressMode mode = CompressMode.GZIP)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }
            streamOut = null;
            try
            {
                stream.Position = 0;
                switch (mode)
                {
                    case CompressMode.GZIP:
                        GzipDecompress(stream, out streamOut);
                        return true;
                    case CompressMode.TGC:
                        TgcDecompress(stream, out streamOut);
                        return true;
                }
            }
            catch { return false; }
            return true;
        }

        #endregion

        #region GZIP

        private static void GzipCompress(Stream stream, out Stream streamOut)
        {
            streamOut = new MemoryStream();
            using (GZipStream gs = new GZipStream(streamOut, CompressionLevel.Optimal, true))
            {
                byte[] buffer = new byte[BUFFER_SIZE];
                int count = -1;
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
                int count = -1;
                while ((count = gs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    streamOut.Write(buffer, 0, count);
                }
            }
            streamOut.Position = 0;
        }

        #endregion

        #region TGC

        private static void TgcCompress(Stream stream, out Stream streamOut)
        {
            throw new NotSupportedException();
        }

        private static void TgcDecompress(Stream stream, out Stream streamOut)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
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
        #region Constants

        /// <summary>
        ///     the fefault compressed ds extension
        /// </summary>
        public const string DEFAULT_COMPRESSED_EXTENSION = ".ds2";

        private const int BUFFER_SIZE = 2048;

        #endregion

        #region Variables

        #region Statics

        #endregion

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Constructors

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

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
                        GZIPCompress(stream, out streamOut);
                        return true;
                    case CompressMode.TGC:
                        TGCCompress(stream, out streamOut);
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
                        GZIPDecompress(stream, out streamOut);
                        return true;
                    case CompressMode.TGC:
                        TGCDecompress(stream, out streamOut);
                        return true;
                }
            }
            catch { return false; }
            return true;
        }

        #region GZIP

        private static void GZIPCompress(Stream stream, out Stream streamOut)
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

        private static void GZIPDecompress(Stream stream, out Stream streamOut)
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

        private static void TGCCompress(Stream stream, out Stream streamOut)
        {
            throw new NotSupportedException();
        }

        private static void TGCDecompress(Stream stream, out Stream streamOut)
        {
            throw new NotSupportedException();
        }

        #endregion

        #endregion

        #endregion
    }
}
using System.IO;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.ContentSerialization.Compression;
using Exomia.Framework.Security;

namespace Exomia.Framework.Content.Resolver
{
    internal sealed class DSFileStreamContentResolver : IContentResolver
    {
        #region Constructors

        #region Statics

        #endregion

        #endregion

        #region Constants

        #endregion

        #region Variables

        #region Statics

        #endregion

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

        public bool Exists(string assetName)
        {
            if (!Path.HasExtension(assetName))
            {
                if (File.Exists(assetName + ContentSerializer.DEFAULT_EXTENSION))
                {
                    return true;
                }
                if (File.Exists(assetName + ExomiaCryptography.DEFAULT_CRYPT_EXTENSION))
                {
                    return true;
                }
                if (File.Exists(assetName + ContentCompressor.DEFAULT_COMPRESSED_EXTENSION))
                {
                    return true;
                }
            }
            else
            {
                return File.Exists(assetName);
            }

            return false;
        }

        public Stream Resolve(string assetName)
        {
            if (!Path.HasExtension(assetName))
            {
                if (File.Exists(assetName + ContentSerializer.DEFAULT_EXTENSION))
                {
                    assetName += ContentSerializer.DEFAULT_EXTENSION;
                }
                else if (File.Exists(assetName + ExomiaCryptography.DEFAULT_CRYPT_EXTENSION))
                {
                    assetName += ExomiaCryptography.DEFAULT_CRYPT_EXTENSION;
                }
                else if (File.Exists(assetName + ContentCompressor.DEFAULT_COMPRESSED_EXTENSION))
                {
                    assetName += ContentCompressor.DEFAULT_COMPRESSED_EXTENSION;
                }
                else
                {
                    return null;
                }
            }

            if (Path.GetExtension(assetName).Equals(ExomiaCryptography.DEFAULT_CRYPT_EXTENSION))
            {
                if (ExomiaCryptography.Decrypt(assetName, out Stream stream))
                {
                    return stream;
                }
            }
            else if (Path.GetExtension(assetName).Equals(ContentCompressor.DEFAULT_COMPRESSED_EXTENSION))
            {
                if (ExomiaCryptography.Decrypt(assetName, out Stream stream))
                {
                    if (ContentCompressor.DecompressStream(stream, out Stream stream2, CompressMode.GZIP))
                    {
                        return stream2;
                    }
                }
            }
            else
            {
                return new FileStream(assetName, FileMode.Open, FileAccess.Read);
            }

            return null;
        }

        #endregion
    }
}
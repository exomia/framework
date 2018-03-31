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

using System.IO;
using Exomia.Framework.ContentSerialization;
using Exomia.Framework.ContentSerialization.Compression;
using Exomia.Framework.Security;

namespace Exomia.Framework.Content.Resolver
{
    /// <inheritdoc />
    internal sealed class DSFileStreamContentResolver : IContentResolver
    {
        #region Methods

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
#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;
using Exomia.Framework.ContentSerialization.Compression;

namespace Exomia.Framework.Content.Resolver
{
    /// <summary>
    ///     A 1 file stream content resolver. This class cannot be inherited.
    /// </summary>
    [ContentResolver(int.MinValue)]
    sealed class E1FileStreamContentResolver : IContentResolver
    {
        /// <inheritdoc />
        public bool Exists(string assetName)
        {
            return Path.GetExtension(assetName) == ContentCompressor.DEFAULT_COMPRESSED_EXTENSION &&
                   File.Exists(assetName);
        }

        /// <inheritdoc />
        public Stream Resolve(string assetName)
        {
            using (FileStream stream = new FileStream(assetName, FileMode.Open, FileAccess.Read))
            {
                return ContentCompressor.DecompressStream(stream, out Stream stream2) ? stream2 : null;
            }
        }
    }
}
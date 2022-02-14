#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.ContentSerialization.Compression;

namespace Exomia.Framework.Core.Content.Resolver
{
    [ContentResolver(int.MinValue)]
    internal sealed class E1FileStreamContentResolver : IContentResolver
    {
        /// <inheritdoc />
        public bool Exists(string assetName)
        {
            return Path.GetExtension(assetName) == ContentCompressor.DEFAULT_COMPRESSED_EXTENSION &&
                   File.Exists(assetName);
        }

        /// <inheritdoc />
        public Stream? Resolve(string assetName)
        {
            using FileStream stream = new FileStream(assetName, FileMode.Open, FileAccess.Read);
            return ContentCompressor.DecompressStream(stream, out Stream stream2)
                ? stream2
                : null;
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Reflection;
using Exomia.Framework.Core.ContentSerialization.Compression;

namespace Exomia.Framework.Core.Content.Resolver.EmbeddedResource
{
    /// <summary>
    ///     A 1 embedded resource stream resolver.
    /// </summary>
    [ContentResolver(int.MinValue)]
    class E1EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
    {
        /// <inheritdoc />
        public bool Exists(Type assetType, string assetName, out Assembly assembly)
        {
            if (Path.GetExtension(assetName) == ContentCompressor.DEFAULT_COMPRESSED_EXTENSION)
            {
                return EmbeddedResourceStreamResolver.ExistsInternal(assetType, assetName, out assembly);
            }

            assembly = null!;
            return false;
        }

        /// <inheritdoc />
        public Stream? Resolve(Assembly assembly, string assetName)
        {
            using Stream stream = EmbeddedResourceStreamResolver.GetManifestResourceStreamInternal(assembly, assetName);
            return ContentCompressor.DecompressStream(stream, out Stream stream2) ? stream2 : null;
        }
    }
}
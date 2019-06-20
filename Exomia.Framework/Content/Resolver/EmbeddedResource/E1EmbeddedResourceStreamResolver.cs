#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Reflection;
using Exomia.Framework.ContentSerialization.Compression;

namespace Exomia.Framework.Content.Resolver.EmbeddedResource
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
                assembly = assetType.Assembly;
                string name = $"{assembly.GetName().Name}.{assetName}";
                foreach (string resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName == name)
                    {
                        return true;
                    }
                }

                assembly = Assembly.GetEntryAssembly();
                name     = $"{assembly.GetName().Name}.{assetName}";
                foreach (string resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName == name)
                    {
                        return true;
                    }
                }
            }
            assembly = null;
            return false;
        }

        /// <inheritdoc />
        public Stream Resolve(Assembly assembly, string assetName)
        {
            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{assetName}"))
            {
                return ContentCompressor.DecompressStream(stream, out Stream stream2) ? stream2 : null;
            }
        }
    }
}
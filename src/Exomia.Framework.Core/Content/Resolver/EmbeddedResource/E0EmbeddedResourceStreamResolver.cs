#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;
using Exomia.Framework.Core.ContentSerialization;

namespace Exomia.Framework.Core.Content.Resolver.EmbeddedResource
{
    [ContentResolver(int.MinValue + 1)]
    internal class E0EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
    {
        /// <inheritdoc />
        public bool Exists(Type assetType, string assetName, out Assembly assembly)
        {
            if (Path.GetExtension(assetName) == ContentSerializer.DEFAULT_EXTENSION)
            {
                return EmbeddedResourceStreamResolver.ExistsInternal(assetType, assetName, out assembly);
            }

            assembly = null!;
            return false;
        }

        /// <inheritdoc />
        public Stream? Resolve(Assembly assembly, string assetName)
        {
            return EmbeddedResourceStreamResolver.GetManifestResourceStreamInternal(assembly, assetName);
        }
    }
}
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
using Exomia.Framework.ContentSerialization;

namespace Exomia.Framework.Content.Resolver.EmbeddedResource
{
    /// <summary>
    ///     A 0 embedded resource stream resolver.
    /// </summary>
    [ContentResolver(int.MinValue + 1)]
    class E0EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
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
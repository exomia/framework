﻿#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Reflection;

namespace Exomia.Framework.Content.Resolver.EmbeddedResource
{
    /// <summary>
    ///     An embedded resource stream resolver.
    /// </summary>
    [ContentResolver(int.MaxValue)]
    class EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
    {
        /// <inheritdoc />
        public bool Exists(Type assetType, string assetName, out Assembly assembly)
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

            assembly = null;
            return false;
        }

        /// <inheritdoc />
        public Stream Resolve(Assembly assembly, string assetName)
        {
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{assetName}");
        }
    }
}
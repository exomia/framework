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

namespace Exomia.Framework.Content.Resolver.EmbeddedResource
{
    /// <summary>
    ///     An embedded resource stream resolver.
    /// </summary>
    [ContentResolver(int.MaxValue)]
    class EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
    {
        /// <inheritdoc/>
        public bool Exists(Type assetType, string assetName, out Assembly assembly)
        {
            return ExistsInternal(assetType, assetName, out assembly);
        }

        /// <inheritdoc/>
        public Stream Resolve(Assembly assembly, string assetName)
        {
            return GetManifestResourceStreamInternal(assembly, assetName);
        }

        /// <summary>
        ///     Gets the asset name.
        /// </summary>
        /// <param name="assetName"> Name of the asset. </param>
        /// <param name="assembly">  [out] The assembly. </param>
        /// <returns>
        ///     The asset name.
        /// </returns>
        private static string GetAssetName(string assetName, Assembly assembly)
        {
            return $"{assembly.GetName().Name}.{assetName}";
        }

        /// <summary>
        ///     Exists internal.
        /// </summary>
        /// <param name="assetType"> Type of the asset. </param>
        /// <param name="assetName"> Name of the asset. </param>
        /// <param name="assembly">  [out] The assembly. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal static bool ExistsInternal(Type assetType, string assetName, out Assembly assembly)
        {
            assembly = assetType.Assembly;
            string name = GetAssetName(assetName, assembly);
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.Equals(name))
                {
                    return true;
                }
            }

            assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            name     = GetAssetName(assetName, assembly);
            foreach (string resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.Equals(name))
                {
                    return true;
                }
            }

            assembly = null!;
            return false;
        }

        /// <summary>
        ///     Gets manifest resource stream internal.
        /// </summary>
        /// <param name="assembly">  [out] The assembly. </param>
        /// <param name="assetName"> Name of the asset. </param>
        /// <returns>
        ///     The manifest resource stream internal.
        /// </returns>
        internal static Stream GetManifestResourceStreamInternal(Assembly assembly, string assetName)
        {
            return assembly.GetManifestResourceStream(GetAssetName(assetName, assembly));
        }
    }
}
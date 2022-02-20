#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;

namespace Exomia.Framework.Core.Content.Resolver.EmbeddedResource;

[ContentResolver(int.MaxValue)]
internal class EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
{
    /// <inheritdoc />
    public bool Exists(Type assetType, string assetName, out Assembly assembly)
    {
        return ExistsInternal(assetType, assetName, out assembly);
    }

    /// <inheritdoc />
    public Stream? Resolve(Assembly assembly, string assetName)
    {
        return GetManifestResourceStreamInternal(assembly, assetName);
    }

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

    internal static Stream? GetManifestResourceStreamInternal(Assembly assembly, string assetName)
    {
        return assembly.GetManifestResourceStream(GetAssetName(assetName, assembly));
    }

    private static string GetAssetName(string assetName, Assembly assembly)
    {
        return $"{assembly.GetName().Name}.{assetName}";
    }
}
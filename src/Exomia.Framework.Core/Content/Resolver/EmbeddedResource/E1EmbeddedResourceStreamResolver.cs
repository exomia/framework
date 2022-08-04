#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Exomia.Framework.Core.Content.Resolver.EmbeddedResource;

[ContentResolver(int.MinValue)]
sealed class E1EmbeddedResourceStreamResolver : IEmbeddedResourceResolver
{
    /// <inheritdoc />
    public bool Exists(Type assetType, string assetName, [NotNullWhen(true)] out Assembly? assembly)
    {
        if (Path.GetExtension(assetName) == E1.EXTENSION_NAME)
        {
            return ExistsInternal(assetType, assetName, out assembly);
        }

        assembly = null;
        return false;
    }

    /// <inheritdoc />
    public Stream? Resolve(Assembly assembly, string assetName)
    {
        Stream? stream = assembly.GetManifestResourceStream(GetAssetName(assetName, assembly));
        if (stream == null)
        {
            return null;
        }

        byte[] buffer = new byte[E1.MagicHeader.Length];
        if (stream.Read(buffer, 0, buffer.Length) != E1.MagicHeader.Length
         || !E1.MagicHeader.SequenceEqual(buffer))
        {
            stream.Dispose();
            return null;
        }

        return stream;
    }

    private static bool ExistsInternal(Type assetType, string assetName, [NotNullWhen(true)] out Assembly? assembly)
    {
        assembly = assetType.Assembly;
        string name = GetAssetName(assetName, assembly);
        if (assembly.GetManifestResourceNames().Any(resourceName => resourceName.Equals(name)))
        {
            return true;
        }

        assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
        name     = GetAssetName(assetName, assembly);
        if (assembly.GetManifestResourceNames().Any(resourceName => resourceName.Equals(name)))
        {
            return true;
        }

        assembly = null;
        return false;
    }

    private static string GetAssetName(string assetName, Assembly assembly)
    {
        return $"{assembly.GetName().Name}.{assetName}";
    }
}
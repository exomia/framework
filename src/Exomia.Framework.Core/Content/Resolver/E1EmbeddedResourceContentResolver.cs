#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Exomia.Framework.Core.Content.E1;

namespace Exomia.Framework.Core.Content.Resolver;

[ContentResolver(int.MinValue)]
sealed class E1EmbeddedResourceContentResolver : IEmbeddedResourceContentResolver
{
    /// <inheritdoc />
    public bool Exists(Type assetType, string assetName, [NotNullWhen(true)] out Assembly? assembly)
    {
        if (Path.GetExtension(assetName) == E1Protocol.EXTENSION_NAME)
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

        byte[] buffer = new byte[E1Protocol.MagicHeader.Length];
        if (stream.Read(buffer, 0, buffer.Length) != E1Protocol.MagicHeader.Length
         || !E1Protocol.MagicHeader.SequenceEqual(buffer))
        {
            stream.Dispose();
            return null;
        }

        return stream;
    }

    private static bool ExistsInternal(Type assetType, string assetName, [NotNullWhen(true)] out Assembly? assembly)
    {
        bool CheckAssembly(Assembly a)
        {
            string name = GetAssetName(assetName, a);
            return a.GetManifestResourceNames().Any(resourceName => resourceName.Equals(name));
        }
        
        if (CheckAssembly(assetType.Assembly))
        {
            assembly = assetType.Assembly;
            return true;
        }
        
        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (CheckAssembly(a))
            {
                assembly = a;
                return true;
            }
        }

        assembly = null;
        return false;
    }

    private static string GetAssetName(string assetName, Assembly assembly)
    {
        return $"{assembly.GetName().Name}.{assetName}";
    }
}
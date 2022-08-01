#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content.Resolver;

[ContentResolver(int.MinValue)]
internal sealed class E1FileStreamContentResolver : IContentResolver
{
    /// <inheritdoc />
    public bool Exists(string assetName)
    {
        return Path.GetExtension(assetName) == E1.EXTENSION_NAME &&
            File.Exists(assetName);
    }

    /// <inheritdoc />
    public Stream? Resolve(string assetName)
    {
        FileStream stream = new FileStream(assetName, FileMode.Open, FileAccess.Read);

        byte[] buffer = new byte[E1.MagicHeader.Length];
        if (stream.Read(buffer, 0, buffer.Length) != E1.MagicHeader.Length
            || !E1.MagicHeader.SequenceEqual(buffer))
        {
            stream.Dispose();
            return null;
        }

        return stream;
    }
}
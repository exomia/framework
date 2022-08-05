#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Protocols;

namespace Exomia.Framework.Core.Content.Resolver;

[ContentResolver(int.MinValue)]
sealed class E1FileStreamContentResolver : IContentResolver
{
    /// <inheritdoc />
    public Type ProtocolType
    {
        get { return typeof(E1Protocol); }
    }

    /// <inheritdoc />
    public bool Exists(string assetName)
    {
        return Path.GetExtension(assetName) == E1Protocol.EXTENSION_NAME &&
            File.Exists(assetName);
    }

    /// <inheritdoc />
    public Stream? Resolve(string assetName)
    {
        FileStream stream = new FileStream(assetName, FileMode.Open, FileAccess.Read);

        byte[] buffer = new byte[E1Protocol.MagicHeader.Length];
        if (stream.Read(buffer, 0, buffer.Length) != E1Protocol.MagicHeader.Length
         || !E1Protocol.MagicHeader.SequenceEqual(buffer))
        {
            stream.Dispose();
            return null;
        }

        return stream;
    }
}
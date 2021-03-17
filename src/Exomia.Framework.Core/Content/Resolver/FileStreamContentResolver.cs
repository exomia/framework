#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;

namespace Exomia.Framework.Core.Content.Resolver
{
    /// <summary>
    ///     A file stream content resolver. This class cannot be inherited.
    /// </summary>
    [ContentResolver(int.MaxValue)]
    sealed class FileStreamContentResolver : IContentResolver
    {
        /// <inheritdoc />
        public bool Exists(string assetName)
        {
            return File.Exists(assetName);
        }

        /// <inheritdoc />
        public Stream Resolve(string assetName)
        {
            return new FileStream(assetName, FileMode.Open, FileAccess.Read);
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;

namespace Exomia.Framework.ContentManager.Extensions;

static class FileSystemExtensions
{
    public static void DeleteIfExists(this FileInfo info)
    {
        if (info.Exists)
        {
            info.Delete();
        }
    }

    public static void DeleteIfExists(this DirectoryInfo info)
    {
        if (info.Exists)
        {
            info.Delete(true);
        }
    }
}
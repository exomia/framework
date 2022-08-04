#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.IO;

interface IImporter
{
    Type OutType { get; }

    Task<object?> ImportAsync(Stream stream, ImporterContext context, CancellationToken cancellationToken);
}
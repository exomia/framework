#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Exomia.Framework.ContentManager.IO;

interface IImporter
{
    Type OutType { get; }

    Task<object?> ImportAsync(Stream stream, ImporterContext context, CancellationToken cancellationToken);
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Threading;

namespace Exomia.Framework.ContentManager.IO
{
    interface IImporter
    {
        Type OutType { get; }

        object? Import(byte[] data, ImporterContext context, CancellationToken cancellationToken);
    }
}
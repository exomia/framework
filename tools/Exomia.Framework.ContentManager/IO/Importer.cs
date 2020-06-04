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
    abstract class Importer<T> : IImporter
        where T : class
    {
        /// <inheritdoc />
        public Type OutType
        {
            get { return typeof(T); }
        }

        object? IImporter.Import(byte[] data, ImporterContext context, CancellationToken cancellationToken)
        {
            return Import(data, context, cancellationToken);
        }

        public abstract T? Import(byte[] data, ImporterContext context, CancellationToken cancellationToken);
    }
}
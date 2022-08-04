#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.IO;

abstract class Importer<T> : IImporter
    where T : class
{
    /// <inheritdoc />
    public Type OutType
    {
        get { return typeof(T); }
    }

    async Task<object?> IImporter.ImportAsync(Stream            stream,
                                              ImporterContext   context,
                                              CancellationToken cancellationToken)
    {
        return await ImportAsync(stream, context, cancellationToken);
    }

    public abstract Task<T?> ImportAsync(Stream            stream,
                                         ImporterContext   context,
                                         CancellationToken cancellationToken);
}
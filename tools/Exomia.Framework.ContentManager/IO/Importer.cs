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

    /// <inheritdoc />
    public virtual object? CreateImporterSettings()
    {
        return null;
    }

    async Task<object?> IImporter.ImportAsync(
        ImporterContext   context,
        CancellationToken cancellationToken)
    {
        return await ImportAsync(context, cancellationToken);
    }

    protected abstract Task<T?> ImportAsync(
        ImporterContext   context,
        CancellationToken cancellationToken);
}

abstract class StreamImporter<T> : IImporter
    where T : class
{
    /// <inheritdoc />
    public Type OutType
    {
        get { return typeof(T); }
    }

    /// <inheritdoc />
    public virtual object? CreateImporterSettings()
    {
        return null;
    }

    async Task<object?> IImporter.ImportAsync(
        ImporterContext   context,
        CancellationToken cancellationToken)
    {
        using (FileStream fs = new FileStream(context.FileName, FileMode.Open, FileAccess.Read))
        {
            return await ImportAsync(fs, context, cancellationToken);
        }
    }

    protected abstract Task<T?> ImportAsync(
        Stream            fs,
        ImporterContext   context,
        CancellationToken cancellationToken);
}
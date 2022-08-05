#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;
using Exomia.Framework.Core.Content.Exceptions;
using Exomia.Framework.Core.Content.Resolver;

namespace Exomia.Framework.Core.Content;

public sealed partial class ContentManager
{
    private Stream ResolveStream(string assetName)
    {
        List<IContentResolver> resolvers;
        lock (_registeredContentResolvers)
        {
            resolvers = new List<IContentResolver>(_registeredContentResolvers);
        }

        if (resolvers.Count == 0)
        {
            throw new InvalidOperationException(
                $"No {nameof(IContentResolver)} registered to this content manager");
        }

        Exception? lastException = null;
        foreach (IContentResolver contentResolver in resolvers)
        {
            try
            {
                if (contentResolver.Exists(assetName))
                {
                    Stream? stream = contentResolver.Resolve(assetName);
                    if (stream != null) { return stream; }
                }
            }
            catch (Exception ex) { lastException = ex; }
        }

        throw new AssetNotFoundException(assetName, lastException);
    }

    private Stream ResolveEmbeddedResourceStream(Type assetType, string assetName)
    {
        List<IEmbeddedResourceContentResolver> resolvers;
        lock (_registeredContentResolvers)
        {
            resolvers = new List<IEmbeddedResourceContentResolver>(_registeredEmbeddedResourceResolvers);
        }

        if (resolvers.Count == 0)
        {
            throw new InvalidOperationException(
                $"No {nameof(IEmbeddedResourceContentResolver)} registered to this content manager");
        }

        Exception? lastException = null;
        foreach (IEmbeddedResourceContentResolver contentResolver in resolvers)
        {
            try
            {
                if (contentResolver.Exists(assetType, assetName, out Assembly? assembly))
                {
                    Stream? stream = contentResolver.Resolve(assembly, assetName);
                    if (stream != null) { return stream; }
                }
            }
            catch (Exception ex) { lastException = ex; }
        }

        throw new AssetNotFoundException(assetName, lastException);
    }

    private object LoadAssetWithDynamicContentReader(Type assetType, string assetName, Stream stream)
    {
        ContentReaderParameters parameters = new(assetName, assetType, stream);

        try
        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (!_registeredContentReaders.TryGetValue(assetType, out IContentReader? contentReader))
            {
                lock (_registeredContentReaderFactories)
                {
                    if (!_registeredContentReaders.TryGetValue(assetType, out contentReader))
                    {
                        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                        foreach (IContentReaderFactory factory in _registeredContentReaderFactories)
                        {
                            if (factory.TryCreate(assetType, out contentReader))
                            {
                                lock (_registeredContentReaders)
                                {
                                    _registeredContentReaders.Add(assetType, contentReader);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (contentReader == null)
            {
                throw new NotSupportedException(
                    $"Type [{assetType.FullName}] doesn't provide a {nameof(ContentReadableAttribute)}, and there is no registered content reader/factory for it.");
            }

            return contentReader.ReadContent(this, ref parameters)
             ?? throw new NotSupportedException(
                    $"Registered {nameof(IContentReader)} of type [{contentReader.GetType()}] fails to load content of type [{assetType.FullName}] from file [{assetName}].");
        }
        finally
        {
            if (!parameters.KeepStreamOpen)
            {
                stream.Dispose();
            }
        }
    }

    private object? GetAssetLocker(AssetKey assetKey, bool create)
    {
        // ReSharper disable once InconsistentlySynchronizedField
        if (!_assetLockers.TryGetValue(assetKey, out object? assetLockerRead) && create)
        {
            lock (_assetLockers)
            {
                if (!_assetLockers.TryGetValue(assetKey, out assetLockerRead))
                {
                    _assetLockers.Add(assetKey, assetLockerRead = new object());
                }
            }
        }
        return assetLockerRead;
    }
}
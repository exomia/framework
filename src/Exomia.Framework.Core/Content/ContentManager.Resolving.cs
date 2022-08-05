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
    private Stream ResolveStream(string assetName, out Type protocolType)
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
                    if (stream != null)
                    {
                        protocolType = contentResolver.ProtocolType;
                        return stream;
                    }
                }
            }
            catch (Exception ex) { lastException = ex; }
        }

        throw new AssetNotFoundException(assetName, lastException);
    }

    private Stream ResolveEmbeddedResourceStream(Type assetType, string assetName, out Type protocolType)
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
                    if (stream != null)
                    {
                        protocolType = contentResolver.ProtocolType;
                        return stream;
                    }
                }
            }
            catch (Exception ex) { lastException = ex; }
        }

        throw new AssetNotFoundException(assetName, lastException);
    }

    private object LoadAsset(Type assetType, string assetName, Stream stream, Type protocolType)
    {
        object LoadAssetFromContentReader(IContentReader contentReader)
        {
            ContentReaderParameters parameters = new(assetName, assetType, stream);
            try
            {
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

        object LoadAssetFromFactory()
        {
            lock (_registeredContentReaders)
            {
                if (_registeredContentReaders.TryGetValue(protocolType, out Dictionary<Type, IContentReader>? contentReaders))
                {
                    if (contentReaders.TryGetValue(assetType, out IContentReader? contentReader))
                    {
                        return LoadAssetFromContentReader(contentReader);
                    }
                }
                else
                {
                    _registeredContentReaders.Add(protocolType, contentReaders = new Dictionary<Type, IContentReader>());
                }

                if (_registeredContentReaderFactories.TryGetValue(protocolType, out List<IContentReaderFactory>? factories))
                {
                    // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                    foreach (IContentReaderFactory factory in factories)
                    {
                        if (factory.TryCreate(assetType, out IContentReader? contentReader))
                        {
                            contentReaders.Add(assetType, contentReader);
                            return LoadAssetFromContentReader(contentReader);
                        }
                    }
                }

                throw new NotSupportedException(
                    $"The protocol type [{protocolType}] has no registered content reader to load the asset {assetName} of type {assetType} from.");
            }
        }

        {
            // ReSharper disable once InconsistentlySynchronizedField
            if (_registeredContentReaders.TryGetValue(protocolType, out Dictionary<Type, IContentReader>? contentReaders) &&
                contentReaders.TryGetValue(assetType, out IContentReader? contentReader))
            {
                return LoadAssetFromContentReader(contentReader);
            }

            return LoadAssetFromFactory();
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
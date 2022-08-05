#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Resolver;

namespace Exomia.Framework.Core.Content;

public sealed partial class ContentManager
{
    /// <inheritdoc />
    public bool AddContentReader(Type type, IContentReader reader)
    {
        lock (_registeredContentReaders)
        {
            if (!_registeredContentReaders.TryGetValue(reader.ProtocolType, out Dictionary<Type, IContentReader>? contentReaders))
            {
                _registeredContentReaders.Add(reader.ProtocolType, new Dictionary<Type, IContentReader>
                {
                    [type] = reader
                });

                return true;
            }
            
            if (contentReaders.ContainsKey(type)) { return false; }
            contentReaders.Add(type, reader);
        }
        return true;
    }

    /// <inheritdoc />
    public bool AddContentReaderFactory(IContentReaderFactory factory)
    {
        lock (_registeredContentReaderFactories)
        {
            if (!_registeredContentReaderFactories.TryGetValue(factory.ProtocolType, out List<IContentReaderFactory>? contentReaderFactories))
            {
                _registeredContentReaderFactories.Add(factory.ProtocolType, new List<IContentReaderFactory>
                {
                    factory
                });

                return true;
            }
            
            if (contentReaderFactories.Contains(factory)) { return false; }
            contentReaderFactories.Add(factory);
        }
        return true;
    }

    /// <inheritdoc />
    public bool AddContentResolver(IContentResolver resolver)
    {
        lock (_registeredContentResolvers)
        {
            if (_registeredContentResolvers.Contains(resolver)) { return false; }
            _registeredContentResolvers.Add(resolver);
        }
        return true;
    }

    /// <inheritdoc />
    public bool AddEmbeddedResourceContentResolver(IEmbeddedResourceContentResolver resolver)
    {
        lock (_registeredEmbeddedResourceResolvers)
        {
            if (_registeredEmbeddedResourceResolvers.Contains(resolver)) { return false; }
            _registeredEmbeddedResourceResolvers.Add(resolver);
        }
        return true;
    }
}
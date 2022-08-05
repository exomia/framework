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
            if (_registeredContentReaders.ContainsKey(type)) { return false; }
            _registeredContentReaders.Add(type, reader);
        }
        return true;
    }

    /// <inheritdoc />
    public bool AddContentReaderFactory(IContentReaderFactory factory)
    {
        lock (_registeredContentReaderFactories)
        {
            if (_registeredContentReaderFactories.Contains(factory)) { return false; }
            _registeredContentReaderFactories.Add(factory);
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
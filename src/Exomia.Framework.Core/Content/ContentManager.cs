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
using Exomia.Framework.Core.Content.Resolver.EmbeddedResource;

namespace Exomia.Framework.Core.Content;

/// <summary> Manager for contents. This class cannot be inherited. </summary>
public sealed class ContentManager : IContentManager
{
    private const int INITIAL_QUEUE_SIZE = 16;

    private readonly Dictionary<AssetKey, object>     _assetLockers;
    private readonly Dictionary<AssetKey, object>     _loadedAssets;
    private readonly List<IContentReaderFactory>      _registeredContentReaderFactories;
    private readonly Dictionary<Type, IContentReader> _registeredContentReaders;
    private readonly List<IContentResolver>           _registeredContentResolvers;
    private readonly List<IEmbeddedResourceResolver>  _registeredEmbeddedResourceResolvers;
    private          string                           _rootDirectory = string.Empty;

    /// <inheritdoc />
    public string RootDirectory
    {
        get { return _rootDirectory; }
        set
        {
            lock (_loadedAssets)
            {
                if (_loadedAssets.Count > 0)
                {
                    throw new InvalidOperationException(
                        $"{nameof(RootDirectory)} cannot be changed when a {nameof(ContentManager)} has already assets loaded");
                }
            }

            _rootDirectory = value;
        }
    }

    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; }

    /// <summary> Initializes a new instance of the <see cref="ContentManager" /> class. </summary>
    /// <param name="serviceProvider"> The service provider. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    /// <exception cref="TypeLoadException">     Thrown when a Type Load error condition occurs. </exception>
    public ContentManager(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _loadedAssets                        = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);
        _registeredContentResolvers          = new List<IContentResolver>(INITIAL_QUEUE_SIZE);
        _registeredEmbeddedResourceResolvers = new List<IEmbeddedResourceResolver>(INITIAL_QUEUE_SIZE);
        _registeredContentReaders            = new Dictionary<Type, IContentReader>(INITIAL_QUEUE_SIZE);
        _registeredContentReaderFactories    = new List<IContentReaderFactory>(INITIAL_QUEUE_SIZE);
        _assetLockers                        = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);

        List<(int order, IContentResolver resolver)> resolvers = new List<(int, IContentResolver)>(3);
        List<(int order, IEmbeddedResourceResolver resolver)> embeddedResourceResolvers =
            new List<(int, IEmbeddedResourceResolver)>(1);

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName!.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)) { continue; }
            if (a.FullName!.StartsWith("ms",     StringComparison.InvariantCultureIgnoreCase)) { continue; }

            foreach (Type t in a.GetTypes())
            {
                if (t.IsClass && !t.IsInterface || t.IsValueType && !t.IsEnum)
                {
                    ContentReadableAttribute? contentReadableAttribute = t.GetCustomAttribute<ContentReadableAttribute>(false);
                    if (contentReadableAttribute != null)
                    {
                        AddContentReader(t, contentReadableAttribute.Reader);
                    }
                }

                if (t.IsClass && !t.IsInterface)
                {
                    if (typeof(IContentResolver).IsAssignableFrom(t))
                    {
                        ContentResolverAttribute? contentResolverAttribute = t.GetCustomAttribute<ContentResolverAttribute>(false);
                        resolvers.Add(
                            (contentResolverAttribute?.Order ?? 0,
                                System.Activator.CreateInstance(t)
                                    as IContentResolver ?? throw new TypeLoadException(
                                    $"Can't create an instance of {nameof(IContentResolver)} from type: {t.AssemblyQualifiedName}")));
                    }

                    if (typeof(IEmbeddedResourceResolver).IsAssignableFrom(t))
                    {
                        ContentResolverAttribute? contentResolverAttribute = t.GetCustomAttribute<ContentResolverAttribute>(false);
                        embeddedResourceResolvers.Add(
                            (contentResolverAttribute?.Order ?? 0,
                                System.Activator.CreateInstance(t)
                                    as IEmbeddedResourceResolver ?? throw new TypeLoadException(
                                    $"Can't create an instance of {nameof(IEmbeddedResourceResolver)} from type: {t.AssemblyQualifiedName}")));
                    }
                }
            }
        }

        foreach ((_, IContentResolver resolver) in resolvers.OrderBy(t => t.order))
        {
            AddContentResolver(resolver);
        }

        resolvers.Clear();

        foreach ((_, IEmbeddedResourceResolver resolver) in embeddedResourceResolvers.OrderBy(t => t.order))
        {
            AddEmbeddedResourceContentResolver(resolver);
        }

        embeddedResourceResolvers.Clear();
    }

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
    public bool AddEmbeddedResourceContentResolver(IEmbeddedResourceResolver resolver)
    {
        lock (_registeredEmbeddedResourceResolvers)
        {
            if (_registeredEmbeddedResourceResolvers.Contains(resolver)) { return false; }
            _registeredEmbeddedResourceResolvers.Add(resolver);
        }

        return true;
    }

    /// <inheritdoc />
    public bool Exists(string assetName)
    {
        if (assetName == null)
        {
            throw new ArgumentNullException(nameof(assetName));
        }

        List<IContentResolver> resolvers;
        lock (_registeredContentResolvers)
        {
            resolvers = new List<IContentResolver>(_registeredContentResolvers);
        }

        if (resolvers.Count == 0)
        {
            throw new InvalidOperationException("No resolver registered to this content manager");
        }

        string assetPath = Path.Combine(_rootDirectory, assetName);

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (IContentResolver contentResolver in resolvers)
        {
            if (contentResolver.Exists(assetPath)) { return true; }
        }

        return false;
    }

    /// <inheritdoc />
    public object Load(Type assetType, string assetName, bool fromEmbeddedResource = false)
    {
        if (assetName == null)
        {
            throw new ArgumentNullException(nameof(assetName));
        }

        if (assetType == null)
        {
            throw new ArgumentNullException(nameof(assetType));
        }

        AssetKey assetKey = new AssetKey(assetType, assetName);
        lock (GetAssetLocker(assetKey, true)!)
        {
            if (_loadedAssets.TryGetValue(assetKey, out object? result))
            {
                return result;
            }
            result = LoadAssetWithDynamicContentReader(
                assetType,
                assetName,
                fromEmbeddedResource
                    ? ResolveEmbeddedResourceStream(assetType, assetName)
                    : ResolveStream(Path.Combine(_rootDirectory, assetName)));

            lock (_loadedAssets)
            {
                _loadedAssets.Add(assetKey, result);
            }

            return result;
        }
    }

    /// <inheritdoc />
    public T Load<T>(string assetName, bool fromEmbeddedResource = false)
    {
        return (T)Load(typeof(T), assetName, fromEmbeddedResource);
    }

    /// <inheritdoc />
    public void Unload()
    {
        lock (_assetLockers)
        {
            lock (_loadedAssets)
            {
                foreach (object loadedAsset in _loadedAssets.Values)
                {
                    if (loadedAsset is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                _assetLockers.Clear();
                _loadedAssets.Clear();
            }
        }
    }

    /// <inheritdoc />
    public bool Unload(Type assetType, string assetName)
    {
        if (assetType == null)
        {
            throw new ArgumentNullException(nameof(assetType));
        }

        if (assetName == null)
        {
            throw new ArgumentNullException(nameof(assetName));
        }

        AssetKey assetKey = new AssetKey(assetType, assetName);

        object? assetLockerRead = GetAssetLocker(assetKey, false);
        if (assetLockerRead == null) { return false; }

        object? asset;

        lock (assetLockerRead)
        {
            lock (_loadedAssets)
            {
                if (!_loadedAssets.TryGetValue(assetKey, out asset)) { return false; }
                _loadedAssets.Remove(assetKey);
            }

            lock (_assetLockers)
            {
                _assetLockers.Remove(assetKey);
            }
        }

        if (asset is IDisposable disposable)
        {
            disposable.Dispose();
        }

        return true;
    }

    /// <inheritdoc />
    public bool Unload<T>(string assetName)
    {
        return Unload(typeof(T), assetName);
    }

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
        List<IEmbeddedResourceResolver> resolvers;
        lock (_registeredContentResolvers)
        {
            resolvers = new List<IEmbeddedResourceResolver>(_registeredEmbeddedResourceResolvers);
        }

        if (resolvers.Count == 0)
        {
            throw new InvalidOperationException(
                $"No {nameof(IEmbeddedResourceResolver)} registered to this content manager");
        }

        Exception? lastException = null;
        foreach (IEmbeddedResourceResolver contentResolver in resolvers)
        {
            try
            {
                if (contentResolver.Exists(assetType, assetName, out Assembly assembly))
                {
                    Stream? stream = contentResolver.Resolve(assembly, assetName);
                    if (stream != null) { return stream; }
                }
            }
            catch (Exception ex) { lastException = ex; }
        }

        throw new AssetNotFoundException(assetName, lastException);
    }

    private object? GetAssetLocker(AssetKey assetKey, bool create)
    {
        lock (_assetLockers)
        {
            if (!_assetLockers.TryGetValue(assetKey, out object? assetLockerRead) && create)
            {
                assetLockerRead = new object();
                _assetLockers.Add(assetKey, assetLockerRead);
            }
            return assetLockerRead;
        }
    }

    private object LoadAssetWithDynamicContentReader(Type assetType, string assetName, Stream stream)
    {
        ContentReaderParameters parameters =
            new ContentReaderParameters(assetName, assetType, stream);

        try
        {
            if (!_registeredContentReaders.TryGetValue(assetType, out IContentReader? contentReader))
            {
                lock (_registeredContentReaderFactories)
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

    private readonly struct AssetKey : IEquatable<AssetKey>
    {
        private readonly Type   _assetType;
        private readonly string _assetName;

        /// <summary> Initializes a new instance of the <see cref="ContentManager" /> class. </summary>
        /// <param name="assetType"> Type of the asset. </param>
        /// <param name="assetName"> Name of the asset. </param>
        public AssetKey(Type assetType, string assetName)
        {
            _assetType = assetType;
            _assetName = assetName;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (_assetType.GetHashCode() * 397) ^ _assetName.GetHashCode();
        }

        /// <inheritdoc />
        public bool Equals(AssetKey other)
        {
            return _assetType == other._assetType &&
                   string.Equals(_assetName, other._assetName, StringComparison.OrdinalIgnoreCase);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is AssetKey key && Equals(key);
        }

        /// <summary> Equality operator. </summary>
        /// <param name="left">  The left. </param>
        /// <param name="right"> The right. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator ==(AssetKey left, AssetKey right)
        {
            return left.Equals(right);
        }

        /// <summary> Inequality operator. </summary>
        /// <param name="left">  The left. </param>
        /// <param name="right"> The right. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator !=(AssetKey left, AssetKey right)
        {
            return !left.Equals(right);
        }
    }

    #region IDisposable Support

    private bool _disposed;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                lock (_loadedAssets)
                {
                    _loadedAssets.Clear();
                }
                lock (_registeredContentReaderFactories)
                {
                    _registeredContentReaderFactories.Clear();
                }
                lock (_registeredContentReaders)
                {
                    _registeredContentReaders.Clear();
                }
                lock (_registeredContentResolvers)
                {
                    _registeredContentResolvers.Clear();
                }
            }

            _disposed = true;
        }
    }

    /// <summary> Finalizes an instance of the <see cref="ContentManager" /> class. </summary>
    ~ContentManager()
    {
        Dispose(false);
    }

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
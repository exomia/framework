#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Reflection;
using Exomia.Framework.Core.Content.ContentReader;
using Exomia.Framework.Core.Content.Resolver;

namespace Exomia.Framework.Core.Content;

/// <summary> Manager for contents. This class cannot be inherited. </summary>
public sealed partial class ContentManager : IContentManager
{
    private const int INITIAL_QUEUE_SIZE = 16;

    private readonly Dictionary<AssetKey, object>                       _assetLockers;
    private readonly Dictionary<AssetKey, object>                       _loadedAssets;
    private readonly Dictionary<Type, List<IContentReaderFactory>>      _registeredContentReaderFactories;
    private readonly Dictionary<Type, Dictionary<Type, IContentReader>> _registeredContentReaders;
    private readonly List<IContentResolver>                             _registeredContentResolvers;
    private readonly List<IEmbeddedResourceContentResolver>             _registeredEmbeddedResourceResolvers;
    private          string                                             _rootDirectory = string.Empty;

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
    /// <exception cref="TypeLoadException"> Thrown when a Type Load error condition occurs. </exception>
    public ContentManager(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

        _loadedAssets                        = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);
        _registeredContentResolvers          = new List<IContentResolver>(INITIAL_QUEUE_SIZE);
        _registeredEmbeddedResourceResolvers = new List<IEmbeddedResourceContentResolver>(INITIAL_QUEUE_SIZE);
        _registeredContentReaders            = new Dictionary<Type, Dictionary<Type, IContentReader>>(INITIAL_QUEUE_SIZE);
        _registeredContentReaderFactories    = new Dictionary<Type, List<IContentReaderFactory>>(INITIAL_QUEUE_SIZE);
        _assetLockers                        = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);

        List<(int order, IContentResolver resolver)>                 resolvers                 = new(3);
        List<(int order, IEmbeddedResourceContentResolver resolver)> embeddedResourceResolvers = new(1);

        foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (a.FullName!.StartsWith("System", StringComparison.InvariantCultureIgnoreCase)) { continue; }
            if (a.FullName!.StartsWith("ms",     StringComparison.InvariantCultureIgnoreCase)) { continue; }

            foreach (Type t in a.GetTypes())
            {
                if ((t.IsClass && !t.IsInterface) || (t.IsValueType && !t.IsEnum))
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

                    if (typeof(IEmbeddedResourceContentResolver).IsAssignableFrom(t))
                    {
                        ContentResolverAttribute? contentResolverAttribute = t.GetCustomAttribute<ContentResolverAttribute>(false);
                        embeddedResourceResolvers.Add(
                            (contentResolverAttribute?.Order ?? 1,
                                System.Activator.CreateInstance(t)
                                    as IEmbeddedResourceContentResolver ?? throw new TypeLoadException(
                                    $"Can't create an instance of {nameof(IEmbeddedResourceContentResolver)} from type: {t.AssemblyQualifiedName}")));
                    }
                }
            }
        }

        foreach ((_, IContentResolver resolver) in resolvers.OrderBy(t => t.order))
        {
            AddContentResolver(resolver);
        }

        resolvers.Clear();

        foreach ((_, IEmbeddedResourceContentResolver resolver) in embeddedResourceResolvers.OrderBy(t => t.order))
        {
            AddEmbeddedResourceContentResolver(resolver);
        }

        embeddedResourceResolvers.Clear();

        // Add default e1 content reader factory
        AddContentReaderFactory(new E1ContentReaderFactory());
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
        /// <param name="left"> The left. </param>
        /// <param name="right"> The right. </param>
        /// <returns> The result of the operation. </returns>
        public static bool operator ==(AssetKey left, AssetKey right)
        {
            return left.Equals(right);
        }

        /// <summary> Inequality operator. </summary>
        /// <param name="left"> The left. </param>
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
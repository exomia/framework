#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Exomia.Framework.Content.Exceptions;
using Exomia.Framework.Content.Resolver;

namespace Exomia.Framework.Content
{
    /// <inheritdoc />
    public sealed class ContentManager : IContentManager
    {
        private const int INITIAL_QUEUE_SIZE = 16;

        private readonly Dictionary<AssetKey, object> _assetLockers;

        private readonly Dictionary<AssetKey, object> _loadedAssets;
        private readonly List<IContentReaderFactory> _registeredContentReaderFactories;
        private readonly Dictionary<Type, IContentReader> _registeredContentReaders;
        private readonly List<IContentResolver> _registeredContentResolvers;

        private string _rootDirectory;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentManager" /> class.
        /// </summary>
        /// <param name="serviceRegistry">IServiceRegistry</param>
        public ContentManager(IServiceRegistry serviceRegistry)
        {
            ServiceRegistry = serviceRegistry ?? throw new ArgumentNullException(nameof(serviceRegistry));

            _loadedAssets = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);
            _registeredContentResolvers = new List<IContentResolver>(INITIAL_QUEUE_SIZE);
            _registeredContentReaders = new Dictionary<Type, IContentReader>(INITIAL_QUEUE_SIZE);
            _registeredContentReaderFactories = new List<IContentReaderFactory>(INITIAL_QUEUE_SIZE);
            _assetLockers = new Dictionary<AssetKey, object>(INITIAL_QUEUE_SIZE);

            AddContentResolver(new DSFileStreamContentResolver());

            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (a.FullName.StartsWith("System")) { continue; }
                if (a.FullName.StartsWith("SharpDX")) { continue; }
                if (a.FullName.StartsWith("ms")) { continue; }
                if (a.FullName.StartsWith("Xilium.CefGlue")) { continue; }

                foreach (Type t in a.GetTypes())
                {
                    ContentReadableAttribute attribute = null;
                    if ((attribute = t.GetCustomAttribute<ContentReadableAttribute>(false)) != null)
                    {
                        AddContentReader(t, attribute.Reader);
                    }
                }
            }
        }

        /// <summary>
        ///     destructor
        /// </summary>
        ~ContentManager()
        {
            Dispose(false);
        }

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
                            "RootDirectory cannot be changed when a ContentManager has already assets loaded");
                    }
                }

                _rootDirectory = value;
            }
        }

        public IServiceRegistry ServiceRegistry { get; }

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
        public bool Exists(string assetName)
        {
            if (assetName == null)
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            // First, resolve the stream for this asset.
            List<IContentResolver> resolvers;
            lock (_registeredContentResolvers)
            {
                resolvers = new List<IContentResolver>(_registeredContentResolvers);
            }

            if (resolvers.Count == 0)
            {
                throw new InvalidOperationException("No resolver registered to this content manager");
            }

            string assetPath = Path.Combine(_rootDirectory ?? string.Empty, assetName);

            foreach (IContentResolver contentResolver in resolvers)
            {
                if (contentResolver.Exists(assetPath)) { return true; }
            }

            return false;
        }

        /// <inheritdoc />
        public object Load(Type assetType, string assetName)
        {
            if (assetName == null)
            {
                throw new ArgumentNullException(nameof(assetName));
            }

            if (assetType == null)
            {
                throw new ArgumentNullException(nameof(assetType));
            }

            object result = null;
            AssetKey assetKey = new AssetKey(assetType, assetName);
            lock (GetAssetLocker(assetKey, true))
            {
                lock (_loadedAssets)
                {
                    if (_loadedAssets.TryGetValue(assetKey, out result))
                    {
                        return result;
                    }
                }

                string assetPath = Path.Combine(_rootDirectory ?? string.Empty, assetName);
                Stream stream = FindStream(assetPath);

                result = LoadAssetWithDynamicContentReader(assetType, assetName, stream);
                lock (_loadedAssets)
                {
                    _loadedAssets.Add(assetKey, result);
                }
            }
            return result;
        }

        /// <inheritdoc />
        public T Load<T>(string assetName)
        {
            return (T)Load(typeof(T), assetName);
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
                            disposable = null;
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

            object assetLockerRead = GetAssetLocker(assetKey, false);
            if (assetLockerRead == null) { return false; }

            object asset = null;

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
                disposable = null;
            }

            return true;
        }

        /// <inheritdoc />
        public bool Unload<T>(string assetName)
        {
            return Unload(typeof(T), assetName);
        }

        private object GetAssetLocker(AssetKey assetKey, bool create)
        {
            lock (_assetLockers)
            {
                if (!_assetLockers.TryGetValue(assetKey, out object assetLockerRead) && create)
                {
                    assetLockerRead = new object();
                    _assetLockers.Add(assetKey, assetLockerRead);
                }
                return assetLockerRead;
            }
        }

        private Stream FindStream(string assetName)
        {
            List<IContentResolver> resolvers;
            lock (_registeredContentResolvers)
            {
                resolvers = new List<IContentResolver>(_registeredContentResolvers);
            }

            if (resolvers.Count == 0)
            {
                throw new InvalidOperationException("No resolver registered to this content manager");
            }

            Stream stream = null;
            Exception lastException = null;
            foreach (IContentResolver contentResolver in resolvers)
            {
                try
                {
                    if (contentResolver.Exists(assetName))
                    {
                        stream = contentResolver.Resolve(assetName);
                        if (stream != null) { break; }
                    }
                }
                catch (Exception ex) { lastException = ex; }
            }

            return stream ?? throw new AssetNotFoundException(assetName, lastException);
        }

        private object LoadAssetWithDynamicContentReader(Type assetType, string assetName, Stream stream)
        {
            object result;
            ContentReaderParameters parameters = new ContentReaderParameters
            {
                AssetName = assetName, AssetType = assetType, Stream = stream
            };

            try
            {
                IContentReader contentReader;
                lock (_registeredContentReaders)
                {
                    if (!_registeredContentReaders.TryGetValue(assetType, out contentReader))
                    {
                        lock (_registeredContentReaderFactories)
                        {
                            foreach (IContentReaderFactory factory in _registeredContentReaderFactories)
                            {
                                if (factory.TryCreate(assetType, out contentReader))
                                {
                                    _registeredContentReaders.Add(assetType, contentReader);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (contentReader == null)
                {
                    throw new NotSupportedException(
                        $"Type [{assetType.FullName}] doesn't provide a ContentReadableAttribute, and there is no registered content reader/factory for it.");
                }

                result = contentReader.ReadContent(this, ref parameters)
                         ?? throw new NotSupportedException(
                             $"Registered ContentReader of type [{contentReader.GetType()}] fails to load content of type [{assetType.FullName}] from file [{assetName}].");
            }
            finally
            {
                if (!parameters.KeepStreamOpen)
                {
                    stream.Dispose();
                }
            }

            return result;
        }

        private struct AssetKey : IEquatable<AssetKey>
        {
            public AssetKey(Type assetType, string assetName)
            {
                _assetType = assetType;
                _assetName = assetName;
            }

            private readonly Type _assetType;

            private readonly string _assetName;

            public bool Equals(AssetKey other)
            {
                return _assetType == other._assetType &&
                       string.Equals(_assetName, other._assetName, StringComparison.OrdinalIgnoreCase);
            }

            public override bool Equals(object obj)
            {
                if (obj is null) { return false; }
                return obj is AssetKey key && Equals(key);
            }

            public override int GetHashCode()
            {
                return (_assetType.GetHashCode() * 397) ^ _assetName.GetHashCode();
            }

            public static bool operator ==(AssetKey left, AssetKey right)
            {
                return left.Equals(right);
            }

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
                    _loadedAssets.Clear();
                    _registeredContentReaderFactories.Clear();
                    _registeredContentReaders.Clear();
                    _registeredContentResolvers.Clear();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
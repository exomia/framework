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
            if (!_loadedAssets.TryGetValue(assetKey, out object? result))
            {
                lock (_loadedAssets)
                {
                    if (!_loadedAssets.TryGetValue(assetKey, out result))
                    {
                        result = LoadAssetWithDynamicContentReader(
                            assetType,
                            assetName,
                            fromEmbeddedResource
                                ? ResolveEmbeddedResourceStream(assetType, assetName)
                                : ResolveStream(Path.Combine(_rootDirectory, assetName)));

                        _loadedAssets.Add(assetKey, result);
                    }
                }
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
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content.Resolver;

namespace Exomia.Framework.Core.Content;

/// <summary> IContentManager interface. </summary>
public interface IContentManager : IDisposable
{
    /// <summary>The root directory. </summary>
    /// <value> The pathname of the root directory. </value>
    string RootDirectory { get;  }

    /// <summary> Gets the service provider. </summary>
    /// <value> The service provider. </value>
    IServiceProvider ServiceProvider { get; }

    /// <summary> Add a new content reader to the list. </summary>
    /// <param name="type"> The type of the content reader. </param>
    /// <param name="reader"> The content reader to add. </param>
    /// <returns> true if the specified content reader successfully added, false otherwise. </returns>
    bool AddContentReader(Type type, IContentReader reader);

    /// <summary> Add a new content reader factory to the list. </summary>
    /// <param name="factory"> The content reader factory to add. </param>
    /// <returns> true if the specified content reader factory successfully added, false otherwise. </returns>
    bool AddContentReaderFactory(IContentReaderFactory factory);

    /// <summary> Add a new content resolver to the list. </summary>
    /// <param name="resolver"> the content resolver to add. </param>
    /// <returns> true if the specified content resolver successfully added, false otherwise. </returns>
    bool AddContentResolver(IContentResolver resolver);

    /// <summary> Add a new embedded resource content resolver to the list. </summary>
    /// <param name="resolver"> The content resolver to add. </param>
    /// <returns> true if the specified content resolver successfully added, false otherwise. </returns>
    bool AddEmbeddedResourceContentResolver(IEmbeddedResourceContentResolver resolver);

    /// <summary> Checks if the specified assets exists. </summary>
    /// <param name="assetName"> The asset name with extension. </param>
    /// <returns> true if the specified assets exists, false otherwise. </returns>
    bool Exists(string assetName);

    /// <summary> Loads an asset that has been processed by the content pipeline. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="assetName"> Asset path and name (with its extension) </param>
    /// <param name="fromEmbeddedResource"> (Optional) true if the asset should be loaded from an embedded resource; false otherwise. </param>
    /// <returns> A asset of type <typeparamref name="T" />. </returns>
    /// <exception cref="Exceptions.AssetNotFoundException"> If the asset was not found from all <see cref="IContentResolver" />. </exception>
    /// <exception cref="NotSupportedException"> If no content reader was suitable to decode the asset. </exception>
    T Load<T>(string assetName, bool fromEmbeddedResource = false);

    /// <summary> Loads an asset that has been processed by the Content Pipeline. </summary>
    /// <param name="assetType"> Asset Type. </param>
    /// <param name="assetName"> Full asset name (with its extension) </param>
    /// <param name="fromEmbeddedResource"> (Optional) true if the asset should be loaded from an embedded resource; false otherwise. </param>
    /// <returns> An asset of type <paramref name="assetType" />. </returns>
    /// <exception cref="Exceptions.AssetNotFoundException"> If the asset was not found from all <see cref="IContentResolver" />. </exception>
    /// <exception cref="NotSupportedException"> If no content reader was suitable to decode the asset. </exception>
    object Load(Type assetType, string assetName, bool fromEmbeddedResource = false);

    /// <summary> Unloads all data that was loaded by this ContentManager. All data will be disposed. </summary>
    /// <remarks> Unlike <see cref="IContentManager.Load(Type, string, bool)" /> method, this method is not thread safe and must be called by a single caller at a single time. </remarks>
    void Unload();

    /// <summary> Unloads and disposes an asset. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="assetName"> The asset name. </param>
    /// <returns> true if the asset exists and was unloaded, false otherwise. </returns>
    bool Unload<T>(string assetName);

    /// <summary> Unloads and disposes an asset. </summary>
    /// <param name="assetType"> The asset type. </param>
    /// <param name="assetName"> The asset name. </param>
    /// <returns> true if the asset exists and was unloaded, false otherwise. </returns>
    bool Unload(Type assetType, string assetName);
}
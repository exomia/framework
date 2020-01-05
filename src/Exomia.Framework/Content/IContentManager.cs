#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Content.Resolver;
using Exomia.Framework.Content.Resolver.EmbeddedResource;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     IContentManager interface.
    /// </summary>
    public interface IContentManager : IDisposable
    {
        /// <summary>
        ///     RootDirectory.
        /// </summary>
        /// <value>
        ///     The pathname of the root directory.
        /// </value>
        string RootDirectory { get; set; }

        /// <summary>
        ///     ServiceRegistry.
        /// </summary>
        /// <value>
        ///     The service registry.
        /// </value>
        IServiceRegistry ServiceRegistry { get; }

        /// <summary>
        ///     add a new content reader to the list.
        /// </summary>
        /// <param name="type">   the type of the content reader. </param>
        /// <param name="reader"> the content reader to add. </param>
        /// <returns>
        ///     <c>true</c> if the specified content reader successfully added, <c>false</c> otherwise.
        /// </returns>
        bool AddContentReader(Type type, IContentReader reader);

        /// <summary>
        ///     add a new content reader factory to the list.
        /// </summary>
        /// <param name="factory"> the content reader factory to add. </param>
        /// <returns>
        ///     <c>true</c> if the specified content reader factory successfully added, <c>false</c>
        ///     otherwise.
        /// </returns>
        bool AddContentReaderFactory(IContentReaderFactory factory);

        /// <summary>
        ///     add a new content resolver to the list.
        /// </summary>
        /// <param name="resolver"> the content resolver to add. </param>
        /// <returns>
        ///     <c>true</c> if the specified content resolver successfully added, <c>false</c> otherwise.
        /// </returns>
        bool AddContentResolver(IContentResolver resolver);

        /// <summary>
        ///     add a new embedded resource content resolver to the list.
        /// </summary>
        /// <param name="resolver"> the content resolver to add. </param>
        /// <returns>
        ///     <c>true</c> if the specified content resolver successfully added, <c>false</c> otherwise.
        /// </returns>
        bool AddEmbeddedResourceContentResolver(IEmbeddedResourceResolver resolver);

        /// <summary>
        ///     Checks if the specified assets exists.
        /// </summary>
        /// <param name="assetName"> The asset name with extension. </param>
        /// <returns>
        ///     <c>true</c> if the specified assets exists, <c>false</c> otherwise.
        /// </returns>
        bool Exists(string assetName);

        /// <summary>
        ///     Loads an asset that has been processed by the Content Pipeline.
        /// </summary>
        /// <typeparam name="T"> . </typeparam>
        /// <param name="assetName">            Full asset name (with its extension) </param>
        /// <param name="fromEmbeddedResource">
        ///     (Optional)
        ///     <c>true</c> if the asset should be loaded from an
        ///     embedded resource; <c>false</c>
        ///     otherwise.
        /// </param>
        /// <returns>
        ///     ``0.
        /// </returns>
        /// <exception cref="Exceptions.AssetNotFoundException">
        ///     If the asset was not found from all
        ///     <see cref="IContentResolver" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     If no content reader was suitable to
        ///     decode the asset.
        /// </exception>
        T Load<T>(string assetName, bool fromEmbeddedResource = false);

        /// <summary>
        ///     Loads an asset that has been processed by the Content Pipeline.
        /// </summary>
        /// <param name="assetType">            Asset Type. </param>
        /// <param name="assetName">            Full asset name (with its extension) </param>
        /// <param name="fromEmbeddedResource">
        ///     (Optional)
        ///     <c>true</c> if the asset should be loaded from an
        ///     embedded resource; <c>false</c>
        ///     otherwise.
        /// </param>
        /// <returns>
        ///     Asset.
        /// </returns>
        /// <exception cref="Exceptions.AssetNotFoundException">
        ///     If the asset was not found from all
        ///     <see cref="IContentResolver" />.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     If no content reader was suitable to
        ///     decode the asset.
        /// </exception>
        object Load(Type assetType, string assetName, bool fromEmbeddedResource = false);

        /// <summary>
        ///     Unloads all data that was loaded by this ContentManager. All data will be disposed.
        /// </summary>
        /// <remarks>
        ///     Unlike <see cref="ContentManager.Load{T}" /> method, this method is not thread safe and
        ///     must be called by a single caller at a single time.
        /// </remarks>
        void Unload();

        /// <summary>
        ///     Unloads and disposes an asset.
        /// </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="assetName"> The asset name. </param>
        /// <returns>
        ///     <c>true</c> if the asset exists and was unloaded, <c>false</c> otherwise.
        /// </returns>
        bool Unload<T>(string assetName);

        /// <summary>
        ///     Unloads and disposes an asset.
        /// </summary>
        /// <param name="assetType"> The asset type. </param>
        /// <param name="assetName"> The asset name. </param>
        /// <returns>
        ///     <c>true</c> if the asset exists and was unloaded, <c>false</c> otherwise.
        /// </returns>
        bool Unload(Type assetType, string assetName);
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;

namespace Exomia.Framework.Content.Loader
{
    /// <summary>
    ///     Interface for model file loader.
    /// </summary>
    /// <typeparam name="TModel"> Type of the model. </typeparam>
    interface IModelFileLoader<out TModel>
    {
        /// <summary>
        ///     Loads the model from the given stream.
        /// </summary>
        /// <param name="stream"> The model file stream to load the model from. </param>
        /// <returns>
        ///     A TModel.
        /// </returns>
        TModel Load(Stream stream);
    }
}
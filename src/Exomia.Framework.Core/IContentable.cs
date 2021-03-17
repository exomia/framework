#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core
{
    /// <summary>
    ///     An interface to load and unload content.
    /// </summary>
    public interface IContentable
    {
        /// <summary>
        ///     Loads the content.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        void LoadContent(IServiceRegistry registry);

        /// <summary>
        ///     Called when graphics resources need to be unloaded.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        void UnloadContent(IServiceRegistry registry);
    }
}
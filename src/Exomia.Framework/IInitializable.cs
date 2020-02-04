#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to initialize a game component.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        ///     This method is called when the component is added to the game.
        /// </summary>
        /// <param name="registry"> The registry. </param>
        /// <remarks>
        ///     This method can be used for tasks like querying for services the component needs and
        ///     setting up non-graphics resources.
        /// </remarks>
        void Initialize(IServiceRegistry registry);
    }
}
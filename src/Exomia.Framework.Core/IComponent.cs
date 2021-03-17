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
    ///     An interface to define a game component.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        ///     the name of the component.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        string Name { get; }
    }
}
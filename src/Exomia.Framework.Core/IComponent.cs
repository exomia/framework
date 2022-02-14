#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core
{
    /// <summary> Interface for a component. </summary>
    public interface IComponent
    {
        /// <summary> Gets a unique identifier. </summary>
        /// <value> The identifier of the unique. </value>
        Guid Guid { get; }
    }
}
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
    ///     An interface to clone a object.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public interface ICloneable<out T>
    {
        /// <summary>
        ///     returns a deep copy of the object.
        /// </summary>
        /// <returns>
        ///     a new deep copied object.
        /// </returns>
        T Clone();
    }
}
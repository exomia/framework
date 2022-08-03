#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core;

/// <summary>
///     An interface to clone a object.
/// </summary>
/// <typeparam name="T"> Generic type parameter. </typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    ///     Returns a deep copy of the object.
    /// </summary>
    /// <returns>
    ///     A new deep copied object.
    /// </returns>
    T Clone();
}
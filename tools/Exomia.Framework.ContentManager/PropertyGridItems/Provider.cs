#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.PropertyGridItems;

/// <summary>
///     A provider.
/// </summary>
public static class Provider
{
    /// <summary>
    ///     Gets the value.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <returns>
    ///     A T.
    /// </returns>
    public delegate T Value<out T>();

    /// <summary>
    ///     Static access to the given item.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="item"> The item. </param>
    /// <returns>
    ///     A Value&lt;T&gt;
    /// </returns>
    public static Value<T> Static<T>(T item)
    {
        return () => item;
    }
}
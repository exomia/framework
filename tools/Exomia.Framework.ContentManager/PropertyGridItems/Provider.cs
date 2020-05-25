#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A provider.
    /// </summary>
    public static class Provider
    {
        public delegate T Value<out T>();

        public static Value<T> Static<T>(T item)
        {
            return () => item;
        }
    }
}
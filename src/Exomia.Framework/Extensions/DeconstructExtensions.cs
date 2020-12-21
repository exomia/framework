#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

#if NETSTANDARD2_0
using System.Collections.Generic;

namespace Exomia.Framework.Extensions
{
    /// <summary>
    ///     A deconstruct extensions.
    /// </summary>
    public static class DeconstructExtensions
    {
        /// <summary>
        ///     A type deconstruct-or that extracts the individual members from this object.
        /// </summary>
        /// <typeparam name="TKey">   Type of the key. </typeparam>
        /// <typeparam name="TValue"> Type of the value. </typeparam>
        /// <param name="kvp">   The kvp. </param>
        /// <param name="key">   [out] The key. </param>
        /// <param name="value"> [out] The value. </param>
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp,
                                                     out  TKey                       key,
                                                     out  TValue                     value)
        {
            key   = kvp.Key;
            value = kvp.Value;
        }
    }
}
#endif
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;

namespace Exomia.Framework.Linq
{
    /// <content>
    ///     Linq extensions.
    /// </content>
    public static partial class LinqExt
    {
        /// <summary>
        ///     Immediately executes the given action on each element in the source sequence.
        /// </summary>
        /// <typeparam name="T"> The type of the elements in the sequence. </typeparam>
        /// <param name="source"> The sequence of elements. </param>
        /// <param name="action"> The action to execute on each element. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            foreach (T element in source)
            {
                action(element);
            }
        }

        /// <summary>
        ///     Immediately executes the given action on each element in the source sequence. Each
        ///     element's index is used in the logic of the action.
        /// </summary>
        /// <typeparam name="T"> The type of the elements in the sequence. </typeparam>
        /// <param name="source"> The sequence of elements. </param>
        /// <param name="action">
        ///     The action to execute on each element; the second parameter of the action
        ///     represents the index of the source element.
        /// </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            int index = 0;
            foreach (T element in source)
            {
                action(element, index++);
            }
        }
    }
}
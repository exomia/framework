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
    ///     A linq extension.
    /// </content>
    public static partial class LinqExt
    {
        /// <summary>
        ///     Returns the element which has the minimum value in a generic sequence.
        /// </summary>
        /// <typeparam name="TSource"> The type of elements of <paramref name="source" />. </typeparam>
        /// <typeparam name="T">       The type to compare the elements of <paramref name="source" /> with. </typeparam>
        /// <param name="source">    A sequence of elements to determine the minimum value of. </param>
        /// <param name="predicate"> A function that returns the value to check against the current lowest value. </param>
        /// <param name="comparer">  (Optional) A comparer used to define the semantics of element comparison. </param>
        /// <returns>
        ///     The element in the specified sequence with the lowest value.
        /// </returns>
        /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are null. </exception>
        /// <exception cref="InvalidOperationException"> The source sequence is empty. </exception>
        public static TSource Min<TSource, T>(this IEnumerable<TSource> source,
                                              Func<TSource, T>          predicate,
                                              IComparer<T>?             comparer = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }

            IComparer<T> c = comparer ?? Comparer<T>.Default;

            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext()) { throw new InvalidOperationException("The source sequence is empty."); }

            TSource r   = enumerator.Current;
            T       min = predicate(r);

            while (enumerator.MoveNext())
            {
                T v = predicate(enumerator.Current);
                if (c.Compare(v, min) < 0)
                {
                    min = v;
                    r   = enumerator.Current;
                }
            }

            return r;
        }

        /// <summary>
        ///     Returns the element which has the maximum value in a generic sequence.
        /// </summary>
        /// <typeparam name="TSource"> The type of elements of <paramref name="source" />. </typeparam>
        /// <typeparam name="T">       The type to compare the elements of <paramref name="source" /> with. </typeparam>
        /// <param name="source">    A sequence of elements to determine the maximum value of. </param>
        /// <param name="predicate"> A function that returns the value to check against the current biggest value. </param>
        /// <param name="comparer">  (Optional) A comparer used to define the semantics of element comparison. </param>
        /// <returns>
        ///     The element in the specified sequence with the biggest value.
        /// </returns>
        /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are null. </exception>
        /// <exception cref="InvalidOperationException"> The source sequence is empty. </exception>
        public static TSource Max<TSource, T>(this IEnumerable<TSource> source,
                                              Func<TSource, T>          predicate,
                                              IComparer<T>?             comparer = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }

            IComparer<T> c = comparer ?? Comparer<T>.Default;

            using IEnumerator<TSource> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext()) { throw new InvalidOperationException("The source sequence is empty."); }

            TSource r   = enumerator.Current;
            T       max = predicate(r);

            while (enumerator.MoveNext())
            {
                T v = predicate(enumerator.Current);
                if (c.Compare(v, max) > 0)
                {
                    max = v;
                    r   = enumerator.Current;
                }
            }

            return r;
        }
    }
}
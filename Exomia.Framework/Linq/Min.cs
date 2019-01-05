#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

using System;
using System.Collections.Generic;

namespace Exomia.Framework.Linq
{
    public static partial class LinqExt
    {
        /// <summary>
        ///     Returns the element which has the minimum value in a generic sequence.
        /// </summary>
        /// <param name="source">A sequence of elements to determine the minimum value of.</param>
        /// <param name="predicate">A function that returns the value to check against the current lowest value.</param>
        /// <param name="comparer">A comparer used to define the semantics of element comparison</param>
        /// <typeparam name="TSource">The type of elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="T">The type to compare the elements of <paramref name="source" /> with.</typeparam>
        /// <returns>The element in the specified sequence with the lowest value.</returns>
        /// <exception cref="T:System.InvalidOperationException">The source sequence is empty.</exception>
        public static TSource Min<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> predicate,
            IComparer<T> comparer = null)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (predicate == null) { throw new ArgumentNullException(nameof(predicate)); }

            IComparer<T> c = comparer ?? Comparer<T>.Default;

            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext()) { throw new InvalidOperationException("The source sequence is empty."); }

                TSource r = enumerator.Current;
                T min = predicate(r);

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
        }
    }
}
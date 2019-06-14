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
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Linq
{
    /// <content>
    ///     A linq extent.
    /// </content>
    public static partial class LinqExt
    {
        /// <summary>
        ///     Calculates the sum of a sequence of T values.
        /// </summary>
        /// <typeparam name="TSource"> TSource. </typeparam>
        /// <param name="source"> sequence. </param>
        /// <returns>
        ///     sum of TSource.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TSource Sum<TSource>(this IEnumerable<TSource> source)
        {
            return LinqExt<TSource>.Sum(source);
        }
    }

    /// <summary>
    ///     Linq{T} extensions.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    public static class LinqExt<T>
    {
        /// <summary>
        ///     Number of.
        /// </summary>
        internal static readonly Func<IEnumerable<T>, T> Sum = __sum();

        /// <summary>
        ///     generates a performance optimized sum function for a generic type t for later use.
        /// </summary>
        /// <returns>
        ///     a function which calculates the sum of a given array of type t.
        /// </returns>
        public static Func<T[], T> CreateSumFunc()
        {
            ParameterExpression arrayExpr  = Expression.Parameter(typeof(T[]));
            ParameterExpression iExpr      = Expression.Variable(typeof(int));
            ParameterExpression rExpr      = Expression.Variable(typeof(T));
            LabelTarget         breakLabel = Expression.Label(typeof(T));
            return Expression.Lambda<Func<T[], T>>(
                                 Expression.Block(
                                     new[] { rExpr, iExpr },
                                     Expression.Loop(
                                         Expression.IfThenElse(
                                             Expression.LessThan(iExpr, Expression.ArrayLength(arrayExpr)),
                                             Expression.Block(
                                                 Expression.Assign(
                                                     rExpr,
                                                     Expression.Add(rExpr, Expression.ArrayAccess(arrayExpr, iExpr))),
                                                 Expression.Assign(iExpr, Expression.Increment(iExpr))),
                                             Expression.Break(breakLabel, rExpr)
                                         ),
                                         breakLabel)), arrayExpr)
                             .Compile(false);
        }

        /// <summary>
        ///     Gets the sum.
        /// </summary>
        /// <returns>
        ///     A Func&lt;IEnumerable&lt;T&gt;,T&gt;
        /// </returns>
        /// <exception cref="InvalidOperationException"> Thrown when the requested operation is invalid. </exception>
        private static Func<IEnumerable<T>, T> __sum()
        {
            Type elementType    = typeof(T);
            Type enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);
            Type enumeratorType = typeof(IEnumerator<>).MakeGenericType(elementType);

            ParameterExpression arrayExpr     = Expression.Parameter(typeof(IEnumerable<T>));
            ParameterExpression rExpr         = Expression.Variable(elementType, "result");
            ParameterExpression enumeratorVar = Expression.Variable(enumeratorType);
            MethodCallExpression moveNextCall = Expression.Call(
                enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext") ?? throw new InvalidOperationException());
            LabelTarget breakLabel = Expression.Label(elementType);
            return Expression.Lambda<Func<IEnumerable<T>, T>>(
                                 Expression.Block(
                                     new[] { enumeratorVar, rExpr },
                                     Expression.Assign(
                                         enumeratorVar,
                                         Expression.Call(
                                             arrayExpr,
                                             enumerableType.GetMethod("GetEnumerator") ??
                                             throw new InvalidOperationException())),
                                     Expression.Loop(
                                         Expression.IfThenElse(
                                             Expression.Equal(moveNextCall, Expression.Constant(true)),
                                             Expression.Assign(
                                                 rExpr,
                                                 Expression.Add(rExpr, Expression.Property(enumeratorVar, "Current"))),
                                             Expression.Break(breakLabel, rExpr)
                                         ),
                                         breakLabel)), arrayExpr)
                             .Compile(false);
        }
    }
}
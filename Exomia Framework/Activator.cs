#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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
using System.Linq.Expressions;
using System.Reflection;

namespace Exomia.Framework
{
    /// <summary>
    ///     A delegate that can be used to repeatedly create instances of type T using a specific constructor.
    ///     You cannot use the same delegate for different constructors.
    ///     You should create a delegate using GetCreator for each constructor that you need to call.
    /// </summary>
    /// <typeparam name="T">The type of object that this delegate will create.</typeparam>
    /// <param name="parameters">The parameters to create the object with.</param>
    public delegate T Creator<out T>(params object[] parameters);

    /// <summary>
    ///     Activator class
    /// </summary>
    public static class Activator
    {
        /// <summary>
        ///     Get a compiled lambda expression that can then be used
        ///     to repeatedly create instances of that type using the constructor that
        ///     the compiled lambda expression was created from.
        /// </summary>
        /// <typeparam name="TDelegate">The type of object that the compiled lambda expression will create.</typeparam>
        /// <returns>A compiled lambda expression in the form of a delegate.</returns>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static TDelegate GetCreator<TDelegate>()
            where TDelegate : class
        {
            Type dType = typeof(TDelegate);
            if (!dType.IsSubclassOf(typeof(Delegate)))
            {
                throw new NotSupportedException($"System.Delegate is not assignable from {dType}.");
            }

            MethodInfo mi = dType.GetMethod("Invoke") ?? throw new NullReferenceException("get method Invoke");
            if (mi.ReturnType == typeof(void)) { throw new NotSupportedException("invalid return type (void)"); }

            Type[] pTypes = Array.ConvertAll(mi.GetParameters(), p => p.ParameterType);
            ParameterExpression[] parameters = new ParameterExpression[pTypes.Length];
            for (int i = 0; i < pTypes.Length; ++i)
            {
                parameters[i] = Expression.Parameter(pTypes[i]);
            }

            return (TDelegate)Convert.ChangeType(
                Expression.Lambda(
                        dType,
                        Expression.New(
                            mi.ReturnType.GetConstructor(pTypes)
                            ?? throw new NullReferenceException(

                                // ReSharper disable once CoVariantArrayConversion (can be disabled cause we do no write operations at run-time)
                                $"can not create a constructor for {typeof(TDelegate)}"), parameters), parameters)
                    .Compile(), dType);
        }

        /// <summary>
        ///     Get a compiled lambda expression that can then be used
        ///     to repeatedly create instances of that type using the constructor that
        ///     the compiled lambda expression was created from.
        ///     Uses the constructor that matches the constructor parameters passed in.
        /// </summary>
        /// <typeparam name="TRes">The type of object that the compiled lambda expression will create.</typeparam>
        /// <param name="constructorParameters">An ordered array of the parameters the constructor takes.</param>
        /// <returns>A compiled lambda expression in the form of a delegate.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when T does not have a constructor that takes the passed in set of parameters.
        /// </exception>
        public static Creator<TRes> GetCreator<TRes>(params Type[] constructorParameters) where TRes : class
        {
            if (constructorParameters == null) { constructorParameters = Type.EmptyTypes; }

            ParameterExpression param = Expression.Parameter(typeof(object[]));
            Expression[] argsExpressions = new Expression[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; ++i)
            {
                argsExpressions[i] =
                    Expression.Convert(
                        Expression.ArrayIndex(param, Expression.Constant(i)),
                        constructorParameters[i]);
            }

            return (Creator<TRes>)Expression.Lambda(
                    typeof(Creator<TRes>),
                    Expression.New(
                        typeof(TRes).GetConstructor(constructorParameters)
                        ?? throw new ArgumentException(
                            @"This type does not have a constructor that takes the passed in set of parameters.",
                            nameof(constructorParameters)), argsExpressions), param)
                .Compile();
        }
    }
}
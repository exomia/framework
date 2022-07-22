#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Linq.Expressions;
using System.Reflection;

namespace Exomia.Framework.Core;

/// <summary>
///     A delegate that can be used to repeatedly create instances of type T using a specific constructor. You cannot use
///     the same delegate for different constructors. You
///     should create a delegate using GetCreator for each constructor that you need to call.
/// </summary>
/// <typeparam name="T"> The type of object that this delegate will create. </typeparam>
/// <param name="parameters"> The parameters to create the object with. </param>
/// <returns> A T. </returns>
public delegate T Creator<out T>(params object[] parameters);

/// <summary> Activator class. </summary>
public static class Activator
{
    /// <summary>
    ///     Get a compiled lambda expression that can then be used to repeatedly create instances of that type using the
    ///     constructor that the compiled lambda expression was created
    ///     from.
    /// </summary>
    /// <param name="dType"> The type. </param>
    /// <exception cref="NullReferenceException"> Thrown when a value was unexpectedly null. </exception>
    /// <exception cref="NotSupportedException">  Thrown when the requested operation is not supported. </exception>
    /// ###
    /// <returns> A compiled lambda expression in the form of a delegate. </returns>
    public static Delegate GetCreator(Type dType)
    {
        MethodInfo mi = dType.GetMethod("Invoke") ?? throw new NullReferenceException("Get method Invoke");
        if (mi.ReturnType == typeof(void)) { throw new NotSupportedException("Invalid return type (void)"); }

        Type[]                pTypes     = Array.ConvertAll(mi.GetParameters(), p => p.ParameterType);
        ParameterExpression[] parameters = Array.ConvertAll(pTypes,             Expression.Parameter);

        return
            Expression
                .Lambda(
                    dType,
                    Expression.New(
                        mi.ReturnType.GetConstructor(pTypes)
                        ?? throw new NullReferenceException(
                            // ReSharper disable once CoVariantArrayConversion (can be disabled cause we do no write operations at run-time)
                            $"Can not create a constructor for {dType}"), parameters),
                    parameters)
                .Compile();
    }

    /// <summary>
    ///     Get a compiled lambda expression that can then be used to repeatedly create instances of that type using the
    ///     constructor that the compiled lambda expression was created
    ///     from.
    /// </summary>
    /// <typeparam name="TDelegate"> The type of object that the compiled lambda expression will create. </typeparam>
    /// <returns> A compiled lambda expression in the form of a delegate. </returns>
    public static TDelegate GetCreator<TDelegate>()
        where TDelegate : Delegate
    {
        Type dType = typeof(TDelegate);
        return (TDelegate)Convert.ChangeType(GetCreator(dType), dType);
    }

    /// <summary>
    ///     Get a compiled lambda expression that can then be used to repeatedly create instances of that type using the
    ///     constructor that the compiled lambda expression was created
    ///     from. Uses the constructor that matches the constructor parameters passed in.
    /// </summary>
    /// <typeparam name="TRes"> The type of object that the compiled lambda expression will create. </typeparam>
    /// <param name="constructorParameters"> An ordered array of the parameters the constructor takes. </param>
    /// <returns> A compiled lambda expression in the form of a delegate. </returns>
    /// <exception cref="ArgumentException">
    ///     Thrown when T does not have a constructor that takes the passed in set of
    ///     parameters.
    /// </exception>
    public static Creator<TRes> GetCreator<TRes>(params Type[] constructorParameters)
    {
        constructorParameters ??= Type.EmptyTypes;

        ParameterExpression param           = Expression.Parameter(typeof(object[]));
        Expression[]        argsExpressions = new Expression[constructorParameters.Length];

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
                                                    "This type does not have a constructor that takes the passed in set of parameters.",
                                                    nameof(constructorParameters)), argsExpressions), param)
                                        .Compile();
    }

    /// <summary>
    ///     Get a compiled lambda expression that can then be used to repeatedly create instances of that type using the
    ///     constructor that the compiled lambda expression was created
    ///     from. Uses the constructor that matches the constructor parameters passed in.
    /// </summary>
    /// <typeparam name="TRes"> The type of object that the compiled lambda expression will create. </typeparam>
    /// <param name="constructorParameters"> An ordered array of the parameters the constructor takes. </param>
    /// <returns> A compiled lambda expression in the form of a delegate. </returns>
    public static Creator<TRes> GetCreator<TRes>(IEnumerable<Type> constructorParameters)
    {
        return GetCreator<TRes>(constructorParameters.ToArray());
    }
}
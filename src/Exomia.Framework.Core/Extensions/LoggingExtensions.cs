#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Game;
using Exomia.Logging;
using IServiceProvider = Exomia.IoC.IServiceProvider;

namespace Exomia.Framework.Core.Extensions;

/// <summary> A logging extensions. </summary>
public static class LoggingExtensions
{
    /// <summary> An <see cref="IGameBuilder" /> extension method that enables you to use logging. </summary>
    /// <param name="gameBuilder">           The gameBuilder to act on. </param>
    /// <param name="implementationFactory"> (Optional) The implementation factory. </param>
    /// <returns> An <see cref="IGameBuilder" />. </returns>
    /// <exception cref="NotSupportedException"> Thrown when the requested operation is not supported. </exception>
    public static IGameBuilder UseLogging(this IGameBuilder gameBuilder, Func<IServiceProvider, Type, object>? implementationFactory = null)
    {
        gameBuilder.ConfigureServices(serviceCollection =>
        {
            serviceCollection
                .Add(typeof(ILogger),
                    (serviceProvider, type) => implementationFactory != null
                        ? implementationFactory(serviceProvider, type)
                        : System.Activator.CreateInstance(
                              typeof(NullLogger))
                          ?? throw new NotSupportedException(type.ToString()))
                .Add(typeof(ILogger<>),
                    (serviceProvider, type) => implementationFactory != null
                        ? implementationFactory(serviceProvider, type)
                        : System.Activator.CreateInstance(
                              typeof(NullLogger<>).MakeGenericType(type.GetGenericArguments()))
                          ?? throw new NotSupportedException(type.ToString()));
        });

        return gameBuilder;
    }
}
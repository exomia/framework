#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Configurations;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Application;

/// <summary> Interface for application builder. </summary>
public interface IApplicationBuilder : IDisposable
{
    /// <summary> Configure options. </summary>
    /// <typeparam name="TConfiguration"> Type of the configuration. </typeparam>
    /// <param name="configure"> The configure delegate. </param>
    /// <returns> An <see cref="IApplicationBuilder" />. </returns>
    IApplicationBuilder Configure<TConfiguration>(Action<TConfiguration, IServiceProvider> configure)
        where TConfiguration : IConfigurableConfiguration;

    /// <summary> Configure services. </summary>
    /// <param name="configure"> The configure delegate. </param>
    /// <returns> An <see cref="IApplicationBuilder" />. </returns>
    IApplicationBuilder ConfigureServices(Action<IServiceCollection> configure);

    /// <summary> Registers the disposable described by disposable. </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="disposable"> The disposable. </param>
    /// <returns> A T. </returns>
    T RegisterDisposable<T>(T disposable) where T : IDisposable;

    /// <summary> Builds the <typeparamref name="TApplication" /> with its dependencies. </summary>
    /// <typeparam name="TApplication"> Type of the application. </typeparam>
    /// <returns> A <typeparamref name="TApplication" />. </returns>
    TApplication Build<TApplication>() where TApplication : Application;
}
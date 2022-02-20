#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.IoC;

/// <summary> Interface for a service collection. </summary>
public interface IServiceCollection
{
    /// <summary> Adds a service to the <see cref="IServiceCollection" />. </summary>
    /// <param name="service">        The service type. </param>
    /// <param name="implementation"> The implementation type. </param>
    /// <param name="serviceKind">    (Optional) The service kind. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add(Type service, Type implementation, ServiceKind serviceKind = ServiceKind.Transient);

    /// <summary> Adds a service to the <see cref="IServiceCollection" />. </summary>
    /// <typeparam name="TImplementation"> Type of the implementation. </typeparam>
    /// <param name="serviceKind"> (Optional) The service kind. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add<TImplementation>(ServiceKind serviceKind = ServiceKind.Transient)
        where TImplementation : class;

    /// <summary> Adds a service to the <see cref="IServiceCollection" />. </summary>
    /// <typeparam name="TService">        Type of the service. </typeparam>
    /// <typeparam name="TImplementation"> Type of the implementation. </typeparam>
    /// <param name="serviceKind"> (Optional) The service kind. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add<TService, TImplementation>(ServiceKind serviceKind = ServiceKind.Transient)
        where TService : class
        where TImplementation : class, TService;

    /// <summary> Adds a service to the <see cref="IServiceCollection" /> via the <paramref name="implementationFactory" />. </summary>
    /// <param name="service">               The service type. </param>
    /// <param name="implementationFactory"> The implementation factory. </param>
    /// <param name="serviceKind">           (Optional) The service kind. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add(Type service, Func<IServiceProvider, object> implementationFactory, ServiceKind serviceKind = ServiceKind.Transient);

    /// <summary> Adds a service to the <see cref="IServiceCollection" /> via the <paramref name="implementationFactory" />. </summary>
    /// <typeparam name="TService"> Type of the service. </typeparam>
    /// <param name="implementationFactory"> The implementation factory. </param>
    /// <param name="serviceKind">           (Optional) The service kind. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add<TService>(Func<IServiceProvider, TService> implementationFactory, ServiceKind serviceKind = ServiceKind.Transient)
        where TService : class;

    /// <summary> Adds a service to the <see cref="IServiceCollection" /> via the <paramref name="implementationFactory" />. </summary>
    /// <param name="service">               The service type. </param>
    /// <param name="implementationFactory"> The implementation factory. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add(Type service, Func<IServiceProvider, Type, object> implementationFactory);

    /// <summary> Adds a service to the <see cref="IServiceCollection" /> via the <paramref name="implementationFactory" />. </summary>
    /// <typeparam name="TService"> Type of the service. </typeparam>
    /// <param name="implementationFactory"> The implementation factory. </param>
    /// <returns> An <see cref="IServiceCollection" />. </returns>
    IServiceCollection Add<TService>(Func<IServiceProvider, Type, TService> implementationFactory)
        where TService : class;

    /// <summary> Builds the <see cref="IServiceCollection" /> to provide a <see cref="IServiceProvider" /> to get services from. </summary>
    /// <param name="serviceProvider"> (Optional) The parented service provider. </param>
    /// <returns> An <see cref="IServiceProvider" />. </returns>
    IServiceProvider Build(IServiceProvider? serviceProvider = null);
}
#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;

namespace Exomia.Framework.Core.IOC
{
    /// <summary> Collection of services. This class cannot be inherited. </summary>
    public sealed class ServiceCollection : IServiceCollection
    {
        private readonly Dictionary<Type, (Type, ServiceKind)>                           _typeEntries        = new(16);
        private readonly Dictionary<Type, (Func<IServiceProvider, object>, ServiceKind)> _factoryEntries     = new(16);
        private readonly Dictionary<Type, Func<IServiceProvider, Type, object>>          _factoryTypeEntries = new(16);

        /// <inheritdoc/>
        public IServiceCollection Add(Type service, Type implementation, ServiceKind serviceKind = ServiceKind.Transient)
        {
            if (!implementation.IsClass || implementation.IsAbstract)
            {
                throw new NotSupportedException("The implementation type must be a class and not abstract!");
            }

            if (!service.IsAssignableFrom(implementation))
            {
                throw new InvalidCastException("The service must be assignable from the implementation type");
            }

            _typeEntries.Add(service, (implementation, serviceKind));
            return this;
        }

        /// <inheritdoc/>
        public IServiceCollection Add<TImplementation>(ServiceKind serviceKind = ServiceKind.Transient)
            where TImplementation : class
        {
            return Add(typeof(TImplementation), typeof(TImplementation), serviceKind);
        }

        /// <inheritdoc/>
        public IServiceCollection Add<TService, TImplementation>(ServiceKind serviceKind = ServiceKind.Transient)
            where TService : class
            where TImplementation : class, TService
        {
            return Add(typeof(TService), typeof(TImplementation), serviceKind);
        }

        /// <inheritdoc/>
        public IServiceCollection Add(Type service, Func<IServiceProvider, object> implementationFactory, ServiceKind serviceKind = ServiceKind.Transient)
        {
            _factoryEntries.Add(service, (implementationFactory, serviceKind));
            return this;
        }

        /// <inheritdoc/>
        public IServiceCollection Add<TService>(Func<IServiceProvider, TService> implementationFactory, ServiceKind serviceKind = ServiceKind.Transient)
            where TService : class
        {
            return Add(typeof(TService), implementationFactory, serviceKind);
        }

        /// <inheritdoc/>
        public IServiceCollection Add(Type service, Func<IServiceProvider, Type, object> implementationFactory)
        {
            _factoryTypeEntries.Add(service, implementationFactory);
            return this;
        }

        /// <inheritdoc/>
        public IServiceCollection Add<TService>(Func<IServiceProvider, Type, TService> implementationFactory) where TService : class
        {
            return Add(typeof(TService), implementationFactory);
        }

        /// <inheritdoc/>
        public IServiceProvider Build(IServiceProvider? serviceProvider = null)
        {
            try
            {
                return ServiceProvider.Create(
                    serviceProvider,
                    _typeEntries,
                    _factoryEntries,
                    _factoryTypeEntries);
            }
            finally
            {
                _typeEntries.Clear();
                _factoryEntries.Clear();
                _factoryTypeEntries.Clear();
            }
        }
    }
}
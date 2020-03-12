#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;

namespace Exomia.Framework
{
    /// <summary>
    ///     A service registry. This class cannot be inherited.
    /// </summary>
    sealed class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, object> _registeredServices;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegistry" /> class.
        /// </summary>
        public ServiceRegistry()
        {
            _registeredServices = new Dictionary<Type, object>();
        }

        /// <inheritdoc />
        public void AddService(Type type, object provider)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }

            if (!type.IsInstanceOfType(provider))
            {
                throw new ArgumentException(
                    $"Service [{provider.GetType().FullName}] must be assignable to [{type.FullName}]");
            }

            lock (_registeredServices)
            {
                if (_registeredServices.ContainsKey(type))
                {
                    throw new ArgumentException("Service is already registered", nameof(type));
                }
                _registeredServices.Add(type, provider);
            }
        }

        /// <inheritdoc />
        public void AddService<T>(T provider)
        {
            AddService(typeof(T), provider!);
        }

        /// <inheritdoc />
        public object GetService(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            lock (_registeredServices)
            {
                if (_registeredServices.TryGetValue(type, out object obj))
                {
                    return obj;
                }
            }

            throw new ArgumentException($"Service of type {type} is not registered.");
        }

        /// <inheritdoc />
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        /// <inheritdoc />
        public bool RemoveService(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            lock (_registeredServices)
            {
                return _registeredServices.Remove(type);
            }
        }
    }
}
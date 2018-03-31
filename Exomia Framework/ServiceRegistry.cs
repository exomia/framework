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
using System.Collections.Generic;

namespace Exomia.Framework
{
    /// <summary>
    ///     base implementation of a <see cref="IServiceRegistry" /> interface
    /// </summary>
    public sealed class ServiceRegistry : IServiceRegistry
    {
        #region Variables

        private readonly Dictionary<Type, object> _registeredServices;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegistry" /> class.
        /// </summary>
        public ServiceRegistry()
        {
            _registeredServices = new Dictionary<Type, object>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     <see cref="IServiceRegistry.AddService(Type, object)" />
        /// </summary>
        public void AddService(Type type, object provider)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }

            if (!type.IsAssignableFrom(provider.GetType()))
            {
                throw new ArgumentException(
                    $"Service [{provider.GetType().FullName}] must be assignable to [{type.GetType().FullName}]");
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

        /// <summary>
        ///     <see cref="IServiceRegistry.AddService{T}(T)" />
        /// </summary>
        public void AddService<T>(T provider)
        {
            AddService(typeof(T), provider);
        }

        /// <summary>
        ///     <see cref="IServiceRegistry.GetService(Type)" />
        /// </summary>
        public object GetService(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            object obj = null;
            lock (_registeredServices)
            {
                if (_registeredServices.TryGetValue(type, out obj))
                {
                    return obj;
                }
            }

            throw new ArgumentException($"Service of type {type} is not registered.");
        }

        /// <summary>
        ///     <see cref="IServiceRegistry.GetService{T}" />
        /// </summary>
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        /// <summary>
        ///     <see cref="IServiceRegistry.RemoveService(Type)" />
        /// </summary>
        public bool RemoveService(Type type)
        {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }

            lock (_registeredServices)
            {
                return _registeredServices.Remove(type);
            }
        }

        #endregion
    }
}
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
    /// <inheritdoc />
    public sealed class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<Type, object> _registeredServices;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void AddService<T>(T provider)
        {
            AddService(typeof(T), provider);
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
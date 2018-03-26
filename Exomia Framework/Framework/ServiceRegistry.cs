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

        #region Statics

        #endregion

        private readonly Dictionary<Type, object> _registeredServices;

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceRegistry" /> class.
        /// </summary>
        public ServiceRegistry()
        {
            _registeredServices = new Dictionary<Type, object>();
        }

        #endregion

        #region Constants

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

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
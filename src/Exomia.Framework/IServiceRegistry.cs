#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework
{
    /// <summary>
    ///     Used for query after all types of services or add them.
    /// </summary>
    public interface IServiceRegistry
    {
        /// <summary>
        ///     Adds a service to this service provider.
        /// </summary>
        /// <param name="type">     The type of service to add. </param>
        /// <param name="provider"> The instance of the service provider to add. </param>
        /// <returns>
        ///     The <paramref name="provider" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Service type cannot be null. </exception>
        /// <exception cref="ArgumentException">     Service is already registered. </exception>
        object AddService(Type type, object provider);

        /// <summary>
        ///     Adds a service to this service provider.
        /// </summary>
        /// <typeparam name="T"> The type of the service to add. </typeparam>
        /// <param name="provider"> The instance of the service provider to add. </param>
        /// <returns>
        ///     The <paramref name="provider" />.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Service type cannot be null. </exception>
        /// <exception cref="ArgumentException">     Service is already registered. </exception>
        T AddService<T>(T provider);

        /// <summary>
        ///     Gets the service object of specified type.
        /// </summary>
        /// <param name="type"> The type of service to add. </param>
        /// <returns>
        ///     The service instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Is thrown when the type is null. </exception>
        /// <exception cref="ArgumentException">
        ///     Is thrown when the corresponding service is not
        ///     registered.
        /// </exception>
        /// <remarks>
        ///     This method will throw an exception if the service is not registered.
        /// </remarks>
        object GetService(Type type);

        /// <summary>
        ///     Gets the service object of specified type. The service must be registered with the
        ///     <typeparamref name="T" /> type key.
        /// </summary>
        /// <typeparam name="T"> The type of the service to get. </typeparam>
        /// <returns>
        ///     The service instance.
        /// </returns>
        /// <exception cref="ArgumentNullException"> Is thrown when the type is null. </exception>
        /// <exception cref="ArgumentException">
        ///     Is thrown when the corresponding service is not
        ///     registered.
        /// </exception>
        /// <remarks>
        ///     This method will throw an exception if the service is not registered.
        /// </remarks>
        T GetService<T>();

        /// <summary>
        ///     Removes the object providing a specified service.
        /// </summary>
        /// <param name="type"> The type of service. </param>
        /// <returns>
        ///     <b>true</b> if successfully removed; <b>false</b> otherwise.
        /// </returns>
        bool RemoveService(Type type);
    }
}
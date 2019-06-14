#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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
        /// <exception cref="ArgumentNullException"> Service type cannot be null. </exception>
        /// <exception cref="ArgumentException">     Service is already registered. </exception>
        void AddService(Type type, object provider);

        /// <summary>
        ///     Adds a service to this service provider.
        /// </summary>
        /// <typeparam name="T"> The type of the service to add. </typeparam>
        /// <param name="provider"> The instance of the service provider to add. </param>
        /// <exception cref="ArgumentNullException"> Service type cannot be null. </exception>
        /// <exception cref="ArgumentException">     Service is already registered. </exception>
        void AddService<T>(T provider);

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
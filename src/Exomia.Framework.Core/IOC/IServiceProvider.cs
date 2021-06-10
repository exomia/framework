using System;

namespace Exomia.Framework.Core.IOC
{
    /// <summary> Interface for service provider. </summary>
    public interface IServiceProvider
    {
        /// <summary> Gets the service specified with <typeparamref name="TService"/>. </summary>
        /// <typeparam name="TService"> Type of the service. </typeparam>
        /// <returns> The service instance of the <typeparamref name="TService"/>. </returns>
        TService Get<TService>();

        /// <summary> Gets the service specified with <paramref name="serviceType"/>. </summary>
        /// <param name="serviceType"> The service type to get. </param>
        /// <returns> The service instance of the <paramref name="serviceType"/>. </returns>
        object Get(Type serviceType);

        /// <summary> Tries to get the service specified with <typeparamref name="TService"/>. </summary>
        /// <typeparam name="TService"> Type of the service. </typeparam>
        /// <param name="service"> [out] The service. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool TryGet<TService>(out TService service);

        /// <summary> Tries to get the service specified with <paramref name="serviceType"/>. </summary>
        /// <param name="serviceType"> The service type to get. </param>
        /// <param name="service">     [out] The service. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool TryGet(Type serviceType, out object service);

        /// <summary> Creates a new scoped <see cref="IServiceProvider"/> out of this one. </summary>
        /// <param name="serviceCollection"> (Optional) Collection of services. </param>
        /// <returns> The new <see cref="IServiceProvider"/>. </returns>
        IServiceProvider CreateScope(IServiceCollection? serviceCollection = null);
    }
}
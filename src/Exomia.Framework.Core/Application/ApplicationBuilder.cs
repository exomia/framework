#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application.Configurations;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ApplicationConfiguration = Exomia.Framework.Core.Vulkan.Configurations.ApplicationConfiguration;

namespace Exomia.Framework.Core.Application;

/// <summary> An application builder. This class cannot be inherited. </summary>
public sealed class ApplicationBuilder : IApplicationBuilder
{
    private readonly IDictionary<Type, IList<Action<object, IServiceProvider>>> _configurables;
    private readonly IList<Action<IServiceCollection>>                          _configurableServices;
    private readonly DisposeCollector                                           _disposeCollector;

    /// <summary> Prevents a default instance of the <see cref="ApplicationBuilder" /> class from being created. </summary>
    private ApplicationBuilder()
    {
        _configurables        = new Dictionary<Type, IList<Action<object, IServiceProvider>>>(16);
        _configurableServices = new List<Action<IServiceCollection>>(16);
        _disposeCollector     = new DisposeCollector();
    }

    /// <inheritdoc />
    public IApplicationBuilder Configure<TConfiguration>(Action<TConfiguration, IServiceProvider> configure)
        where TConfiguration : IConfigurableConfiguration
    {
        if (!_configurables.TryGetValue(typeof(TConfiguration), out IList<Action<object, IServiceProvider>>? list))
        {
            _configurables.Add(typeof(TConfiguration), list = new List<Action<object, IServiceProvider>>(4));
        }
        list.Add(((configuration, provider) => { configure.Invoke((TConfiguration)configuration, provider); }));
        return this;
    }

    /// <inheritdoc />
    public IApplicationBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        _configurableServices.Add(configure);
        return this;
    }

    /// <inheritdoc />
    public T RegisterDisposable<T>(T disposable)
        where T : IDisposable
    {
        return _disposeCollector.Collect(disposable);
    }

    /// <inheritdoc />
    public TApplication Build<TApplication>() where TApplication : Application
    {
        IServiceCollection appServiceCollection = new ServiceCollection()
                                                  /* vulkan */
                                                  .AddSingleton<Vulkan.Vulkan>()
                                                  /* application */
                                                  .AddSingleton<TApplication>()
                                                  .AddSingleton<Application>(p => p.GetRequiredService<TApplication>());

        /* vulkan options */
        AddOptions<ApplicationConfiguration>(appServiceCollection);
        AddOptions<DebugUtilsMessengerConfiguration>(appServiceCollection);
        AddOptions<DepthStencilConfiguration>(appServiceCollection);
        AddOptions<DeviceConfiguration>(appServiceCollection);
        AddOptions<InstanceConfiguration>(appServiceCollection);
        AddOptions<PhysicalDeviceConfiguration>(appServiceCollection);
        AddOptions<QueueConfiguration>(appServiceCollection);
        AddOptions<SurfaceConfiguration>(appServiceCollection);
        AddOptions<SwapchainConfiguration>(appServiceCollection);

        /* application options */
        AddOptions<Configurations.ApplicationConfiguration>(appServiceCollection);
        AddOptions<RenderFormConfiguration>(appServiceCollection);

        /* custom user setup */
        foreach (Action<IServiceCollection> callback in _configurableServices)
        {
            callback.Invoke(appServiceCollection);
        }

        /* add null logging, if nothing else was provided! */
        appServiceCollection.TryAddSingleton<ILoggerFactory, NullLoggerFactory>();
        appServiceCollection.TryAddSingleton(typeof(ILogger),   typeof(NullLogger));
        appServiceCollection.TryAddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        IServiceProvider serviceProvider = appServiceCollection.BuildServiceProvider();

        Vulkan.Vulkan vulkan = _disposeCollector.Collect(serviceProvider.GetRequiredService<Vulkan.Vulkan>());
        if (!vulkan.Initialize())
        {
            _disposeCollector.RemoveAndDispose(ref vulkan);
            throw new VulkanException("Vulkan initialization failed!");
        }

        return serviceProvider.GetRequiredService<TApplication>();
    }

    /// <summary> Creates a new <see cref="IApplicationBuilder" />. </summary>
    /// <returns> An <see cref="IApplicationBuilder" />. </returns>
    public static IApplicationBuilder Create()
    {
        return new ApplicationBuilder();
    }

    private void AddOptions<TOption>(IServiceCollection serviceCollection) where TOption : class
    {
        if (_configurables.TryGetValue(typeof(TOption), out IList<Action<object, IServiceProvider>>? callbacks))
        {
            serviceCollection
                .AddOptions<TOption>()
                .Configure<IServiceProvider>(((configuration, provider) =>
                {
                    foreach (Action<object, IServiceProvider> callback in callbacks)
                    {
                        callback(configuration, provider);
                    }
                }));
            return;
        }

        serviceCollection.AddOptions<TOption>();
    }

    #region IDisposable Support

    private bool _disposed;

    /// <inheritdoc />
    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources.
    /// </summary>
    /// <param name="disposing"> true if user code; false called by finalizer. </param>
    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _disposeCollector.Dispose();
            }
            _disposed = true;
        }
    }

    /// <summary> Finalizes an instance of the <see cref="ApplicationBuilder" /> class. </summary>
    ~ApplicationBuilder()
    {
        Dispose(false);
    }

    #endregion
}
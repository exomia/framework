using System;
using System.Collections.Generic;
using Exomia.IoC;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.Framework.Core.Vulkan.Extensions;
using IServiceProvider = Exomia.IoC.IServiceProvider;

namespace Exomia.Framework.Core.Game
{
    /// <summary> A game builder. This class cannot be inherited. </summary>
    public sealed class GameBuilder : IGameBuilder
    {
        private          Action<IServiceCollection>?     _configureServices;
        private readonly IList<Action<IServiceProvider>> _configurables;
        private readonly DisposeCollector                _disposeCollector;

        /// <summary> Prevents a default instance of the <see cref="GameBuilder"/> class from being created. </summary>
        private GameBuilder()
        {
            _configurables    = new List<Action<IServiceProvider>>(16);
            _disposeCollector = new DisposeCollector();
        }

        /// <summary> Configure services. </summary>
        /// <param name="configureDelegate"> The configure delegate. </param>
        /// <returns> An <see cref="IGameBuilder"/>. </returns>
        public IGameBuilder ConfigureServices(Action<IServiceCollection> configureDelegate)
        {
            _configureServices += configureDelegate;
            return this;
        }

        /// <summary> Configure vulkan. </summary>
        /// <param name="configureDelegate"> The configure delegate. </param>
        /// <returns> An <see cref="IGameBuilder"/>. </returns>
        public IGameBuilder Configure<TConfiguration>(Action<IServiceProvider, TConfiguration> configureDelegate)
            where TConfiguration : class
        {
            _configurables.Add((provider => { configureDelegate.Invoke(provider, provider.Get<TConfiguration>()); }));
            return this;
        }

        /// <summary> Registers the disposable to automatically be disposed on shutdown. </summary>
        /// <typeparam name="T"> Generic type parameter. </typeparam>
        /// <param name="disposable"> The disposable. </param>
        /// <returns> An <see cref="IGameBuilder"/>. </returns>
        public T RegisterDisposable<T>(T disposable)
            where T : IDisposable
        {
            return _disposeCollector.Collect(disposable);
        }

        /// <summary> Gets the build. </summary>
        /// <typeparam name="TGame"> Type of the game. </typeparam>
        /// <returns> A <typeparamref name="TGame"/>. </returns>
        public TGame Build<TGame>() where TGame : Game
        {
            IServiceCollection appServiceCollection = new ServiceCollection()
                .AddVulkan()
                .Add<GameConfiguration>(ServiceKind.Singleton)
                .Add<RenderFormConfiguration>(ServiceKind.Singleton)
                .Add<TGame>(ServiceKind.Singleton)
                .Add<Game>(p => p.Get<TGame>(), ServiceKind.Singleton);

            _configureServices?.Invoke(appServiceCollection);

            IServiceProvider serviceProvider = appServiceCollection.Build();

            foreach (Action<IServiceProvider> configurable in _configurables)
            {
                configurable.Invoke(serviceProvider);
            }

            Vulkan.Vulkan vulkan = _disposeCollector.Collect(serviceProvider.Get<Vulkan.Vulkan>());
            if (!vulkan.Initialize())
            {
                _disposeCollector.RemoveAndDispose(ref vulkan);
                throw new VulkanException("Vulkan initialization failed!");
            }

            return serviceProvider.Get<TGame>();
        }

        /// <summary> Creates a new <see cref="IGameBuilder"/>. </summary>
        /// <returns> An <see cref="IGameBuilder"/>. </returns>
        public static IGameBuilder Create()
        {
            return new GameBuilder();
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

        /// <summary> Finalizes an instance of the <see cref="GameBuilder"/> class. </summary>
        ~GameBuilder()
        {
            Dispose(false);
        }

        #endregion
    }
}
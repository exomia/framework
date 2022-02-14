#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using IServiceProvider = Exomia.IoC.IServiceProvider;

namespace Exomia.Framework.Core.Game
{
    public abstract unsafe class Game : IRunnable
    {
        private const double                    FIXED_TIMESTAMP_THRESHOLD = 3.14159265359;
        private event EventHandler<Game, bool>? _IsRunningChanged;

        private readonly delegate*<void> _doEvents;

        private readonly IServiceProvider     _serviceProvider;
        private readonly ManualResetEventSlim _isShutdownCompleted;

        private readonly Vulkan.Vulkan _vulkan;

        private bool _isRunning, _isInitialized, _isContentLoaded, _shutdown;

        private readonly SpriteBatch _spriteBatch;

        /// <summary> Gets or sets a value indicating whether this object is fixed time step. </summary>
        /// <value> True if this object is fixed time step, false if not. </value>
        public bool IsFixedTimeStep { get; set; } = false;

        /// <summary> Gets or sets the target elapsed time in ms. </summary>
        /// <value> The target elapsed time in ms. </value>
        public double TargetElapsedTime { get; set; } = 1000.0 / 60.0;

        /// <inheritdoc />
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                if (_isRunning != value)
                {
                    _IsRunningChanged?.Invoke(this, value);
                    _isRunning = value;
                }
            }
        }

        /// <summary> Initializes a new instance of the <see cref="Game" /> class. </summary>
        /// <param name="serviceProvider"> The service provider. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        protected Game(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _isShutdownCompleted = new ManualResetEventSlim(true);

            _doEvents = serviceProvider.Get<GameConfiguration>().DoEvents;
            _vulkan   = serviceProvider.Get<Vulkan.Vulkan>();

            _spriteBatch = new SpriteBatch(_vulkan);

            IRenderForm renderForm = serviceProvider.Get<IRenderForm>();
            renderForm.Closing += (ref bool cancel) =>
            {
                if (!cancel)
                {
                    Shutdown();
                    _isShutdownCompleted.Wait(5 * 1000);
                }
            };
            renderForm.Show();
        }

        /// <inheritdoc />
        public void Run()
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("The instance is already running!");
            }

            _isRunning = true;
            //moep:
            //    Thread.Sleep(500);
            //    VkResult result = ((delegate*<VkDevice, VkSwapchainKHR, VkResult>)_vulkan.Context->Device
            //        .GetDeviceProcAddr("vkAcquireFullScreenExclusiveModeEXT"))(_vulkan.Context->Device, _vulkan.Context->Swapchain);
            //    Console.WriteLine(result);
            //    if (result != VK_SUCCESS)
            //    {
            //        goto moep;
            //    }

            if (!_isInitialized)
            {
                Renderloop();
            }
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            if (!_shutdown)
            {
                _isShutdownCompleted.Reset();
                _shutdown = true;
            }
        }

        private void Renderloop()
        {
            Stopwatch stopwatch = new Stopwatch();
            GameTime  gameTime  = GameTime.StartNew();

            void OnIsRunningChanged(Game s, bool v)
            {
                if (v) { gameTime.Start(); }
                else { gameTime.Stop(); }
            }

            _IsRunningChanged += OnIsRunningChanged;

            int   frames = 0;
            float timer  = 0;
            while (!_shutdown)
            {
                stopwatch.Restart();

                _doEvents();

                if (!_isRunning)
                {
                    Thread.Sleep(16);
                    continue;
                }

                timer += gameTime.DeltaTimeS;
                if (timer > 1.0f)
                {
                    timer -= 1.0f;
                    Console.WriteLine(frames);
                    frames = 0;
                }

                // scene update

                if (_vulkan.BeginFrame())
                {
                    _spriteBatch.Begin();

                    //for(int i = 0; i < 8_000; i++)
                    _spriteBatch.DrawFillRectangle(new RectangleF(50, 50, 100, 100), VkColors.White, 0.0f);
                    _spriteBatch.DrawFillRectangle(new RectangleF(75, 75, 100, 100), VkColors.Red,   1f);

                    _spriteBatch.End();

                    _vulkan.EndFrame();
                    frames++;
                }

                if (IsFixedTimeStep)
                {
                    //SLEEP
                    while (TargetElapsedTime - FIXED_TIMESTAMP_THRESHOLD > stopwatch.Elapsed.TotalMilliseconds)
                    {
                        Thread.Yield();
                    }

                    //IDLE
                    while (stopwatch.Elapsed.TotalMilliseconds < TargetElapsedTime) { }
                }

                gameTime.Tick();
            }

            _IsRunningChanged -= OnIsRunningChanged;

            _isShutdownCompleted.Set();
        }

        #region IDisposable Support

        private bool _disposed;

        /// <summary> Dispose. </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                OnDispose(disposing);
                if (disposing)
                {
                    _spriteBatch.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary> called once if disposed was called. </summary>
        /// <param name="disposing"> true for user code; false otherwise. </param>
        protected virtual void OnDispose(bool disposing) { }

        #endregion
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using Exomia.Framework.Game;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using Device4 = SharpDX.DXGI.Device4;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     The graphics device. This class cannot be inherited.
    /// </summary>
    public sealed class GraphicsDevice : IGraphicsDevice
    {
        /// <summary>
        ///     The feature levels.
        /// </summary>
        private static readonly FeatureLevel[] s_featureLevels =
        {
            FeatureLevel.Level_11_1, FeatureLevel.Level_11_0, FeatureLevel.Level_10_1, FeatureLevel.Level_10_0,
            FeatureLevel.Level_9_3, FeatureLevel.Level_9_2, FeatureLevel.Level_9_1
        };

        /// <summary>
        ///     <see cref="IGraphicsDevice.ResizeFinished" />
        /// </summary>
        public event EventHandler<ViewportF>? ResizeFinished;

        /// <summary>
        ///     The fourth adapter.
        /// </summary>
        private Adapter4? _adapter4;

        /// <summary>
        ///     The current render view.
        /// </summary>
        private RenderTargetView1? _currentRenderView;

        /// <summary>
        ///     The fifth d 3 d device.
        /// </summary>
        private Device5? _d3DDevice5;

        /// <summary>
        ///     Context for the 3 d device.
        /// </summary>
        private DeviceContext4? _d3DDeviceContext;

        /// <summary>
        ///     The depth stencil view.
        /// </summary>
        private DepthStencilView? _depthStencilView;

        /// <summary>
        ///     The fourth dxgi device.
        /// </summary>
        private Device4? _dxgiDevice4;

        /// <summary>
        ///     The dxgi factory.
        /// </summary>
        private Factory5? _dxgiFactory;

        /// <summary>
        ///     True to need resize.
        /// </summary>
        private bool _needResize;

        /// <summary>
        ///     The output.
        /// </summary>
        private Output? _output;

        /// <summary>
        ///     The first render view.
        /// </summary>
        private RenderTargetView1? _renderView1;

        /// <summary>
        ///     Options for controlling the resize.
        /// </summary>
        private ResizeParameters _resizeParameters;

        /// <summary>
        ///     The fourth swap chain.
        /// </summary>
        private SwapChain4? _swapChain4;

        /// <summary>
        ///     The synchronise.
        /// </summary>
        private int _vSync;

        /// <inheritdoc />
        public Adapter4 Adapter
        {
            get { return _adapter4!; }
        }

        /// <inheritdoc />
        public Device5 Device
        {
            get { return _d3DDevice5!; }
        }

        /// <inheritdoc />
        public DeviceContext4 DeviceContext
        {
            get { return _d3DDeviceContext!; }
        }

        /// <inheritdoc />
        public Device4 DxgiDevice
        {
            get { return _dxgiDevice4!; }
        }

        /// <inheritdoc />
        public Factory5 Factory
        {
            get { return _dxgiFactory!; }
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public RenderTargetView1 RenderView
        {
            get { return _renderView1!; }
        }

        /// <inheritdoc />
        public SwapChain4 SwapChain
        {
            get { return _swapChain4!; }
        }

        /// <inheritdoc />
        public ViewportF Viewport { get; private set; }

        /// <inheritdoc />
        public bool VSync
        {
            get { return _vSync != 0; }
            set { _vSync = value ? 1 : 0; }
        }

        /// <inheritdoc />
        public bool BeginFrame()
        {
            if (_needResize)
            {
                _needResize = false;
                Resize(_resizeParameters);
            }
            return IsInitialized;
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (_d3DDevice5!)
            {
                _d3DDeviceContext!.ClearRenderTargetView(_currentRenderView, Color.CornflowerBlue);
                _d3DDeviceContext.ClearDepthStencilView(
                    _depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
            }
        }

        /// <inheritdoc />
        public void Clear(Color color)
        {
            lock (_d3DDevice5!)
            {
                _d3DDeviceContext!.ClearRenderTargetView(_currentRenderView, color);
                _d3DDeviceContext.ClearDepthStencilView(
                    _depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
            }
        }

        /// <inheritdoc />
        public void EndFrame()
        {
            _swapChain4!.Present(_vSync, PresentFlags.None);
        }

        /// <inheritdoc />
        public bool GetFullscreenState()
        {
            return _swapChain4!.IsFullScreen;
        }

        /// <inheritdoc />
        public void Initialize(ref GameGraphicsParameters parameters)
        {
            if (IsInitialized) { return; }

            _vSync = parameters.UseVSync ? 1 : 0;

            ModeDescription modeDescription =
                new ModeDescription(
                    parameters.Width,
                    parameters.Height,
                    parameters.Rational,
                    parameters.Format);

            using (Factory4 factory4 = new Factory4())
            {
                _dxgiFactory = factory4.QueryInterface<Factory5>();
            }
            _dxgiFactory.MakeWindowAssociation(parameters.Handle, parameters.WindowAssociationFlags);

            _adapter4 = null!;

            for (int i = 0; i < _dxgiFactory.GetAdapterCount1(); i++)
            {
                //skip software adapters
                if ((_dxgiFactory.GetAdapter1(i).Description1.Flags & AdapterFlags.Software) == AdapterFlags.Software)
                {
                    continue;
                }

                Adapter4 a4 = _dxgiFactory.GetAdapter1(i).QueryInterface<Adapter4>();
                if (a4 == null) { continue; }

                int outputCount;
                if ((outputCount = a4.GetOutputCount()) <= 0) { continue; }

                for (int o = 0; o < outputCount; o++)
                {
                    Output output = a4.GetOutput(o);
                    output.GetClosestMatchingMode(
                        null,
                        modeDescription,
                        out ModeDescription desc);

                    if (o == 0
                     || desc.RefreshRate.Numerator / desc.RefreshRate.Denominator
                      > modeDescription.RefreshRate.Numerator / modeDescription.RefreshRate.Denominator)
                    {
                        modeDescription = desc;
                        _output         = output;
                    }
                }

                _adapter4 = a4;
                break;
            }

            Device defaultDevice;

            if (_adapter4 != null)
            {
                Console.WriteLine("------- GRAPHIC CARD INFORMATION -------");
                Console.WriteLine(
                    $"Description:\t\t{_adapter4.Description2.Description.TrimEnd('\t', ' ', '\r', '\n', (char)0)}");
                Console.WriteLine($"DeviceId:\t\t{_adapter4.Description2.DeviceId}");
                Console.WriteLine($"VendorId:\t\t{_adapter4.Description2.VendorId}");
                Console.WriteLine($"Revision:\t\t{_adapter4.Description2.Revision}");
                Console.WriteLine($"SubsystemId:\t\t{_adapter4.Description2.SubsystemId}");
#if x64
                Console.WriteLine();
                Console.WriteLine($"SystemMemory:\t\t{_adapter4.Description2.DedicatedSystemMemory}");
                Console.WriteLine($"VideoMemory:\t\t{_adapter4.Description2.DedicatedVideoMemory}");
                Console.WriteLine($"SharedSystemMemory:\t{_adapter4.Description2.SharedSystemMemory}");
#endif
                Console.WriteLine();
                Console.WriteLine($"Format:\t\t\t{modeDescription.Format}");
                Console.WriteLine($"Width x Height:\t\t{modeDescription.Width} x {modeDescription.Height}");
                Console.WriteLine(
                    $"RefreshRate:\t\t{modeDescription.RefreshRate} ({modeDescription.RefreshRate.Numerator / modeDescription.RefreshRate.Denominator}HZ)");
                Console.WriteLine($"Scaling:\t\t{modeDescription.Scaling}");
                Console.WriteLine($"ScanlineOrdering:\t{modeDescription.ScanlineOrdering}");
                Console.WriteLine();
                Console.WriteLine($"DeviceName:\t\t{_output!.Description.DeviceName}");
                Console.WriteLine(
                    $"DesktopBounds:\t\t{_output.Description.DesktopBounds.Left};{_output.Description.DesktopBounds.Top};{_output.Description.DesktopBounds.Right};{_output.Description.DesktopBounds.Bottom}");
                Console.WriteLine($"MonitorHandle:\t\t{_output.Description.MonitorHandle}");
                Console.WriteLine($"IsAttachedToDesktop:\t{_output.Description.IsAttachedToDesktop}");
                Console.WriteLine($"Rotation:\t\t{_output.Description.Rotation}");
                Console.WriteLine("----------------------------------------\n");

                defaultDevice = new Device(_adapter4, parameters.DeviceCreationFlags, s_featureLevels);
            }
            else
            {
                defaultDevice = new Device(parameters.DriverType, parameters.DeviceCreationFlags, s_featureLevels);
            }

            _d3DDevice5 = defaultDevice.QueryInterface<Device5>();
            defaultDevice.Dispose();

            _d3DDeviceContext = _d3DDevice5.ImmediateContext3.QueryInterface<DeviceContext4>();

            SampleDescription sampleDescription = new SampleDescription(1, 0);
            if (parameters.EnableMultiSampling && parameters.MultiSampleCount != MultiSampleCount.None)
            {
                sampleDescription.Count = (int)parameters.MultiSampleCount;
                sampleDescription.Quality =
                    Math.Max(
                        _d3DDevice5.CheckMultisampleQualityLevels(
                            modeDescription.Format,
                            sampleDescription.Count) - 1,
                        0);
            }

            SwapChainDescription swapChainDescription = new SwapChainDescription
            {
                BufferCount       = parameters.BufferCount,
                ModeDescription   = modeDescription,
                IsWindowed        = parameters.IsWindowed,
                OutputHandle      = parameters.Handle,
                SampleDescription = sampleDescription,
                SwapEffect        = parameters.SwapEffect,
                Usage             = parameters.Usage,
                Flags             = parameters.SwapChainFlags
            };

            SwapChain swapChain = new SwapChain(_dxgiFactory, _d3DDevice5, swapChainDescription);
            _swapChain4 = swapChain.QueryInterface<SwapChain4>();
            swapChain.Dispose();

            _dxgiDevice4 = _d3DDevice5.QueryInterface<Device4>();

            _resizeParameters = new ResizeParameters
            {
                BufferCount    = parameters.BufferCount,
                Width          = parameters.Width,
                Height         = parameters.Height,
                SwapChainFlags = parameters.SwapChainFlags
            };

            Resize(_resizeParameters);

            IsInitialized = true;
        }

        /// <inheritdoc />
        public void Resize(ref GameGraphicsParameters parameters)
        {
            _resizeParameters = new ResizeParameters
            {
                BufferCount    = parameters.BufferCount,
                Width          = parameters.Width,
                Height         = parameters.Height,
                SwapChainFlags = parameters.SwapChainFlags
            };
            _needResize = true;
        }

        /// <inheritdoc />
        public void Resize(int width, int height)
        {
            _resizeParameters = new ResizeParameters
            {
                BufferCount    = _swapChain4!.Description1.BufferCount,
                Width          = width,
                Height         = height,
                SwapChainFlags = _swapChain4.Description1.Flags
            };
            _needResize = true;
        }

        /// <inheritdoc />
        public void SetFullscreenState(bool state, Output? output = null)
        {
            if (_swapChain4!.IsFullScreen != state)
            {
                _swapChain4.SetFullscreenState(state, output);
            }
        }

        /// <inheritdoc />
        public void SetRenderTarget(RenderTargetView1? target)
        {
            lock (_d3DDevice5!)
            {
                _currentRenderView = target ?? _renderView1;
                _d3DDeviceContext!.OutputMerger.SetRenderTargets(_depthStencilView, _currentRenderView);
            }
        }

        /// <summary>
        ///     Resizes the given arguments.
        /// </summary>
        /// <param name="args"> The arguments. </param>
        private void Resize(ResizeParameters args)
        {
            lock (_d3DDevice5!)
            {
                if (_renderView1 != null)
                {
                    _renderView1.Dispose();
                    _renderView1 = null;
                }

                _swapChain4!.ResizeBuffers(
                    args.BufferCount, args.Width, args.Height, Format.Unknown, args.SwapChainFlags);

                RenderTargetView renderView;
                using (Texture2D backBuffer = _swapChain4.GetBackBuffer<Texture2D>(0))
                {
                    renderView   = new RenderTargetView(_d3DDevice5, backBuffer);
                    _renderView1 = renderView.QueryInterface<RenderTargetView1>();
                }
                renderView.Dispose();

                using (Texture2D depthBuffer = new Texture2D(
                    _d3DDevice5,
                    new Texture2DDescription
                    {
                        Format            = Format.D24_UNorm_S8_UInt,
                        ArraySize         = 1,
                        MipLevels         = 1,
                        Width             = args.Width,
                        Height            = args.Height,
                        SampleDescription = _swapChain4.Description.SampleDescription,
                        BindFlags         = BindFlags.DepthStencil,
                        CpuAccessFlags    = CpuAccessFlags.None,
                        OptionFlags       = ResourceOptionFlags.None,
                        Usage             = ResourceUsage.Default
                    }))
                {
                    _depthStencilView = new DepthStencilView(
                        _d3DDevice5, depthBuffer, new DepthStencilViewDescription
                        {
                            Dimension = _swapChain4.Description.SampleDescription.Count > 1 ||
                                        _swapChain4.Description.SampleDescription.Quality > 0
                                ? DepthStencilViewDimension.Texture2DMultisampled
                                : DepthStencilViewDimension.Texture2D
                        });
                }

                Viewport = new Viewport(0, 0, args.Width, args.Height);

                _d3DDeviceContext!.Rasterizer.SetViewport(Viewport);
                SetRenderTarget(null);
            }
            ResizeFinished?.Invoke(Viewport);
        }

        /// <summary>
        ///     A resize parameters.
        /// </summary>
        private struct ResizeParameters
        {
            /// <summary>
            ///     The width.
            /// </summary>
            public int Width;

            /// <summary>
            ///     The height.
            /// </summary>
            public int Height;

            /// <summary>
            ///     Number of buffers.
            /// </summary>
            public int BufferCount;

            /// <summary>
            ///     The swap chain flags.
            /// </summary>
            public SwapChainFlags SwapChainFlags;
        }

        #region IDisposable Support

        /// <summary>
        ///     True if disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    /* USER CODE */
                    Utilities.Dispose(ref _depthStencilView);
                    Utilities.Dispose(ref _renderView1);
                    Utilities.Dispose(ref _dxgiDevice4);
                    Utilities.Dispose(ref _d3DDeviceContext);
                    Utilities.Dispose(ref _d3DDevice5);
                    Utilities.Dispose(ref _swapChain4);
                    Utilities.Dispose(ref _output);
                    Utilities.Dispose(ref _adapter4);
                    Utilities.Dispose(ref _dxgiFactory);
                }
                _disposed = true;
            }
        }

        /// <inheritdoc />
        ~GraphicsDevice()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged/managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private static readonly PresentParameters s_defaultPresentParameters = new PresentParameters();

        private static readonly FeatureLevel[] s_featureLevels =
        {
            FeatureLevel.Level_11_1, FeatureLevel.Level_11_0, FeatureLevel.Level_10_1, FeatureLevel.Level_10_0,
            FeatureLevel.Level_9_3, FeatureLevel.Level_9_2, FeatureLevel.Level_9_1
        };

        /// <summary>
        ///     <see cref="IGraphicsDevice.ResizeFinished" />
        /// </summary>
        public event EventHandler<ViewportF>? ResizeFinished;

        private Adapter4?          _adapter4;
        private RenderTargetView1  _currentRenderView  = null!;
        private Device5            _d3DDevice5         = null!;
        private DeviceContext4     _d3DDeviceContext   = null!;
        private DepthStencilView   _depthStencilView   = null!;
        private Device4            _dxgiDevice4        = null!;
        private Factory5           _dxgiFactory        = null!;
        private Output6            _output6            = null!;
        private RenderTargetView1  _renderView1        = null!;
        private SwapChain4         _swapChain4         = null!;
        private BlendStates        _blendStates        = null!;
        private DepthStencilStates _depthStencilStates = null!;
        private RasterizerStates   _rasterizerStates   = null!;
        private SamplerStates      _samplerStates      = null!;
        private Textures           _textures           = null!;
        private ResizeParameters   _resizeParameters;
        private bool               _needResize;
        private int                _vSync;

        /// <inheritdoc />
        public Adapter4? Adapter
        {
            get { return _adapter4; }
        }

        /// <inheritdoc />
        public Device5 Device
        {
            get { return _d3DDevice5; }
        }

        /// <inheritdoc />
        public DeviceContext4 DeviceContext
        {
            get { return _d3DDeviceContext; }
        }

        /// <inheritdoc />
        public Device4 DxgiDevice
        {
            get { return _dxgiDevice4; }
        }

        /// <inheritdoc />
        public Factory5 Factory
        {
            get { return _dxgiFactory; }
        }

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public RenderTargetView1 RenderView
        {
            get { return _renderView1; }
        }

        /// <inheritdoc />
        public SwapChain4 SwapChain
        {
            get { return _swapChain4; }
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
        public BlendStates BlendStates
        {
            get { return _blendStates; }
        }

        /// <inheritdoc />
        public DepthStencilStates DepthStencilStates
        {
            get { return _depthStencilStates; }
        }

        /// <inheritdoc />
        public RasterizerStates RasterizerStates
        {
            get { return _rasterizerStates; }
        }

        /// <inheritdoc />
        public SamplerStates SamplerStates
        {
            get { return _samplerStates; }
        }

        /// <inheritdoc />
        public Textures Textures
        {
            get { return _textures; }
        }

        /// <inheritdoc />
        public void Clear()
        {
            Clear(Color.CornflowerBlue);
        }

        /// <inheritdoc />
        public void Clear(Color color)
        {
            _d3DDeviceContext.ClearDepthStencilView(
                _depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1f, 0);
            _d3DDeviceContext!.ClearRenderTargetView(_currentRenderView, color);
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
                BufferCount    = _swapChain4.Description1.BufferCount,
                Width          = width,
                Height         = height,
                SwapChainFlags = _swapChain4.Description1.Flags
            };
            _needResize = true;
        }

        /// <inheritdoc />
        public void SetFullscreenState(bool state, Output? output = null)
        {
            if (GetFullscreenState() != state)
            {
                _swapChain4.SetFullscreenState(state, output);
            }
        }

        /// <inheritdoc />
        public bool GetFullscreenState()
        {
            return _swapChain4.IsFullScreen;
        }

        /// <inheritdoc />
        public void SetRenderTarget(RenderTargetView1? target)
        {
            _currentRenderView = target ?? _renderView1;
            _d3DDeviceContext.OutputMerger.SetRenderTargets(_depthStencilView, _currentRenderView);
        }

        /// <summary>
        ///     Begins a frame.
        /// </summary>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool BeginFrame()
        {
            if (_needResize)
            {
                _needResize = false;
                Resize(_resizeParameters);
            }
            return IsInitialized;
        }

        /// <summary>
        ///     Ends a frame.
        /// </summary>
        public void EndFrame()
        {
            _swapChain4.Present(_vSync, PresentFlags.None, s_defaultPresentParameters);
        }

        /// <summary>
        ///     Initializes this object.
        /// </summary>
        /// <param name="parameters"> [in,out] Options for controlling the operation. </param>
        public void Initialize(ref GameGraphicsParameters parameters)
        {
            if (IsInitialized) { return; }

            _vSync = parameters.UseVSync ? 1 : 0;

            using (Factory4 factory4 = new Factory4())
            {
                _dxgiFactory = factory4.QueryInterface<Factory5>();
            }

            ModeDescription modeDescription =
                new ModeDescription(
                    parameters.Width,
                    parameters.Height,
                    parameters.Rational,
                    parameters.Format);

            if (parameters.AdapterLuid != -1 && parameters.OutputIndex != -1)
            {
                using (Adapter adapter = _dxgiFactory.GetAdapterByLuid(parameters.AdapterLuid))
                using (Output output = adapter.GetOutput(parameters.OutputIndex))
                {
                    _adapter4 = adapter.QueryInterface<Adapter4>();
                    _output6  = output.QueryInterface<Output6>();
                    _output6.GetClosestMatchingMode(
                        null,
                        modeDescription,
                        out modeDescription);
                }
            }
            else
            {
                IEnumerable<Adapter> adapters =
                    _dxgiFactory.Adapters1
                                .Where(
                                    a => (a.Description1.Flags & AdapterFlags.Software) !=
                                        AdapterFlags.Software && a.GetOutputCount() > 0)
                                .Select(
                                    a => (adapter: a,
                                          featureLevel: SharpDX.Direct3D11.Device.GetSupportedFeatureLevel(a)))
                                .GroupBy(t => t.featureLevel)
                                .OrderByDescending(t => t.Key)
                                .First()
                                .Select(k => k.adapter);

                _adapter4 = null;
                foreach (Adapter adapter in adapters)
                {
                    using (adapter)
                    {
                        for (int o = 0; o < adapter.GetOutputCount(); o++)
                        {
                            using (Output output = adapter.GetOutput(o))
                            {
                                output.GetClosestMatchingMode(
                                    null,
                                    modeDescription,
                                    out ModeDescription desc);
                                if (_adapter4 == null ||
                                    desc.RefreshRate.Numerator / desc.RefreshRate.Denominator >
                                    modeDescription.RefreshRate.Numerator / modeDescription.RefreshRate.Denominator)
                                {
                                    modeDescription = desc;
                                    Interlocked.Exchange(ref _output6, output.QueryInterface<Output6>())?.Dispose();
                                    Interlocked.Exchange(ref _adapter4, adapter.QueryInterface<Adapter4>())?.Dispose();
                                }
                            }
                        }
                    }
                }
            }

            // ReSharper disable once VariableHidesOuterVariable
            Device CreateDevice(in GameGraphicsParameters parameters)
            {
                if (_adapter4 != null)
                {
                    Console.WriteLine("------- GRAPHIC CARD INFORMATION -------");
                    Console.WriteLine($"Luid:\t\t\t{_adapter4.Description2.Luid}");
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
                    Console.WriteLine($"DeviceName:\t\t{_output6.Description.DeviceName}");
                    Console.WriteLine(
                        $"DesktopBounds:\t\t{_output6.Description.DesktopBounds.Left};{_output6.Description.DesktopBounds.Top};{_output6.Description.DesktopBounds.Right};{_output6.Description.DesktopBounds.Bottom}");
                    Console.WriteLine($"MonitorHandle:\t\t{_output6.Description.MonitorHandle}");
                    Console.WriteLine($"IsAttachedToDesktop:\t{_output6.Description.IsAttachedToDesktop}");
                    Console.WriteLine($"Rotation:\t\t{_output6.Description.Rotation}");
                    Console.WriteLine("----------------------------------------\n");

                    return new Device(_adapter4, parameters.DeviceCreationFlags, s_featureLevels);
                }
                return new Device(parameters.DriverType, parameters.DeviceCreationFlags, s_featureLevels);
            }

            using (Device defaultDevice = CreateDevice(in parameters))
            {
                _d3DDevice5 = defaultDevice.QueryInterface<Device5>();
            }

            _d3DDeviceContext = _d3DDevice5.ImmediateContext3.QueryInterface<DeviceContext4>();
            _dxgiDevice4      = _d3DDevice5.QueryInterface<Device4>();

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
                IsWindowed        = true,
                OutputHandle      = parameters.Handle,
                SampleDescription = sampleDescription,
                SwapEffect        = parameters.SwapEffect,
                Usage             = parameters.Usage,
                Flags             = parameters.SwapChainFlags
            };

            using (SwapChain swapChain = new SwapChain(_dxgiFactory, _d3DDevice5, swapChainDescription))
            {
                _swapChain4 = swapChain.QueryInterface<SwapChain4>();
            }

            _dxgiFactory.MakeWindowAssociation(parameters.Handle, parameters.WindowAssociationFlags);

            _swapChain4.ResizeTarget(ref modeDescription);

            SetFullscreenState(parameters.DisplayType == DisplayType.Fullscreen);

            _resizeParameters = new ResizeParameters
            {
                BufferCount    = parameters.BufferCount,
                Width          = parameters.Width,
                Height         = parameters.Height,
                SwapChainFlags = parameters.SwapChainFlags
            };

            Resize(_resizeParameters);

            _blendStates        = new BlendStates(this);
            _depthStencilStates = new DepthStencilStates(this);
            _rasterizerStates   = new RasterizerStates(this);
            _samplerStates      = new SamplerStates(this);
            _textures           = new Textures(this);

            IsInitialized = true;
        }

        private void Resize(ResizeParameters args)
        {
            Utilities.Dispose(ref _renderView1);
            Utilities.Dispose(ref _depthStencilView);

            _d3DDeviceContext.ClearState();

            _swapChain4.ResizeBuffers(
                args.BufferCount, args.Width, args.Height, Format.Unknown, args.SwapChainFlags);

            using (Texture2D backBuffer = _swapChain4.GetBackBuffer<Texture2D>(0))
            using (RenderTargetView renderView = new RenderTargetView(_d3DDevice5, backBuffer))
            {
                _renderView1 = renderView.QueryInterface<RenderTargetView1>();
            }

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

            _d3DDeviceContext.Rasterizer.SetViewport(Viewport = new Viewport(0, 0, args.Width, args.Height));

            SetRenderTarget(null);

            ResizeFinished?.Invoke(Viewport);
        }

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

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _blendStates.Dispose();
                    _depthStencilStates.Dispose();
                    _rasterizerStates.Dispose();
                    _samplerStates.Dispose();
                    
                    _textures.Dispose();

                    Utilities.Dispose(ref _depthStencilView);
                    Utilities.Dispose(ref _renderView1);
                    Utilities.Dispose(ref _dxgiDevice4);
                    Utilities.Dispose(ref _d3DDeviceContext);
                    Utilities.Dispose(ref _d3DDevice5);
                    Utilities.Dispose(ref _swapChain4);
                    Utilities.Dispose(ref _output6);
                    Utilities.Dispose(ref _adapter4);
                    Utilities.Dispose(ref _dxgiFactory);
                }
                _disposed = true;
            }
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="GraphicsDevice" /> class.
        /// </summary>
        ~GraphicsDevice()
        {
            Dispose(false);
        }

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
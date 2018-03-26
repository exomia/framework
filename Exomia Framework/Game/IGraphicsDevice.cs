#pragma warning disable 1591

using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device4 = SharpDX.DXGI.Device4;

namespace Exomia.Framework.Game
{
    public delegate void ResizeEventHandler(ViewportF viewport);

    public interface IGraphicsDevice : IDisposable
    {
        bool IsInitialized { get; }
        bool VSync { get; set; }

        Adapter4 Adapter { get; }

        Device5 Device { get; }
        DeviceContext4 DeviceContext { get; }

        RenderTargetView1 RenderView { get; }

        Device4 DXGIDevice { get; }

        SwapChain4 SwapChain { get; }

        Factory5 Factory { get; }

        ViewportF Viewport { get; }
        event ResizeEventHandler ResizeFinished;

        void Initialize(ref GameGraphicsParameters parameters);

        void Resize(ref GameGraphicsParameters parameters);
        void Resize(int width, int height);

        bool BeginFrame();
        void EndFrame();

        void Clear();
        void Clear(Color color);

        void SetRenderTarget(RenderTargetView1 target);

        void SetFullscreenState(bool state, Output output = null);
        bool GetFullscreenState();
    }
}
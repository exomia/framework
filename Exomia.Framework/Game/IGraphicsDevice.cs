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

#pragma warning disable 1591

using System;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device4 = SharpDX.DXGI.Device4;

namespace Exomia.Framework.Game
{
    public interface IGraphicsDevice : IDisposable
    {
        event EventHandler<ViewportF> ResizeFinished;
        bool IsInitialized { get; }

        Adapter4 Adapter { get; }

        Device5 Device { get; }

        DeviceContext4 DeviceContext { get; }

        Device4 DxgiDevice { get; }

        Factory5 Factory { get; }

        RenderTargetView1 RenderView { get; }

        SwapChain4 SwapChain { get; }

        ViewportF Viewport { get; }

        bool VSync { get; set; }

        void Initialize(ref GameGraphicsParameters parameters);

        bool BeginFrame();
        void EndFrame();

        void Clear();
        void Clear(Color color);

        void Resize(ref GameGraphicsParameters parameters);
        void Resize(int width, int height);

        void SetFullscreenState(bool state, Output output = null);
        bool GetFullscreenState();

        void SetRenderTarget(RenderTargetView1 target);
    }
}
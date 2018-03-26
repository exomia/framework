#pragma warning disable 1591

using System;
using SharpDX.Windows;

namespace Exomia.Framework.Game
{
    public interface IGameWindow : IDisposable
    {
        bool IsInitialized { get; }

        int Width { get; }

        int Height { get; }

        string Title { get; set; }

        void Initialize(ref GameGraphicsParameters parameters);

        void Resize(int width, int height);
    }

    public interface IWinFormsGameWindow : IGameWindow
    {
        RenderForm RenderForm { get; }
    }
}
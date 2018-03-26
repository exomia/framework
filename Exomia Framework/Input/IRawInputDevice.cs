#pragma warning disable 1591

using System.Windows.Forms;
using Exomia.Framework.Game;
using SharpDX.RawInput;

namespace Exomia.Framework.Input
{
    public delegate void RKeyEventHandler(Keys key, KeyState state, int extraInformation, RSpecialKeys sKeys);

    public delegate void RMouseEventHandler(int x, int y, RMouseButtons buttons, int clicks, int wheelDelta);

    public enum RMouseButtons
    {
        Left = 1 << 0,
        Middle = 1 << 1,
        Right = 1 << 2,
        Button4 = 1 << 3,
        Button5 = 1 << 4
    }

    public enum RSpecialKeys
    {
        None = 1 << 0,
        Shift = 1 << 1,
        Alt = 1 << 2,
        Control = 1 << 3
    }

    public interface IRawInputDevice
    {
        int WheelData { get; }
        event RKeyEventHandler KeyDown;
        event RKeyEventHandler KeyUp;
        event RKeyEventHandler KeyPress;

        event RMouseEventHandler MouseMove;
        event RMouseEventHandler MouseDown;
        event RMouseEventHandler MouseUp;

        void Initialize(IGameWindow window);
        void Initialize(Panel panel);

        void EndUpdate();

        bool IsMouseButtonDown(RMouseButtons button);
        bool IsMouseButtonDown(params RMouseButtons[] buttons);
        bool IsKeyUp(RMouseButtons button);
        bool IsKeyUp(params RMouseButtons[] buttons);

        bool IsKeyDown(Keys key);
        bool IsKeyUp(Keys key);
        bool IsKeyDown(params Keys[] key);
        bool IsKeyUp(params Keys[] key);
    }
}
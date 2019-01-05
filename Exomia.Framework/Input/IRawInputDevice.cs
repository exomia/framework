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
using System.Windows.Forms;
using Exomia.Framework.Game;
using SharpDX.RawInput;

namespace Exomia.Framework.Input
{
    public delegate void RKeyEventHandler(Keys key, KeyState state, int extraInformation, RSpecialKeys sKeys);

    public delegate void RMouseEventHandler(int x, int y, RMouseButtons buttons, int clicks, int wheelDelta);

    [Flags]
    public enum RMouseButtons
    {
        Left = 1 << 0,
        Middle = 1 << 1,
        Right = 1 << 2,
        Button4 = 1 << 3,
        Button5 = 1 << 4
    }

    [Flags]
    public enum RSpecialKeys
    {
        None = 1 << 0,
        Shift = 1 << 1,
        Alt = 1 << 2,
        Control = 1 << 3
    }

    public interface IRawInputDevice
    {
        event RKeyEventHandler KeyDown;
        event RKeyEventHandler KeyPress;
        event RKeyEventHandler KeyUp;
        event RMouseEventHandler MouseDown;

        event RMouseEventHandler MouseMove;
        event RMouseEventHandler MouseUp;

        int WheelData { get; }

        void EndUpdate();

        void Initialize(IGameWindow window);
        void Initialize(Panel panel);

        bool IsKeyDown(Keys key);
        bool IsKeyDown(params Keys[] key);
        bool IsKeyUp(RMouseButtons button);
        bool IsKeyUp(params RMouseButtons[] buttons);
        bool IsKeyUp(Keys key);
        bool IsKeyUp(params Keys[] key);

        bool IsMouseButtonDown(RMouseButtons button);
        bool IsMouseButtonDown(params RMouseButtons[] buttons);
    }
}
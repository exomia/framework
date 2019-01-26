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

namespace Exomia.Framework.Native
{
    static class User32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int DispatchMessage(ref MSG lpMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int PeekMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin,
            uint wMsgFilterMax,
            uint wRemoveMsg);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", SetLastError = true)]
        internal static extern int TranslateMessage(ref MSG lpMsg);

        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            public IntPtr hWnd;
            public int message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point pt;
        }
    }
}
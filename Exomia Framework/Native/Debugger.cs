#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
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

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Native
{
    /// <summary>
    ///     Debugger utils
    /// </summary>
    public static class Debugger
    {
        /// <summary>
        ///     detects if a native debugger is attached
        /// </summary>
        /// <param name="hProcess">process handle</param>
        /// <param name="isDebuggerPresent"><c>true</c> if a native debugger is attached; <c>false</c> otherwise</param>
        /// <returns><c>true</c> if handle failure; <c>false</c> otherwise</returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = false, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CheckRemoteDebuggerPresent(
            IntPtr hProcess,
            [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);
    }
}
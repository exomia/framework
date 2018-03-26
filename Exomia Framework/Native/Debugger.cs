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
#pragma warning disable 1591

using System;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Native
{
    public static class Diagnostic
    {
        /// <summary>
        ///     <see href=">https://msdn.microsoft.com/en-us/library/ms724400(VS.85).aspx" />
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemTimes(
            out FILETIME lpIdleTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        /// <summary>
        ///     <see href=">https://msdn.microsoft.com/en-us/library/ms683223(VS.85).aspx" />
        /// </summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetProcessTimes(
            IntPtr hProcess,
            out FILETIME lpCreationTime,
            out FILETIME lpExitTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        [StructLayout(LayoutKind.Sequential)]
        public struct FILETIME
        {
            public uint DwLowDateTime { get; private set; }
            public uint DwHighDateTime { get; private set; }

            public ulong Value
            {
                get { return ((ulong)DwHighDateTime << 32) + DwLowDateTime; }
            }
        }
    }
}
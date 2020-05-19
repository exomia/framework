using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Win32
{
    public static class Shell32
    {
        [SuppressUnmanagedCodeSecurity]
        [DllImport("shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern IntPtr ExtractIcon(IntPtr hInst, string exeFileName, int iconIndex);
    }
}

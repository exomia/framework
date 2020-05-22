using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Exomia.Framework.Win32
{
    static class Shell32
    {
        /// <summary>
        ///     Extracts the icon.
        /// </summary>
        /// <param name="hInst">       The instance. </param>
        /// <param name="exeFileName"> Filename of the executable file. </param>
        /// <param name="iconIndex">   Zero-based index of the icon. </param>
        /// <returns>
        ///     The extracted icon.
        /// </returns>
        [SuppressUnmanagedCodeSecurity]
        [DllImport("shell32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern IntPtr ExtractIcon(IntPtr hInst, string exeFileName, int iconIndex);
    }
}

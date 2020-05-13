using System.Runtime.InteropServices;

namespace Exomia.Framework.Win32.RawInput
{
    /// <summary>
    /// Contains the raw input from a device.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUT
    {
        /// <summary>
        ///     The header.
        /// </summary>
        public RAWINPUTHEADER Header;
        
        /// <summary>
        ///     The data.
        /// </summary>
        public Union Data;
        
        /// <summary>
        ///     An union.
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        public struct Union
        {
            /// <summary>
            /// Mouse raw input data.
            /// </summary>
            [FieldOffset(0)]
            public RAWINPUTMOUSE Mouse;
            /// <summary>
            /// Keyboard raw input data.
            /// </summary>
            [FieldOffset(0)]
            public RAWINPUTKEYBOARD Keyboard;
            /// <summary>
            /// HID raw input data.
            /// </summary>
            [FieldOffset(0)]
            public RAWINPUTHID HID;
        }
    }
}

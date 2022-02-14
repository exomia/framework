#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32
{
    /// <summary> A message. </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Message
    {
        /// <summary> The window. </summary>
        public IntPtr hWnd;

        /// <summary> The message. </summary>
        public uint msg;

        /// <summary> The w parameter. </summary>
        public IntPtr wParam;

        /// <summary> The l parameter. </summary>
        public IntPtr lParam;
    }
}
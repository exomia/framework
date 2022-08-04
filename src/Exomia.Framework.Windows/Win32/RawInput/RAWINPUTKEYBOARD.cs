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
namespace Exomia.Framework.Windows.Win32.RawInput;

[StructLayout(LayoutKind.Sequential)]
struct RAWINPUTKEYBOARD
{
    /// <summary> Scan code for key depression. </summary>
    public ushort MakeCode;

    /// <summary> Scan code information. </summary>
    public RawKeyboardFlags Flags;

    /// <summary> Reserved. </summary>
    public ushort Reserved;

    /// <summary> Virtual key code. </summary>
    public ushort VKey;

    /// <summary> Corresponding window message. </summary>
    public uint Message;

    /// <summary> Extra information. </summary>
    public uint ExtraInformation;
}
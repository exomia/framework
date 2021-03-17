#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32.RawInput
{
    [Flags]
    enum RawKeyboardFlags : ushort
    {
        /// <summary></summary>
        KeyMake = 0,

        /// <summary></summary>
        KeyBreak = 1,

        /// <summary></summary>
        KeyE0 = 2,

        /// <summary></summary>
        KeyE1 = 4,

        /// <summary></summary>
        TerminalServerSetLED = 8,

        /// <summary></summary>
        TerminalServerShadow = 0x10,

        /// <summary></summary>
        TerminalServerVKPACKET = 0x20
    }
}
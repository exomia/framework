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
namespace Exomia.Framework.Platform.Windows.Win32.RawInput
{
    /// <summary>
    ///     Enumeration containing the button data for raw mouse input.
    /// </summary>
    [Flags]
    enum RawMouseButtons : ushort
    {
        /// <summary>No button.</summary>
        None = 0,

        /// <summary>Left (button 1) down.</summary>
        LeftDown = 0x0001,

        /// <summary>Left (button 1) up.</summary>
        LeftUp = 0x0002,

        /// <summary>Right (button 2) down.</summary>
        RightDown = 0x0004,

        /// <summary>Right (button 2) up.</summary>
        RightUp = 0x0008,

        /// <summary>Middle (button 3) down.</summary>
        MiddleDown = 0x0010,

        /// <summary>Middle (button 3) up.</summary>
        MiddleUp = 0x0020,

        /// <summary>Button 4 down.</summary>
        Button4Down = 0x0040,

        /// <summary>Button 4 up.</summary>
        Button4Up = 0x0080,

        /// <summary>Button 5 down.</summary>
        Button5Down = 0x0100,

        /// <summary>Button 5 up.</summary>
        Button5Up = 0x0200,

        /// <summary>Mouse wheel moved.</summary>
        MouseWheel = 0x0400
    }
}
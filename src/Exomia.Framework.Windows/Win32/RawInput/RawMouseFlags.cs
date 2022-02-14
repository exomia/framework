#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion



// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
namespace Exomia.Framework.Windows.Win32.RawInput
{
    [Flags]
    internal enum RawMouseFlags : ushort
    {
        /// <summary>Relative to the last position.</summary>
        MoveRelative = 0,

        /// <summary>Absolute positioning.</summary>
        MoveAbsolute = 1,

        /// <summary>Coordinate data is mapped to a virtual desktop.</summary>
        VirtualDesktop = 2,

        /// <summary>Attributes for the mouse have changed.</summary>
        AttributesChanged = 4
    }
}
﻿#region License

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
namespace Exomia.Framework.Win32.RawInput
{
    /// <summary>Enumeration containing flags for a raw input device.</summary>
    [Flags]
    enum RawInputDeviceFlags
    {
        /// <summary>No flags.</summary>
        None = 0,

        /// <summary>
        ///     If set, this removes the top level collection from the inclusion list. This tells the operating system to stop
        ///     reading from a device which matches the top level collection.
        /// </summary>
        Remove = 0x00000001,

        /// <summary>
        ///     If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only
        ///     affects a TLC whose usage page is already specified with PageOnly.
        /// </summary>
        Exclude = 0x00000010,

        /// <summary>
        ///     If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that
        ///     Usage must be zero. To exclude a particular top level collection, use Exclude.
        /// </summary>
        PageOnly = 0x00000020,

        /// <summary>
        ///     If set, this prevents any devices specified by UsagePage or Usage from generating legacy messages. This is
        ///     only for the mouse and keyboard.
        /// </summary>
        NoLegacy = 0x00000030,

        /// <summary>
        ///     If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that
        ///     WindowHandle must be specified.
        /// </summary>
        InputSink = 0x00000100,

        /// <summary>If set, the mouse button click does not activate the other window.</summary>
        CaptureMouse = 0x00000200,

        /// <summary>
        ///     If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for
        ///     example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. NoHotKeys can
        ///     be specified even if NoLegacy is not specified and WindowHandle is NULL.
        /// </summary>
        NoHotKeys = 0x00000200,

        /// <summary>If set, application keys are handled.  NoLegacy must be specified.  Keyboard only.</summary>
        AppKeys = 0x00000400
    }
}
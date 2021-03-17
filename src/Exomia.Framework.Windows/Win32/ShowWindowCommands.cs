#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace Exomia.Framework.Windows.Win32
{
    enum ShowWindowCommands
    {
        /// <summary>
        ///     Hides the window and activates another window.
        /// </summary>
        Hide = 0,

        /// <summary>
        ///     Activates and displays a window. If the window is minimized or
        ///     maximized, the system restores it to its original size and position.
        ///     An application should specify this flag when displaying the window
        ///     for the first time.
        /// </summary>
        Normal = 1,

        /// <summary>
        ///     Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,

        /// <summary>
        ///     Maximizes the specified window.
        /// </summary>
        Maximize = 3,

        /// <summary>
        ///     Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,

        /// <summary>
        ///     Displays a window in its most recent size and position. This value
        ///     is similar to <see cref="ShowWindowCommands.Normal" />, except
        ///     the window is not activated.
        /// </summary>
        ShowNoActivate = 4,

        /// <summary>
        ///     Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,

        /// <summary>
        ///     Minimizes the specified window and activates the next top-level
        ///     window in the Z order.
        /// </summary>
        Minimize = 6,

        /// <summary>
        ///     Displays the window as a minimized window. This value is similar to
        ///     <see cref="ShowWindowCommands.ShowMinimized" />, except the
        ///     window is not activated.
        /// </summary>
        ShowMinNoActive = 7,

        /// <summary>
        ///     Displays the window in its current size and position. This value is
        ///     similar to <see cref="ShowWindowCommands.Show" />, except the
        ///     window is not activated.
        /// </summary>
        ShowNoActive = 8,

        /// <summary>
        ///     Activates and displays the window. If the window is minimized or
        ///     maximized, the system restores it to its original size and position.
        ///     An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,

        /// <summary>
        ///     Sets the show state based on the SW_* value specified in the
        ///     STARTUPINFO structure passed to the CreateProcess function by the
        ///     program that started the application.
        /// </summary>
        ShowDefault = 10,

        /// <summary>
        ///     <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        ///     that owns the window is not responding. This flag should only be
        ///     used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }
}
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
// ReSharper disable CommentTypo
namespace Exomia.Framework.Windows.Win32;

static class WS
{
    /// <summary> The window has a thin-line border. </summary>
    public const uint BORDER = 0x800000;

    /// <summary> The window has a title bar (includes the public const uint BORDER style). </summary>
    public const uint CAPTION = 0xc00000;

    /// <summary> The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the public const uint POPUP style. </summary>
    public const uint CHILD = 0x40000000;

    /// <summary> Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window. </summary>
    public const uint CLIPCHILDREN = 0x2000000;

    /// <summary>
    ///     Clips child windows relative to each other; that is; when a particular child window receives a WM_PAINT message;
    ///     the public const uint CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated. If public const uint CLIPSIBLINGS
    ///     is not specified and child windows overlap; it is possible; when drawing within the client area of a child window; to draw within the client area of a neighboring child
    ///     window.
    /// </summary>
    public const uint CLIPSIBLINGS = 0x4000000;

    /// <summary>
    ///     The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created; use the EnableWindow function.
    /// </summary>
    public const uint DISABLED = 0x8000000;

    /// <summary> The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar. </summary>
    public const uint DLGFRAME = 0x400000;

    /// <summary>
    ///     The window is the first control of a group of controls. The group consists of this first control and all controls defined after it; up to the next control with the
    ///     public const uint GROUP style. The first control in each group usually has the public const uint TABSTOP style so that the user can move from group to group. The user
    ///     can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys. You can turn this style on and off
    ///     to change dialog box navigation. To change this style after a window has been created; use the SetWindowLong function.
    /// </summary>
    public const uint GROUP = 0x20000;

    /// <summary> The window has a horizontal scroll bar. </summary>
    public const uint HSCROLL = 0x100000;

    /// <summary> The window is initially maximized. </summary>
    public const uint MAXIMIZE = 0x1000000;

    /// <summary>
    ///     The window has a maximize button. Cannot be combined with the public const uint EX_CONTEXTHELP style. The public const uint SYSMENU style must also be specified.
    /// </summary>
    public const uint MAXIMIZEBOX = 0x10000;

    /// <summary> The window is initially minimized. </summary>
    public const uint MINIMIZE = 0x20000000;

    /// <summary>
    ///     The window has a minimize button. Cannot be combined with the public const uint EX_CONTEXTHELP style. The public const uint SYSMENU style must also be specified.
    /// </summary>
    public const uint MINIMIZEBOX = 0x20000;

    /// <summary> The window is an overlapped window. An overlapped window has a title bar and a border. </summary>
    public const uint OVERLAPPED = 0x0;

    /// <summary> The window is an overlapped window. </summary>
    public const uint OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | SIZEFRAME | MINIMIZEBOX | MAXIMIZEBOX;

    /// <summary> The window is a pop-up window. This style cannot be used with the public const uint CHILD style. </summary>
    public const uint POPUP = 0x80000000u;

    /// <summary> The window is a pop-up window. The public const uint CAPTION and public const uint POPUPWINDOW styles must be combined to make the window menu visible. </summary>
    public const uint POPUPWINDOW = POPUP | BORDER | SYSMENU;

    /// <summary> The window has a sizing border. </summary>
    public const uint SIZEFRAME = 0x40000;

    /// <summary> The window has a window menu on its title bar. The public const uint CAPTION style must also be specified. </summary>
    public const uint SYSMENU = 0x80000;

    /// <summary>
    ///     The window is a control that can receive the keyboard focus when the user presses the TAB key. Pressing the TAB key changes the keyboard focus to the next control with
    ///     the public const uint TABSTOP style. You can turn this style on and off to change dialog box navigation. To change this style after a window has been created; use the
    ///     SetWindowLong function. For user-created windows and modeless dialogs to work with tab stops; alter the message loop to call the IsDialogMessage function.
    /// </summary>
    public const uint TABSTOP = 0x10000;

    /// <summary> The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function. </summary>
    public const uint VISIBLE = 0x10000000;

    /// <summary> The window has a vertical scroll bar. </summary>
    public const uint VSCROLL = 0x200000;
}
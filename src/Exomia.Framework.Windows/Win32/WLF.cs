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

internal static class WLF
{
    public const int GWL_EXSTYLE     = -20;
    public const int GWLP_HINSTANCE  = -6;
    public const int GWLP_HWNDPARENT = -8;
    public const int GWL_ID          = -12;
    public const int GWL_STYLE       = -16;
    public const int GWL_USERDATA    = -21;
    public const int GWL_WNDPROC     = -4;
    public const int DWLP_USER       = 0x8;
    public const int DWLP_MSGRESULT  = 0x0;
    public const int DWLP_DLGPROC    = 0x4;
}
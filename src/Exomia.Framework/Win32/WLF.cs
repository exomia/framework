#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo

namespace Exomia.Framework.Win32
{
    static class WLF
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
}
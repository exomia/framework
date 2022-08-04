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
namespace Exomia.Framework.Windows.Win32;

[StructLayout(LayoutKind.Sequential)]
struct RECT
{
    public POINT LeftTop;
    public POINT RightBottom;

    public RECT(int width, int height)
    {
        LeftTop.X     = LeftTop.Y = 0;
        RightBottom.X = width;
        RightBottom.Y = height;
    }
}
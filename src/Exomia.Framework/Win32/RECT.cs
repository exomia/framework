#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

namespace Exomia.Framework.Win32
{
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
}
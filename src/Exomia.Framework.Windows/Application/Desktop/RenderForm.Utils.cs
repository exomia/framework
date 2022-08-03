﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

namespace Exomia.Framework.Windows.Application.Desktop;

sealed partial class RenderForm
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int LowWord(IntPtr number)
    {
        return (int)number.ToInt64() & 0x0000FFFF;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int HighWord(IntPtr number)
    {
        return (int)number.ToInt64() >> 16;
    }
}
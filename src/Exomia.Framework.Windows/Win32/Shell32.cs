#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
namespace Exomia.Framework.Windows.Win32
{
    static class Shell32
    {
        private const string SHELL32 = "shell32.dll";

        [SuppressUnmanagedCodeSecurity]
        [DllImport(SHELL32, EntryPoint ="ExtractIcon", CharSet = CharSet.Auto, BestFitMapping = false)]
        internal static extern IntPtr ExtractIcon(IntPtr hInst, string exeFileName, int iconIndex);
    }
}
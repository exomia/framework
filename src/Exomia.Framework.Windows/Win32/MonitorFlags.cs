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

internal enum MonitorFlags : uint
{
    DEFAULTTONULL    = 0u,
    DEFAULTTOPRIMARY = 1u,
    DEFAULTTONEAREST = 2u
}
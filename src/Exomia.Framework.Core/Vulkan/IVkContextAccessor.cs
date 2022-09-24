#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> An interface to access the vk context. </summary>
public interface IVkContextAccessor
{
    /// <summary> Gets the vk context. </summary>
    /// <value> The vk context. </value>
    public unsafe VkContext* Context
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Swapchain
{
    /// <summary> Delegate for handling swapchain events. </summary>
    /// <param name="swapchain"> The swapchain. </param>
    public delegate void SwapchainEventHandler(Swapchain swapchain);

    /// <summary> Occurs when the swapchain was recreated. </summary>
    public event SwapchainEventHandler? SwapChainRecreated;

    /// <summary> Occurs when the swapchain is cleaned up. </summary>
    public event SwapchainEventHandler? CleanupSwapChain;
}
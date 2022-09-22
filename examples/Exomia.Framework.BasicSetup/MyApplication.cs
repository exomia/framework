#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Resources;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Vulkan.Api.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.BasicSetup;

sealed unsafe class MyApplication : Application
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<MyApplication> _logger;

    private readonly Swapchain _swapchain;
    private readonly Renderer  _renderer;
    
    /// <summary> Initializes a new instance of the <see cref="MyApplication" /> class. </summary>
    /// <param name="serviceProvider"> The service provider. </param>
    /// <param name="vkContextAccessor"> The vk context accessor. </param>
    /// <param name="swapchainConfiguration"> The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The Depth stencil configuration. </param>
    /// <param name="logger"> The logger. </param>
    public MyApplication(
        IServiceProvider                    serviceProvider,
        IVkContextAccessor                  vkContextAccessor,
        IOptions<SwapchainConfiguration>    swapchainConfiguration,
        IOptions<DepthStencilConfiguration> depthStencilConfiguration,
        ILogger<MyApplication>              logger)
        : base(serviceProvider)
    {
        _logger = logger;

        //IsFixedTimeStep   = true;
        //TargetElapsedTime = 1000.0 / 144;

        RenderPassConfiguration renderPassConfiguration = new();

        _swapchain = ToDispose(new Swapchain(
            vkContextAccessor.Context,
            swapchainConfiguration.Value,
            depthStencilConfiguration.Value,
            renderPassConfiguration));

        _renderer = ToDispose(new Renderer(_swapchain));
    }
    
    protected override bool BeginFrame()
    {
        return _swapchain.BeginFrame();
    }

    protected override void Render(Time time)
    {
        if (_renderer.Begin(out VkCommandBuffer commandBuffer))
        {
            _swapchain.BeginRenderPass(commandBuffer);

            _swapchain.EndRenderPass(commandBuffer);
            _renderer.End(commandBuffer);

            base.Render(time);
        }
    }

    protected override void EndFrame()
    {
        _swapchain.EndFrame();
    }

    protected override void OnDispose(bool disposing) { }
}
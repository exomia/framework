#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics;
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

using static Exomia.Vulkan.Api.Core.VkSubpassContents;

namespace Exomia.Framework.Example.Canvas;

sealed unsafe class MyApplication : Application
{
    private readonly Core.Vulkan.Vulkan     _vulkan;
    private readonly IContentManager        _contentManager;
    private readonly ILogger<MyApplication> _logger;

    private readonly Swapchain            _swapchain;
    private readonly Renderer             _renderer;
    private readonly Core.Graphics.Canvas _canvas;
    private          Texture              _texture1    = null!;
    private          SpriteFont           _spriteFont1 = null!;

    private int   _frames = 0;
    private float _timer  = 0;

    /// <summary> Initializes a new instance of the <see cref="MyApplication" /> class. </summary>
    /// <param name="serviceProvider"> The service provider. </param>
    /// <param name="vulkan"> The vulkan. </param>
    /// <param name="contentManager"> Manager for content. </param>
    /// <param name="swapchainConfiguration"> The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The Depth stencil configuration. </param>
    /// <param name="logger"> The logger. </param>
    public MyApplication(
        IServiceProvider                    serviceProvider,
        Core.Vulkan.Vulkan                  vulkan,
        IContentManager                     contentManager,
        IOptions<SwapchainConfiguration>    swapchainConfiguration,
        IOptions<DepthStencilConfiguration> depthStencilConfiguration,
        ILogger<MyApplication>              logger)
        : base(serviceProvider)
    {
        _vulkan         = vulkan;
        _contentManager = contentManager;
        _logger         = logger;

        //IsFixedTimeStep   = true;
        //TargetElapsedTime = 1000.0 / 144;

        RenderPassConfiguration renderPassConfiguration = new();

        _swapchain = ToDispose(new Swapchain(
            vulkan.Context,
            swapchainConfiguration.Value,
            depthStencilConfiguration.Value,
            renderPassConfiguration));

        _renderer = ToDispose(new Renderer(_swapchain));
        _canvas   = ToDispose(new Core.Graphics.Canvas(_swapchain));
    }

    /// <inheritdoc />
    protected override void OnLoadContent()
    {
        _texture1    = _contentManager.Load<Texture>("icon.e1");
        _spriteFont1 = _contentManager.Load<SpriteFont>(Fonts.ARIAL_12_PX, true);
    }

    /// <inheritdoc />
    protected override void OnUnloadContent()
    {
        // wait for the device idle before unloading content
        _vulkan.DeviceWaitIdle();
        
        _contentManager.Unload<Texture>("icon.e1");
        _contentManager.Unload<SpriteFont>(Fonts.ARIAL_12_PX);
    }

    protected override bool BeginFrame()
    {
        return _swapchain.BeginFrame();
    }

    protected override void Render(Time time)
    {
        if (_renderer.Begin(out VkCommandBuffer commandBuffer))
        {
             _swapchain.BeginRenderPass(commandBuffer, VkColors.CornflowerBlue, VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS );
            
            _canvas.Begin();
            
            _canvas.RenderArc(new Arc2(new Vector2(500, 500), 100), 40f, VkColors.Black, 0f, Vector2.Zero, 1f);
            
            Random2 rnd = new Random2(100);
            for (int i = 0; i < 1_000; i++)
            {
               _canvas.RenderArc(
                    new Arc2(new Vector2(rnd.Next(50, 900), rnd.Next(50, 700)), rnd.Next(60, 200) + 50 * MathF.Sin(time.TotalTimeS)), 
                    10f,
                    new VkColor(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle(), 1.0f),
                    0f, 
                    Vector2.Zero, 
                    1f);
            }
            
            _canvas.End(commandBuffer);
            
            _canvas.EndFrame();
            
            _swapchain.EndRenderPass(commandBuffer);
            
            _renderer.End(commandBuffer);
        }
        
        
        _timer += time.DeltaTimeS;
        if (_timer > 1.0f)
        {
            _timer -= 1.0f;
            Console.WriteLine(_frames);
            _frames = 0;
        }
        _frames++;
        
        base.Render(time);
    }

    protected override void EndFrame()
    {
        _swapchain.EndFrame();
    }

    protected override void OnDispose(bool disposing) { }
}
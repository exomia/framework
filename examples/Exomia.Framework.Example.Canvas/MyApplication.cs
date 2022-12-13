﻿#region License

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
            _swapchain.BeginRenderPass(commandBuffer, VkColors.CornflowerBlue, VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS);

            _canvas.Begin();

            Random2 rnd = new Random2(100);
            // for (int i = 0; i < 1_000; i++)
            // {
            //     _canvas.RenderArc(
            //          new Arc2(new Vector2(rnd.Next(150, 900), rnd.Next(150, 600)), rnd.Next(60, 200) + 50 * MathF.Sin(time.TotalTimeS), rnd.NextSingle() * MathF.PI * 2f, rnd.NextSingle() * MathF.PI * 2f), 
            //          20f,
            //          new VkColor(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle(), 1.0f),
            //          1.0f,
            //          0f, 
            //          Vector2.Zero, 
            //          1f);
            //
            //     _canvas.RenderFillArc(
            //         new Arc2(new Vector2(rnd.Next(150, 900), rnd.Next(150, 600)), rnd.Next(60, 200) + 50 * MathF.Sin(time.TotalTimeS), rnd.NextSingle() * MathF.PI * 2f, rnd.NextSingle() * MathF.PI * 2f),
            //         new VkColor(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle(), 1.0f),
            //         1.0f,
            //         0f, 
            //         Vector2.Zero,
            //         1f);
            // }

            _canvas.RenderArc(new Arc2(new Vector2(500, 500), 100 + MathF.Sin(time.TotalTimeS) * 50, time.TotalTimeS, MathF.PI / 2f + time.TotalTimeS), 40 + MathF.Sin(time.TotalTimeS) * 20,
                VkColors.Black, 1.0f, time.TotalTimeS, new Vector2(450, 450), 0f);

            _canvas.RenderFillArc(
                new Arc2(new Vector2(300, 300), 100 + MathF.Sin(time.TotalTimeS) * 50, time.TotalTimeS, MathF.PI / 2f + time.TotalTimeS),
                VkColors.Black, 1.0f, time.TotalTimeS, new Vector2(320, 320), 0f);

            _canvas.RenderLine(new Line2(new Vector2(100, 100), new Vector2(100, 200)), 10f, VkColors.Red,   1f, 0f, Vector2.Zero, 1.0f, 0f);
            _canvas.RenderLine(new Line2(new Vector2(100, 100), new Vector2(100, 200)), 10f, VkColors.Black, 1f, 0f, Vector2.Zero, 0.5f, 0f);

            _canvas.RenderLine(new Line2(new Vector2(300, 300), new Vector2(400, 400)), 10f, VkColors.Red,   1f, time.TotalTimeS, new Vector2(300, 300), 1.0f,                              0f);
            _canvas.RenderLine(new Line2(new Vector2(300, 300), new Vector2(400, 400)), 10f, VkColors.Black, 1f, time.TotalTimeS, new Vector2(300, 300), 1.0f + MathF.Sin(time.TotalTimeS), 0f);

            _canvas.RenderFillPolygon(
                new[] { new Vector2(500, 500), new Vector2(600, 550), new Vector2(600, 600), new Vector2(500, 580) }, VkColors.Red, 1.0f, 0f, Vector2.Zero, 0f);
            _canvas.RenderPolygon(
                new[] { new Vector2(500, 500), new Vector2(600, 550), new Vector2(600, 600), new Vector2(500, 580) }, 10, VkColors.Black, 1.0f, 0f, Vector2.Zero, 0f);

            _canvas.RenderFillPolygon(
                new[] { new Vector2(200, 500), new Vector2(300, 550), new Vector2(300, 600), new Vector2(200, 580) }, VkColors.Red, 1.0f, time.TotalTimeS, new Vector2(200, 500), 0f);
            _canvas.RenderPolygon(
                new[] { new Vector2(200, 500), new Vector2(300, 550), new Vector2(300, 600), new Vector2(200, 580) }, 10, VkColors.Black, 1.0f, time.TotalTimeS, new Vector2(200, 500), 0f);
            
            _canvas.RenderRectangle(new RectangleF(300, 100, 100, 100), VkColors.Red,  10.0f,                                    0f,              Vector2.Zero,          1f);
            _canvas.RenderRectangle(new RectangleF(400, 100, 100, 100), VkColors.Blue, 10f + 10.0f * MathF.Sin(time.TotalTimeS), time.TotalTimeS, new Vector2(400, 100), 1f);
            
            _canvas.RenderFillRectangle(new RectangleF(500, 100, 100, 100), VkColors.Red,  0f,              Vector2.Zero,          1f);
            _canvas.RenderFillRectangle(new RectangleF(600, 100, 100, 100), VkColors.Blue, time.TotalTimeS, new Vector2(600, 100), 1f);

            _canvas.RenderTriangle(new Triangle2(250, 50, 350, 150, 150, 150), VkColors.Red,  10f, 0f,              Vector2.Zero,          1f);
            _canvas.RenderTriangle(new Triangle2(250, 50, 350, 150, 150, 150), VkColors.Blue, 10f, time.TotalTimeS, new Vector2(250, 100), 1f);
            
            _canvas.RenderFillTriangle(new Triangle2(250 + 500, 50, 350 + 500, 150, 150 + 500, 150), VkColors.Red,  0f,              Vector2.Zero,                1f);
            _canvas.RenderFillTriangle(new Triangle2(250 + 500, 50, 350 + 500, 150, 150 + 500, 150), VkColors.Blue, time.TotalTimeS, new Vector2(250 + 500, 50), 1f);

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
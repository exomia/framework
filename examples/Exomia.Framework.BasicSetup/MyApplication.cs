﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Vulkan.Api.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.BasicSetup;

/// <summary>
///     My application class. This class cannot be inherited.
/// </summary>
internal sealed unsafe class MyApplication : Application
{
#pragma warning disable IDE0052 // Remove unread private members
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<MyApplication> _logger;
#pragma warning restore IDE0052 // Remove unread private members

    private readonly Swapchain   _swapchain;
    private readonly Renderer    _renderer;
    private readonly SpriteBatch _spriteBatch;

    /// <summary> Initializes a new instance of the <see cref="MyApplication" /> class. </summary>
    /// <param name="serviceProvider">           The service provider. </param>
    /// <param name="swapchainConfiguration">    The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The Depth stencil configuration. </param>
    /// <param name="logger">                    The logger. </param>
    public MyApplication(IServiceProvider                    serviceProvider,
                         IOptions<SwapchainConfiguration>    swapchainConfiguration,
                         IOptions<DepthStencilConfiguration> depthStencilConfiguration,
                         ILogger<MyApplication>              logger)
        : base(serviceProvider)
    {
        _logger = logger;

        //IsFixedTimeStep   = true;
        //TargetElapsedTime = 1000.0 / 144;

        Core.Vulkan.Vulkan vulkan = serviceProvider.GetRequiredService<Core.Vulkan.Vulkan>();

        RenderPassConfiguration renderPassConfiguration = new();

        _swapchain = new Swapchain(
            vulkan.Context,
            swapchainConfiguration.Value,
            depthStencilConfiguration.Value,
            renderPassConfiguration);

        _renderer    = new Renderer(_swapchain);
        _spriteBatch = new SpriteBatch(_swapchain);
    }

    int   frames = 0;
    float timer  = 0;

    protected override bool BeginFrame()
    {
        return _swapchain.BeginFrame();
    }

    protected override void Render(Time time)
    {
        Random rnd = new Random(100);

        if (_renderer.Begin(out VkCommandBuffer commandBuffer))
        {
            _renderer.BeginRenderPass(commandBuffer);

            const int iterations = 8_000;

            _spriteBatch.Begin(SpriteSortMode.Deferred);
            for (int i = 0; i < iterations; i++)
            {
                if (i > iterations * 0.75)
                {
                    _spriteBatch.DrawFillRectangle(
                        new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
                        new VkColor(0f, 0f, 0f, 1f),
                        rnd.NextSingle());
                }
                else if (i > iterations * 0.50)
                {
                    _spriteBatch.DrawFillRectangle(
                        new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
                        new VkColor(0f, 0f, 1f, 1f),
                        rnd.NextSingle());
                }
                else if (i > iterations * 0.25)
                {
                    _spriteBatch.DrawFillRectangle(
                        new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
                        new VkColor(0f, 1f, 0f, 1f),
                        rnd.NextSingle());
                }
                else
                {
                    _spriteBatch.DrawFillRectangle(
                        new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
                        new VkColor(1f, 0f, 0f, 1f),
                        rnd.NextSingle());
                }
            }
            _spriteBatch.End(commandBuffer);

            //_spriteBatch.Begin();
            //for (int i = iterations; i < iterations * 2; i++)
            //{
            //    if (i > iterations * 0.75)
            //    {
            //        _spriteBatch.DrawFillRectangle(
            //            new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
            //            new VkColor(0f, 0f, 0f, 1f),
            //            rnd.NextSingle());
            //    }
            //    else if (i > iterations * 0.50)
            //    {
            //        _spriteBatch.DrawFillRectangle(
            //            new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
            //            new VkColor(0f, 0f, 1f, 1f),
            //            rnd.NextSingle());
            //    }
            //    else if (i > iterations * 0.25)
            //    {
            //        _spriteBatch.DrawFillRectangle(
            //            new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
            //            new VkColor(0f, 1f, 0f, 1f),
            //            rnd.NextSingle());
            //    }
            //    else
            //    {
            //        _spriteBatch.DrawFillRectangle(
            //            new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
            //            new VkColor(1f, 0f, 0f, 1f),
            //            rnd.NextSingle());
            //    }
            //}
            //_spriteBatch.End(commandBuffer);

            _spriteBatch.EndFrame();

            _renderer.EndRenderPass(commandBuffer);
            _renderer.End(commandBuffer);
        }

        timer += time.DeltaTimeS;
        if (timer > 1.0f)
        {
            timer -= 1.0f;
            Console.WriteLine(frames);
            frames = 0;
        }
        frames++;
    }

    protected override void EndFrame()
    {
        _swapchain.EndFrame();
    }

    protected override void OnDispose(bool disposing)
    {
        _spriteBatch.Dispose();
        _renderer.Dispose();
        _swapchain.Dispose();
    }
}
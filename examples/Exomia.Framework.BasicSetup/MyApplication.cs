#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Graphics;
using Exomia.Framework.Core.Mathematics;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Vulkan.Api.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exomia.Framework.BasicSetup;

sealed unsafe class MyApplication : Application
{
#pragma warning disable IDE0052 // Remove unread private members
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger<MyApplication> _logger;
#pragma warning restore IDE0052 // Remove unread private members

    private readonly Swapchain   _swapchain;
    private readonly Renderer    _renderer;
    private readonly SpriteBatch _spriteBatch;
    private readonly Texture     _texture1;
    private readonly Texture     _texture2;

    /// <summary> Initializes a new instance of the <see cref="MyApplication" /> class. </summary>
    /// <param name="serviceProvider">           The service provider. </param>
    /// <param name="swapchainConfiguration">    The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The Depth stencil configuration. </param>
    /// <param name="logger">                    The logger. </param>
    /// <param name="contentManager">            Manager for content. </param>
    public MyApplication(
        IServiceProvider                    serviceProvider,
        IOptions<SwapchainConfiguration>    swapchainConfiguration,
        IOptions<DepthStencilConfiguration> depthStencilConfiguration,
        ILogger<MyApplication>              logger,
        IContentManager                     contentManager)
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

        _texture1 = contentManager.Load<Texture>("icon.e1");
        _texture2 = contentManager.Load<Texture>("skizze_eg.e1");
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
            const int iterations = 10_000;
            _spriteBatch.Begin(SpriteSortMode.Texture);
            for (int i = 0; i < iterations / 100; i++)
            {
                _spriteBatch.Draw(_texture1, new RectangleF(rnd.Next(0, 900), rnd.Next(0, 700), 40, 40), VkColors.White);

                _spriteBatch.Draw(_texture2, new RectangleF(rnd.Next(0, 900), rnd.Next(0, 700), 40, 40), VkColors.White);
            }
            _spriteBatch.End(commandBuffer);

            _spriteBatch.Begin();
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
        _texture1.Dispose();
        _texture2.Dispose();
    }
}
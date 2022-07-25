#region License

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
    private readonly SpriteBatch _spriteBatch;

    /// <summary> Initializes a new instance of the <see cref="MyApplication" /> class. </summary>
    /// <param name="serviceProvider">           The service provider. </param>
    /// <param name="swapchainConfiguration">    The swapchain configuration. </param>
    /// <param name="depthStencilConfiguration"> The depth stencil configuration. </param>
    /// <param name="logger">                    The logger. </param>
    public MyApplication(IServiceProvider                    serviceProvider,
                         IOptions<SwapchainConfiguration>    swapchainConfiguration,
                         IOptions<DepthStencilConfiguration> depthStencilConfiguration,
                         ILogger<MyApplication>              logger)
        : base(serviceProvider)
    {
        _logger = logger;

        //IsFixedTimeStep   = true;
        //TargetElapsedTime = 1000.0 / 1000;

        Core.Vulkan.Vulkan vulkan = serviceProvider.GetRequiredService<Core.Vulkan.Vulkan>();

        RenderPassConfiguration renderPassConfiguration = new();
        renderPassConfiguration.ColorAttachments[0].attachmentConfiguration.LoadOp     = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE;
        renderPassConfiguration.ColorAttachments[0].attachmentConfiguration.StoreOp    = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE;
        renderPassConfiguration.DepthStencilAttachment.attachmentConfiguration.LoadOp  = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE;
        renderPassConfiguration.DepthStencilAttachment.attachmentConfiguration.StoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE;

        _swapchain = new Swapchain(
            vulkan.Context,
            swapchainConfiguration.Value,
            depthStencilConfiguration.Value,
            renderPassConfiguration);

        //_renderer = new Renderer(vulkan.Context, _swapchain);
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
        //if (_renderer.Begin(out VkCommandBuffer* commandBuffers, out uint frameInFlight))
        //{
        //    _renderer.BeginRenderPass(*(commandBuffers + frameInFlight));
        //    _renderer.EndRenderPass(*(commandBuffers + frameInFlight));
        //    _renderer.End(commandBuffers, frameInFlight);
        //}
        
        _spriteBatch.Begin();

        Random rnd = new Random(100);

        //var       k          = Stopwatch.StartNew();
        const int iterations = 10_000;
        for (int i = 0; i < iterations; i++)
        {
            if (i > iterations * 0.75)
            {
                _spriteBatch.DrawFillRectangle(
                    new RectangleF(50 + rnd.Next(0, 900) + MathF.Sin(time.TotalTimeS) * 50, 50 + rnd.Next(0, 600), 4, 4),
                    new VkColor(0f, 1f, 1f, 1f),
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
        //Console.WriteLine(k.Elapsed.TotalMilliseconds);
        _spriteBatch.End();

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
        //_renderer.Dispose();
        _spriteBatch.Dispose();
        _swapchain.Dispose();
    }
}
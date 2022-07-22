#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Windows.Game.Desktop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;

namespace Exomia.Framework.BasicSetup;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                     .MinimumLevel.Debug()
                     .WriteTo.Console()
                     .CreateLogger();

        using (IGameBuilder gameBuilder = GameBuilder.Create())
        using (Game game = gameBuilder
                           .ConfigureServices(serviceCollection =>
                           {
                               serviceCollection.AddLogging(builder =>
                               {
                                   builder.ClearProviders();
                                   builder.AddSerilog(Log.Logger);
                               });
                           })
                           .Configure<DebugUtilsMessengerConfiguration>((configuration, _) =>
                           {
                               configuration.MessageSeverity |= VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT;
                           })
                           .Configure<RenderFormConfiguration>((configuration, _) =>
                           {
                               configuration.Title       = "Exomia.Framework.BasicSetup";
                               configuration.Width       = 1024;
                               configuration.Height      = 768;
                               configuration.DisplayType = DisplayType.Window;
                           })
                           .UseWin32Platform() // should always be the last in the chain before calling build!
                           .Build<MyGame>())
        {
            game.Run();
        }
    }
}
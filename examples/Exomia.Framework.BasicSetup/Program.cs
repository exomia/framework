#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Extensions;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Windows.Game.Desktop;

namespace Exomia.Framework.BasicSetup;

internal sealed class Program
{
    private static void Main(string[] args)
    {
        using (IGameBuilder gameBuilder = GameBuilder.Create())
        using (MyGame game = gameBuilder
                   //.UseLogging()
                   .Configure<DebugUtilsMessengerConfiguration>((_, configuration) =>
                   {
                       //configuration.MessageSeverity |= VkDebugUtilsMessageSeverityFlagsEXT.VERBOSE_BIT_EXT;
                   })
                   .Configure<RenderFormConfiguration>(((_, configuration) =>
                   {
                       configuration.Title       = "Exomia.Framework.BasicSetup";
                       configuration.Width       = 1024;
                       configuration.Height      = 768;
                       configuration.DisplayType = DisplayType.Window;
                   }))
                   .UseWin32Platform() // should always be the last in the chain before calling build!
                   .Build<MyGame>())

            //TODO: add use startup possibility
        {
            game.Run();
        }
    }
}
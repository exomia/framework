#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Application.Configurations;
using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Extensions;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Windows.Application.Desktop;
using Exomia.Vulkan.Api.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;

namespace Exomia.Framework.BasicSetup;

static class Program
{
    // ReSharper disable once UnusedParameter.Local
    private static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .CreateLogger();

        using (IApplicationBuilder applicationBuilder = ApplicationBuilder.Create())
        using (Application application
               = applicationBuilder
                .ConfigureServices(serviceCollection =>
                 {
                     serviceCollection
                        .AddLogging(builder =>
                         {
                             builder.ClearProviders();
                             builder.AddSerilog(Log.Logger);
                         })
                        .AddDefaultContentManagement(new ContentManager.Configuration
                         {
                             RootDirectory = "Content"
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
                .Configure<InstanceConfiguration>((configuration, _) =>
                 {
#if DEBUG
                     configuration.ValidationFeatureEnable.Add(
                         VkValidationFeatureEnableEXT.VK_VALIDATION_FEATURE_ENABLE_DEBUG_PRINTF_EXT);
#endif
                 })
                .UseWin32Platform() // should always be the last in the chain before calling build!
                .Build<MyApplication>())
        {
            application.Run();
        }
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Application;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.Framework.Windows.Input;
using Exomia.Framework.Windows.Win32;
using Exomia.Vulkan.Api.Core;
using Exomia.Vulkan.Api.Win32;
using Microsoft.Extensions.DependencyInjection;
using static Exomia.Vulkan.Api.Core.VkKhrGetSurfaceCapabilities2;
using static Exomia.Vulkan.Api.Win32.VkKhrWin32Surface;
using static Exomia.Vulkan.Api.Win32.VkExtFullScreenExclusive;
using static Exomia.Vulkan.Api.Win32.VkFullScreenExclusiveEXT;
using ApplicationConfiguration = Exomia.Framework.Core.Application.Configurations.ApplicationConfiguration;

namespace Exomia.Framework.Windows.Application.Desktop;

/// <summary> The platform. </summary>
public static class Platform
{
    private const int PM_REMOVE = 0x0001;

    /// <summary> An <see cref="IApplicationBuilder" /> extension method to use the win32 platform. </summary>
    /// <param name="builder"> The builder to act on. </param>
    /// <returns> An <see cref="IApplicationBuilder" />. </returns>
    public static unsafe IApplicationBuilder UseWin32Platform(this IApplicationBuilder builder)
    {
        return builder.ConfigureServices(serviceCollection =>
                       {
                           serviceCollection
                              .AddSingleton<RenderForm>()
                              .AddSingleton((Func<IServiceProvider, IRenderForm>)(p => p.GetRequiredService<RenderForm>()))
                              .AddSingleton((Func<IServiceProvider, IWin32RenderForm>)(p => p.GetRequiredService<RenderForm>()))
                              .AddSingleton((Func<IServiceProvider, IInputDevice>)(p => p.GetRequiredService<RenderForm>()))
                              .AddSingleton((Func<IServiceProvider, IWindowsInputDevice>)(p => p.GetRequiredService<RenderForm>()));
                       })
                      .Configure<InstanceConfiguration>((configuration, _) =>
                       {
                           configuration.EnabledExtensionNames.Add(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
                       })
                      .Configure<DeviceConfiguration>((configuration, _) =>
                       {
                           configuration.EnabledExtensionNames.Add(VK_EXT_FULL_SCREEN_EXCLUSIVE_EXTENSION_NAME);
                       })
                      .Configure<ApplicationConfiguration>((configuration, _) =>
                       {
                           configuration.DoEvents = &DoEvents;
                       })
                      .Configure<SurfaceConfiguration>((configuration, serviceProvider) =>
                       {
                           RenderForm renderForm = builder.RegisterDisposable(serviceProvider.GetRequiredService<RenderForm>());

                           configuration.CreateSurface = context =>
                           {
                               VkKhrWin32Surface.Load(context->Instance);

                               (IntPtr hInstance, IntPtr hWnd) = renderForm.CreateWindow();

                               context->InitialWidth  = (uint)renderForm.Width;
                               context->InitialHeight = (uint)renderForm.Height;

                               VkWin32SurfaceCreateInfoKHR win32SurfaceCreateInfoKhr;
                               win32SurfaceCreateInfoKhr.sType     = VkWin32SurfaceCreateInfoKHR.STYPE;
                               win32SurfaceCreateInfoKhr.pNext     = null;
                               win32SurfaceCreateInfoKhr.flags     = 0;
                               win32SurfaceCreateInfoKhr.hinstance = hInstance;
                               win32SurfaceCreateInfoKhr.hwnd      = hWnd;

                               vkCreateWin32SurfaceKHR(context->Instance, &win32SurfaceCreateInfoKhr, null, &context->SurfaceKhr)
                                  .AssertVkResult();

                               return true;
                           };
                       }).Configure<SwapchainConfiguration>((configuration, serviceProvider) =>
                       {
                           RenderForm renderForm = serviceProvider.GetRequiredService<RenderForm>();

                           //renderForm.Resized += form =>
                           //{
                           //    configuration.Resize?.Invoke(form.Width, form.Height);
                           //};

                           configuration.BeforeSwapchainCreation = context =>
                           {
                               if (context->PhysicalDevice == VkPhysicalDevice.Null) { return false; }

                               VkSurfaceCapabilitiesFullScreenExclusiveEXT surfaceCapabilitiesFullScreenExclusiveExt;
                               surfaceCapabilitiesFullScreenExclusiveExt.sType = VkSurfaceCapabilitiesFullScreenExclusiveEXT.STYPE;
                               surfaceCapabilitiesFullScreenExclusiveExt.pNext = null;

                               VkSurfaceCapabilities2KHR surfaceCapabilities2Khr;
                               surfaceCapabilities2Khr.sType = VkSurfaceCapabilities2KHR.STYPE;
                               surfaceCapabilities2Khr.pNext = &surfaceCapabilitiesFullScreenExclusiveExt;

                               VkSurfaceFullScreenExclusiveWin32InfoEXT* pSurfaceFullScreenExclusiveWin32InfoExt = Allocator.Allocate<VkSurfaceFullScreenExclusiveWin32InfoEXT>(1u);
                               pSurfaceFullScreenExclusiveWin32InfoExt->sType    = VkSurfaceFullScreenExclusiveWin32InfoEXT.STYPE;
                               pSurfaceFullScreenExclusiveWin32InfoExt->pNext    = null;
                               pSurfaceFullScreenExclusiveWin32InfoExt->hmonitor = User32.MonitorFromWindow(renderForm.HWnd, MonitorFlags.DEFAULTTONEAREST);

                               VkSurfaceFullScreenExclusiveInfoEXT* pSurfaceFullScreenExclusiveInfoExt = Allocator.Allocate<VkSurfaceFullScreenExclusiveInfoEXT>(1u);
                               pSurfaceFullScreenExclusiveInfoExt->sType               = VkSurfaceFullScreenExclusiveInfoEXT.STYPE;
                               pSurfaceFullScreenExclusiveInfoExt->pNext               = pSurfaceFullScreenExclusiveWin32InfoExt;
                               pSurfaceFullScreenExclusiveInfoExt->fullScreenExclusive = VK_FULL_SCREEN_EXCLUSIVE_APPLICATION_CONTROLLED_EXT;

                               VkPhysicalDeviceSurfaceInfo2KHR physicalDeviceSurfaceInfo2Khr;
                               physicalDeviceSurfaceInfo2Khr.sType   = VkPhysicalDeviceSurfaceInfo2KHR.STYPE;
                               physicalDeviceSurfaceInfo2Khr.pNext   = pSurfaceFullScreenExclusiveInfoExt;
                               physicalDeviceSurfaceInfo2Khr.surface = context->SurfaceKhr;

                               vkGetPhysicalDeviceSurfaceCapabilities2KHR(context->PhysicalDevice, &physicalDeviceSurfaceInfo2Khr, &surfaceCapabilities2Khr)
                                  .AssertVkResult();

                               if (!surfaceCapabilitiesFullScreenExclusiveExt.fullScreenExclusiveSupported)
                               {
                                   Allocator.Free(pSurfaceFullScreenExclusiveInfoExt,      1u);
                                   Allocator.Free(pSurfaceFullScreenExclusiveWin32InfoExt, 1u);
                                   throw new VulkanException("The system doesn't support required surface capabilities (full screen exclusive)!");
                               }

                               configuration.Next = pSurfaceFullScreenExclusiveInfoExt;

                               configuration.AfterSwapchainCreation = ctx =>
                               {
                                   Allocator.Free(pSurfaceFullScreenExclusiveInfoExt,      1u);
                                   Allocator.Free(pSurfaceFullScreenExclusiveWin32InfoExt, 1u);
                                   return true;
                               };

                               return true;
                           };
                       });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DoEvents()
    {
        while (User32.PeekMessage(out MSG m, IntPtr.Zero, 0, 0, PM_REMOVE))
        {
#pragma warning disable CA1806 // Do not ignore method results
            User32.TranslateMessage(ref m);
            User32.DispatchMessage(ref m);
#pragma warning restore CA1806 // Do not ignore method results
        }
    }
}
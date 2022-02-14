#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;
using Exomia.Framework.Core.Vulkan;
using Exomia.Framework.Core.Vulkan.Configurations;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.Framework.Windows.Input;
using Exomia.Framework.Windows.Win32;
using Exomia.IoC;
using Exomia.Vulkan.Api.Core;
using Exomia.Vulkan.Api.Win32;
using static Exomia.Vulkan.Api.Core.VkKhrGetSurfaceCapabilities2;
using static Exomia.Vulkan.Api.Win32.VkKhrWin32Surface;
using static Exomia.Vulkan.Api.Win32.VkExtFullScreenExclusive;
using static Exomia.Vulkan.Api.Win32.VkFullScreenExclusiveEXT;

namespace Exomia.Framework.Windows.Game.Desktop
{
    /// <summary> The game platform. </summary>
    public static class GamePlatform
    {
        private const int PM_REMOVE = 0x0001;

        /// <summary> An <see cref="IGameBuilder" /> extension method to use the win32 platform. </summary>
        /// <param name="builder"> The builder to act on. </param>
        /// <returns> An <see cref="IGameBuilder" />. </returns>
        public static unsafe IGameBuilder UseWin32Platform(this IGameBuilder builder)
        {
            return builder.ConfigureServices(services =>
                {
                    services
                        .Add<RenderForm>(ServiceKind.Singleton)
                        .Add<IRenderForm>(p => p.Get<RenderForm>(),         ServiceKind.Singleton)
                        .Add<IWin32RenderForm>(p => p.Get<RenderForm>(),    ServiceKind.Singleton)
                        .Add<IInputDevice>(p => p.Get<RenderForm>(),        ServiceKind.Singleton)
                        .Add<IWindowsInputDevice>(p => p.Get<RenderForm>(), ServiceKind.Singleton);
                })
                .Configure<InstanceConfiguration>((_, configuration) =>
                {
                    configuration.EnabledExtensionNames.Add(VK_KHR_WIN32_SURFACE_EXTENSION_NAME);
                })
                .Configure<DeviceConfiguration>((_, configuration) =>
                {
                    configuration.EnabledExtensionNames.Add(VK_EXT_FULL_SCREEN_EXCLUSIVE_EXTENSION_NAME);
                })
                .Configure<SurfaceConfiguration>((serviceProvider, configuration) =>
                {
                    RenderForm renderForm = builder.RegisterDisposable(serviceProvider.Get<RenderForm>());

                    configuration.CreateSurface = context =>
                    {
                        VkKhrWin32Surface.Load(context->Instance);

                        (IntPtr hInstance, IntPtr hWnd) = renderForm.CreateWindow();

                        context->Width  = (uint)renderForm.Width;
                        context->Height = (uint)renderForm.Height;

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
                })
                .Configure<SwapchainConfiguration>((serviceProvider, configuration) =>
                {
                    RenderForm renderForm = serviceProvider.Get<RenderForm>();

                    configuration.BeginSwapchainCreation = context =>
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

                        configuration.SwapchainCreationSuccessful = ctx =>
                        {
                            Allocator.Free(pSurfaceFullScreenExclusiveInfoExt,      1u);
                            Allocator.Free(pSurfaceFullScreenExclusiveWin32InfoExt, 1u);
                            return true;
                        };

                        return true;
                    };
                })
                .Configure<GameConfiguration>((_, configuration) =>
                {
                    configuration.DoEvents = &DoEvents;
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
}
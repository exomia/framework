﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using Exomia.Framework.Core.Allocators;
using Exomia.Framework.Core.Vulkan.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static Exomia.Vulkan.Api.Core.VkImageLayout;
using static Exomia.Vulkan.Api.Core.VkAccessFlagBits;
using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;

namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Vulkan : IDisposable
{
    private readonly ILogger<Vulkan>                  _logger;
    private readonly ApplicationConfiguration         _applicationConfiguration;
    private readonly InstanceConfiguration            _instanceConfiguration;
    private readonly DebugUtilsMessengerConfiguration _debugUtilsMessengerConfiguration;
    private readonly SurfaceConfiguration             _surfaceConfiguration;
    private readonly PhysicalDeviceConfiguration      _physicalDeviceConfiguration;
    private readonly DeviceConfiguration              _deviceConfiguration;
    private readonly QueueConfiguration               _queueConfiguration;

    private VkContext* _context;

    /// <summary> Gets the context. </summary>
    /// <value> The context. </value>
    public VkContext* Context
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get { return _context; }
    }

    /// <summary> Initializes a new instance of the <see cref="Vulkan" /> class. </summary>
    /// <param name="applicationConfiguration">         The application configuration. </param>
    /// <param name="debugUtilsMessengerConfiguration"> The debug utilities messenger configuration. </param>
    /// <param name="deviceConfiguration">              The device configuration. </param>
    /// <param name="instanceConfiguration">            The instance configuration. </param>
    /// <param name="physicalDeviceConfiguration">      The physical device configuration. </param>
    /// <param name="queueConfiguration">               The queue configuration. </param>
    /// <param name="surfaceConfiguration">             The surface configuration. </param>
    /// <param name="logger">                           The logger. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    public Vulkan(IOptions<ApplicationConfiguration>         applicationConfiguration,
                  IOptions<DebugUtilsMessengerConfiguration> debugUtilsMessengerConfiguration,
                  IOptions<DeviceConfiguration>              deviceConfiguration,
                  IOptions<InstanceConfiguration>            instanceConfiguration,
                  IOptions<PhysicalDeviceConfiguration>      physicalDeviceConfiguration,
                  IOptions<QueueConfiguration>               queueConfiguration,
                  IOptions<SurfaceConfiguration>             surfaceConfiguration,
                  ILogger<Vulkan>                            logger)
    {
        _applicationConfiguration         = applicationConfiguration.Value         ?? throw new ArgumentNullException(nameof(applicationConfiguration));
        _debugUtilsMessengerConfiguration = debugUtilsMessengerConfiguration.Value ?? throw new ArgumentNullException(nameof(debugUtilsMessengerConfiguration));
        _deviceConfiguration              = deviceConfiguration.Value              ?? throw new ArgumentNullException(nameof(deviceConfiguration));
        _instanceConfiguration            = instanceConfiguration.Value            ?? throw new ArgumentNullException(nameof(instanceConfiguration));
        _physicalDeviceConfiguration      = physicalDeviceConfiguration.Value      ?? throw new ArgumentNullException(nameof(physicalDeviceConfiguration));
        _queueConfiguration               = queueConfiguration.Value               ?? throw new ArgumentNullException(nameof(queueConfiguration));
        _surfaceConfiguration             = surfaceConfiguration.Value             ?? throw new ArgumentNullException(nameof(surfaceConfiguration));
        _logger                           = logger                                 ?? throw new ArgumentNullException(nameof(logger));

        *(_context = Allocator.Allocate<VkContext>(1u)) = VkContext.Create();
    }

    internal bool Initialize()
    {
        using (_logger.BeginScope("[{method}] started...", nameof(Initialize)))
        {
            if (!InitializeInstance(
                    _applicationConfiguration,
                    _instanceConfiguration,
                    _debugUtilsMessengerConfiguration,
                    _surfaceConfiguration,
                    _physicalDeviceConfiguration,
                    _deviceConfiguration))
            {
                _logger.LogCritical("{method} failed!", nameof(Initialize));
                return false;
            }

            CreateDevice(Context, _deviceConfiguration, _queueConfiguration);

            _logger.LogInformation("[{method}] done!", nameof(Initialize));
            return true;
        }
    }

    internal void Cleanup()
    {
        using (_logger.BeginScope("[{method}] started...", nameof(Cleanup)))
        {
            DestroyDevice(Context);
            CleanupInstance(Context);

            _logger.LogInformation("[{method}] done!", nameof(Cleanup));
        }
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            if (_context != null)
            {
                Cleanup();

                Allocator.Free(ref _context, 1u);
            }
        }
        GC.SuppressFinalize(this);
    }

    /// <summary> Finalizes an instance of the <see cref="Vulkan" /> class. </summary>
    ~Vulkan()
    {
        Dispose();
    }

    #endregion
}
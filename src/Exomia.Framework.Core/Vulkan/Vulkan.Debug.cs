#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using Exomia.Framework.Core.Vulkan.Configurations;
using Microsoft.Extensions.Logging;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageTypeFlagBitsEXT;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    /// <summary> Handler, called within the debug user callback to log to the logger of the vulkan instance. </summary>
    /// <param name="logLevel">      The log level. </param>
    /// <param name="exception">     The exception. </param>
    /// <param name="messageFormat"> The message format. </param>
    /// <param name="args">          A variable-length parameters list containing arguments. </param>
    public delegate void LogHandler(LogLevel logLevel, Exception? exception, string messageFormat, params object[] args);

    private static VkBool32 UserCallbackDefault(
        VkDebugUtilsMessageSeverityFlagBitsEXT severityFlags,
        VkDebugUtilsMessageTypeFlagBitsEXT     messageType,
        VkDebugUtilsMessengerCallbackDataEXT*  pCallbackData,
        void*                                  pUserData)
    {
        Marshal.GetDelegateForFunctionPointer<LogHandler>(new IntPtr(pUserData))(
            severityFlags switch
            {
                VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT => LogLevel.Trace,
                VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT    => LogLevel.Information,
                VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT => LogLevel.Warning,
                VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT   => LogLevel.Error,
                _                                               => throw new ArgumentOutOfRangeException(nameof(severityFlags), severityFlags, null)
            },
            null,
            "[{id}/{kind}] {message}",
            VkHelper.ToString(pCallbackData->pMessageIdName),
            messageType switch
            {
                VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT     => "GENERAL",
                VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT  => "VALIDATION",
                VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT => "PERFORMANCE",
                _                                               => throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null)
            },
            VkHelper.ToString(pCallbackData->pMessage));

        return VkBool32.False;
    }

    private static void LogCallback(ILogger logger, LogLevel logLevel, Exception? exception, string? message, params object?[] args)
    {
#pragma warning disable CA2254 // Template should be a static expression
        logger.Log(logLevel, exception, message, args);
#pragma warning restore CA2254 // Template should be a static expression
    }

    private void SetupDebugCallback(
        DebugUtilsMessengerConfiguration debugUtilsMessengerConfiguration)
    {
        VkExtDebugUtils.Load(_context->Instance);

        VkDebugUtilsMessengerCreateInfoEXT debugUtilsMessengerCreateInfoExt;
        debugUtilsMessengerCreateInfoExt.sType           = VkDebugUtilsMessengerCreateInfoEXT.STYPE;
        debugUtilsMessengerCreateInfoExt.pNext           = null;
        debugUtilsMessengerCreateInfoExt.flags           = 0u;
        debugUtilsMessengerCreateInfoExt.messageSeverity = debugUtilsMessengerConfiguration.MessageSeverity;
        debugUtilsMessengerCreateInfoExt.messageType     = debugUtilsMessengerConfiguration.MessageType;
        debugUtilsMessengerCreateInfoExt.pfnUserCallback = debugUtilsMessengerConfiguration.UserCallback != null
            ? debugUtilsMessengerConfiguration.UserCallback
            : &UserCallbackDefault;

        Expression[] parameters =
        {
            Expression.Constant(_logger),
            Expression.Parameter(typeof(LogLevel),  "logLevel"),
            Expression.Parameter(typeof(Exception), "exception"),
            Expression.Parameter(typeof(string),    "message"),
            Expression.Parameter(typeof(object[]),  "args")
        };

        debugUtilsMessengerCreateInfoExt.pUserData = debugUtilsMessengerConfiguration.PUserData != null
            ? debugUtilsMessengerConfiguration.PUserData
            : (void*)Marshal.GetFunctionPointerForDelegate(
                debugUtilsMessengerConfiguration.LogHandler =
                    Expression.Lambda<LogHandler>(
                                  Expression.Call(
                                      null,
                                      typeof(Vulkan).GetMethod(
                                          nameof(LogCallback),
                                          BindingFlags.NonPublic | BindingFlags.Static,
                                          null,
                                          CallingConventions.Standard,
                                          parameters.Select(p => p.Type).ToArray(),
                                          null) ?? throw new NullReferenceException(),
                                      parameters.Cast<Expression>()),
                                  parameters.OfType<ParameterExpression>())
                              .Compile());

        vkCreateDebugUtilsMessengerEXT(_context->Instance, &debugUtilsMessengerCreateInfoExt, null, &_context->DebugUtilsMessengerExt)
            .AssertVkResult();
    }
}
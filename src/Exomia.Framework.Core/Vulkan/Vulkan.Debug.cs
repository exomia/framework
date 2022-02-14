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
using Exomia.Logging;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageTypeFlagBitsEXT;
using static Exomia.Vulkan.Api.Core.VkDebugUtilsMessageSeverityFlagBitsEXT;

namespace Exomia.Framework.Core.Vulkan
{
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
            LogLevel logLevel;
            if ((severityFlags & VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT)
            {
                logLevel = LogLevel.Trace;
            }
            else if ((severityFlags & VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT)
            {
                logLevel = LogLevel.Information;
            }
            else if ((severityFlags & VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT)
            {
                logLevel = LogLevel.Warning;
            }
            else if ((severityFlags & VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT)
            {
                logLevel = LogLevel.Error;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(severityFlags), severityFlags, null);
            }

            LogHandler logHandler = Marshal.GetDelegateForFunctionPointer<LogHandler>(new IntPtr(pUserData));

            if ((messageType & VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT)
            {
                logHandler(logLevel, null, "[{0}/GENERAL] {1}", VkHelper.ToString(pCallbackData->pMessageIdName), VkHelper.ToString(pCallbackData->pMessage));
            }
            else if ((messageType & VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT)
            {
                logHandler(logLevel, null, "[{0}/VALIDATION] {1}", VkHelper.ToString(pCallbackData->pMessageIdName), VkHelper.ToString(pCallbackData->pMessage));
            }
            else if ((messageType & VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT) == VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT)
            {
                logHandler(logLevel, null, "[{0}/PERFORMANCE] {1}", VkHelper.ToString(pCallbackData->pMessageIdName), VkHelper.ToString(pCallbackData->pMessage));
            }

            return VkBool32.False;
        }

        private bool SetupDebugCallback(VkContext* context, DebugUtilsMessengerConfiguration debugUtilsMessengerConfiguration)
        {
            VkDebugUtilsMessengerCreateInfoEXT debugUtilsMessengerCreateInfoExt;
            debugUtilsMessengerCreateInfoExt.sType           = VkDebugUtilsMessengerCreateInfoEXT.STYPE;
            debugUtilsMessengerCreateInfoExt.pNext           = null;
            debugUtilsMessengerCreateInfoExt.flags           = 0u;
            debugUtilsMessengerCreateInfoExt.messageSeverity = debugUtilsMessengerConfiguration.MessageSeverity;
            debugUtilsMessengerCreateInfoExt.messageType     = debugUtilsMessengerConfiguration.MessageType;
            debugUtilsMessengerCreateInfoExt.pfnUserCallback = debugUtilsMessengerConfiguration.UserCallback != null
                ? debugUtilsMessengerConfiguration.UserCallback
                : &UserCallbackDefault;

            ParameterExpression[] parameters =
            {
                Expression.Parameter(typeof(LogLevel),  "logLevel"),
                Expression.Parameter(typeof(Exception), "exception"),
                Expression.Parameter(typeof(string),    "messageFormat"),
                Expression.Parameter(typeof(object[]),  "args")
            };

            VkExtDebugUtils.Load(context->Instance);

            debugUtilsMessengerCreateInfoExt.pUserData = (void*)Marshal.GetFunctionPointerForDelegate(
                _logHandler = Expression.Lambda<LogHandler>(
                        Expression.Call(
                            Expression.Constant(_logger),
                            typeof(ILogger).GetMethod(
                                nameof(ILogger.Log),
                                BindingFlags.Public | BindingFlags.Instance,
                                null,
                                CallingConventions.HasThis,
                                new[] { typeof(LogLevel), typeof(Exception), typeof(string), typeof(object).MakeArrayType() },
                                null) ?? throw new NullReferenceException(),
                            parameters.Cast<Expression>()),
                        parameters)
                    .Compile());

            vkCreateDebugUtilsMessengerEXT(context->Instance, &debugUtilsMessengerCreateInfoExt, null, &context->DebugUtilsMessengerExt)
                .AssertVkResult();

            return true;
        }
    }
}
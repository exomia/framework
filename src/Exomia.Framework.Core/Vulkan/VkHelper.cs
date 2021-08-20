﻿#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using System.Text;
using Exomia.Framework.Core.Vulkan.Exceptions;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Core.Vulkan
{
    internal static unsafe class VkHelper
    {
        public static void AssertVkResult(this               VkResult result,
                                          [CallerMemberName] string   callingMethod         = "",
                                          [CallerFilePath]   string   callingFilePath       = "",
                                          [CallerLineNumber] int      callingFileLineNumber = 0)
        {
            if (result != VkResult.SUCCESS) { throw new VulkanException(result, callingMethod, callingFilePath, callingFileLineNumber); }
        }

        public static void* GetInstanceProcAddr(this in VkInstance instance, string name)
        {
            int    maxByteCount;
            sbyte* pName = stackalloc sbyte[maxByteCount = Encoding.UTF8.GetMaxByteCount(name.Length) + 1];
            name.ToNtStringUtf8(pName, maxByteCount);
            return Vk.GetInstanceProcAddr(instance, pName);
        }

        public static void* GetDeviceProcAddr(this in VkDevice device, string name)
        {
            int    maxByteCount;
            sbyte* pName = stackalloc sbyte[maxByteCount = Encoding.UTF8.GetMaxByteCount(name.Length) + 1];
            name.ToNtStringUtf8(pName, maxByteCount);
            return Vk.GetDeviceProcAddr(device, pName);
        }

        public static void ToNtStringUtf8(this string? value, sbyte* dstPointer, int maxByteCount)
        {
            if (value == null) { return; }
            fixed (char* srcPointer = value)
            {
                dstPointer[Encoding.UTF8.GetBytes(srcPointer, value.Length, (byte*)dstPointer, maxByteCount)] = 0;
            }
        }

        public static string ToString(sbyte* dstPointer)
        {
            int i = -1;
            while (*(bool*)(dstPointer + (++i))) { }
            return Encoding.UTF8.GetString((byte*)dstPointer, i);
        }
    }
}
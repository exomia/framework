#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.CompilerServices;
using System.Text;
using Exomia.Framework.Core.Vulkan.Exceptions;

namespace Exomia.Framework.Core.Vulkan;

internal static unsafe class VkHelper
{
    public static void AssertVkResult(this               VkResult result,
                                      [CallerMemberName] string   callingMethod         = "",
                                      [CallerFilePath]   string   callingFilePath       = "",
                                      [CallerLineNumber] int      callingFileLineNumber = 0)
    {
        if (result != VK_SUCCESS) { throw new VulkanException(result, callingMethod, callingFilePath, callingFileLineNumber); }
    }

    public static void* GetInstanceProcAddr(this in VkInstance instance, string name)
    {
        int   maxByteCount;
        byte* pName = stackalloc byte[maxByteCount = Encoding.UTF8.GetMaxByteCount(name.Length) + 1];
        name.ToNtStringUtf8(pName, maxByteCount);
        return vkGetInstanceProcAddr(instance, pName);
    }

    public static void* GetDeviceProcAddr(this in VkDevice device, string name)
    {
        int   maxByteCount;
        byte* pName = stackalloc byte[maxByteCount = Encoding.UTF8.GetMaxByteCount(name.Length) + 1];
        name.ToNtStringUtf8(pName, maxByteCount);
        return vkGetDeviceProcAddr(device, pName);
    }

    public static string ToNtStringUtf8(this string value)
    {
        StringBuilder valueAsUtf8InUtf16 = new StringBuilder((value.Length * 2) + 1);

        for (int i = 0, n = value.Length - 1; i < n; i += 2)
        {
            byte low  = (byte)value[i];     // is okay, because only ascii is supported
            byte high = (byte)value[i + 1]; // is okay, because only ascii is supported
            valueAsUtf8InUtf16.Append($"\\u{high:x2}{low:x2}");
        }

        return valueAsUtf8InUtf16
               .Append(
                   value.Length % 2 == 0
                       ? "\\u0000"
                       : $"\\u00{(byte)value[value.Length - 1]:x2}")
               .ToString();
    }

    public static void ToNtStringUtf8(this string? value, byte* dstPointer, int maxByteCount)
    {
        if (value == null) { return; }
        fixed (char* srcPointer = value)
        {
            dstPointer[Encoding.UTF8.GetBytes(srcPointer, value.Length, dstPointer, maxByteCount)] = 0;
        }
    }

    public static string ToString(byte* dstPointer)
    {
        int i = -1;
        while (*(bool*)(dstPointer + (++i))) { }
        return Encoding.UTF8.GetString(dstPointer, i);
    }
}
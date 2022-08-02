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
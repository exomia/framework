#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;

namespace Exomia.Framework.Core.Extensions;

public static class StringBuilderExtensions
{
    public static void AppendFormatIfNotNull<T>(this StringBuilder sb, string format, T? value)
        where T : class
    {
        if(value != null) { sb.AppendFormat(format, value); }
    }
    
    public static void AppendFormatIfNotNull<T>(this StringBuilder sb, string format, T? value)
        where T : struct
    {
        if(value.HasValue) { sb.AppendFormat(format, value.Value); }
    }
}
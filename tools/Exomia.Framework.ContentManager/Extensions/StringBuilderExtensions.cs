#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Text;

namespace Exomia.Framework.ContentManager.Extensions;

/// <summary>
/// The string builder extensions class
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the format if not null using the specified sb
    /// </summary>
    /// <typeparam name="T">The </typeparam>
    /// <param name="sb">The sb</param>
    /// <param name="format">The format</param>
    /// <param name="value">The value</param>
    public static void AppendFormatIfNotNull<T>(this StringBuilder sb, string format, T? value)
        where T : class
    {
        if(value != null) { sb.AppendFormat(format, value); }
    }
    
    /// <summary>
    /// Appends the format if not null using the specified sb
    /// </summary>
    /// <typeparam name="T">The </typeparam>
    /// <param name="sb">The sb</param>
    /// <param name="format">The format</param>
    /// <param name="value">The value</param>
    public static void AppendFormatIfNotNull<T>(this StringBuilder sb, string format, T? value)
        where T : struct
    {
        // ReSharper disable once HeapView.BoxingAllocation
        if(value.HasValue) { sb.AppendFormat(format, value.Value); }
    }
}
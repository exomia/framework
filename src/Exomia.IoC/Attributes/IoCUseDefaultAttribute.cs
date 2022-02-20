#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.IoC.Attributes;

/// <summary> Attribute for ioc use default. This class cannot be inherited. </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class IoCUseDefaultAttribute : Attribute
{
    /// <summary> Gets the default value. </summary>
    /// <value> The default value or null. </value>
    public object? DefaultValue { get; }

    /// <summary> Initializes a new instance of the <see cref="IoCUseDefaultAttribute" /> class. </summary>
    /// <param name="defaultValue"> (Optional) The default value. </param>
    public IoCUseDefaultAttribute(object? defaultValue = null)
    {
        DefaultValue = defaultValue;
    }
}
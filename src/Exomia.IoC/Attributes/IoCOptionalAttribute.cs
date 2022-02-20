#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.IoC.Attributes;

/// <summary> Attribute for ioc optional. This class cannot be inherited. </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class IoCOptionalAttribute : Attribute
{
    /// <summary> Gets the type of the optional. </summary>
    /// <value> The type of the optional. </value>
    public Type OptionalType { get; }

    /// <summary> Initializes a new instance of the <see cref="IoCParameterFactoryAttribute" /> class. </summary>
    /// <param name="optionalType"> The type of the optional. </param>
    /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
    public IoCOptionalAttribute(Type optionalType)
    {
        OptionalType = optionalType ?? throw new ArgumentNullException(nameof(optionalType));
    }
}
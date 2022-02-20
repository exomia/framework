#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.IoC.Attributes;

/// <summary> Attribute for ioc parameter factory. This class cannot be inherited. </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class IoCParameterFactoryAttribute : Attribute
{
    /// <summary> Gets the type of the factory. </summary>
    /// <value> The type of the factory. </value>
    public Type FactoryType { get; }

    /// <summary> Gets the name of the method. </summary>
    /// <value> The name of the method. </value>
    public string MethodName { get; }

    /// <summary> Initializes a new instance of the <see cref="IoCParameterFactoryAttribute" /> class. </summary>
    /// <param name="factoryType"> Type of the factory. </param>
    /// <param name="methodName">  Name of the method. </param>
    public IoCParameterFactoryAttribute(Type factoryType, string methodName)
    {
        FactoryType = factoryType ?? throw new ArgumentNullException(nameof(factoryType));
        MethodName  = methodName ?? throw new ArgumentNullException(nameof(methodName));
    }
}
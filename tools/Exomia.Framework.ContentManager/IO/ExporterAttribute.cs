#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.IO;

/// <summary>
///     Attribute for exporter. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExporterAttribute : Attribute
{
    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; }

    /// <inheritdoc />
    public ExporterAttribute(string name)
    {
        Name = name;
    }
}
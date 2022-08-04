#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.ContentManager.IO;

/// <summary>
///     Attribute for importer. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ImporterAttribute : Attribute
{
    /// <summary>
    ///     Gets the name.
    /// </summary>
    /// <value>
    ///     The name.
    /// </value>
    public string Name { get; }

    /// <summary>
    ///     Gets the extensions.
    /// </summary>
    /// <value>
    ///     The extensions.
    /// </value>
    public string[] Extensions { get; }

    /// <inheritdoc />
    public ImporterAttribute(string name, params string[] extensions)
    {
        Name       = name;
        Extensions = extensions;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Content;

public sealed partial class ContentManager 
{
    /// <summary> The <see cref="ContentManager"/> configuration. </summary>
    public sealed class Configuration
    {
        /// <summary> Get or sets the root directory. </summary>
        /// <value> The pathname of the root directory. </value>
        public string RootDirectory { get; init; } = string.Empty;
    }
}
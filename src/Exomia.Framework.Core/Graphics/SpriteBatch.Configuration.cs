#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteBatch
{
    /// <summary> A configuration. This class cannot be inherited. </summary>
    public sealed class Configuration
    {
        public bool  Center                { get; init; } = false;
        public bool  AnisotropyEnable      { get; init; } = false;
        public float MaxAnisotropy         { get; init; } = 1.0f;
        public uint  DescriptorPoolMaxSets { get; init; } = 128u;
    }
}
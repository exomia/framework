#region License

// Copyright (c) 2018-2023, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Graphics;

/// <content>
/// The msdf font class
/// </content>
public sealed partial class MsdfFont
{
    /// <summary>
    /// The atlas info class
    /// </summary>
    public sealed class AtlasInfo
    {
        /// <summary>
        /// Gets or inits the value of the type
        /// </summary>
#if NET7_0_OR_GREATER
        public required string Type { get; init; }
#else
        public string Type { get; init; } = null!;
#endif

        /// <summary>
        /// Gets or inits the value of the distance range
        /// </summary>
#if NET7_0_OR_GREATER
        public required int DistanceRange { get; init; }
#else
        public int DistanceRange { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the size
        /// </summary>
#if NET7_0_OR_GREATER
        public required double Size { get; init; }
#else
        public double Size { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the width
        /// </summary>
#if NET7_0_OR_GREATER
        public required int Width { get; init; }
#else
        public int Width { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the height
        /// </summary>
#if NET7_0_OR_GREATER
        public required int Height { get; init; }
#else
        public int Height { get; init; }
#endif
        /// <summary>
        /// Gets or inits the value of the y origin
        /// </summary>
#if NET7_0_OR_GREATER
        public required string YOrigin { get; init; }
#else
        public string YOrigin { get; init; } = null!;
#endif
    }
}
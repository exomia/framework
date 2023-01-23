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
public partial class MsdfFont
{
    /// <summary>
    /// The atlas info class
    /// </summary>
    public sealed class AtlasInfo
    {
        /// <summary>
        /// Gets or inits the value of the type
        /// </summary>
        public string Type { get; init; }

        /// <summary>
        /// Gets or inits the value of the distance range
        /// </summary>
        public int DistanceRange { get; init; }

        /// <summary>
        /// Gets or inits the value of the size
        /// </summary>
        public double Size { get; init; }

        /// <summary>
        /// Gets or inits the value of the width
        /// </summary>
        public int Width { get; init; }

        /// <summary>
        /// Gets or inits the value of the height
        /// </summary>
        public int Height { get; init; }

        /// <summary>
        /// Gets or inits the value of the y origin
        /// </summary>
        public string YOrigin { get; init; }
    }
}
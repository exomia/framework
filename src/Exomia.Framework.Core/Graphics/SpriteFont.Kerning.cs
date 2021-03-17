#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.ContentSerialization;

namespace Exomia.Framework.Core.Graphics
{
    public sealed partial class SpriteFont
    {
        /// <summary>
        ///     A kerning.
        /// </summary>
        [ContentSerializable(typeof(SpriteFontKerningCR), typeof(SpriteFontKerningCW))]
        public struct Kerning
        {
            /// <summary>
            ///     The first.
            /// </summary>
            public int First;

            /// <summary>
            ///     The second.
            /// </summary>
            public int Second;

            /// <summary>
            ///     The offset.
            /// </summary>
            public int Offset;
        }
    }
}
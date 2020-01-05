#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    /// <summary>
    ///     A color carriage return. This class cannot be inherited.
    /// </summary>
    sealed class ColorCR : ContentSerializationReader<Color>
    {
        /// <inheritdoc />
        public override Color ReadContext(ContentSerializationContext context)
        {
            return new Color
            {
                A = context.Get<byte>("A"),
                R = context.Get<byte>("R"),
                G = context.Get<byte>("G"),
                B = context.Get<byte>("B")
            };
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    /// <summary>
    ///     A color cw. This class cannot be inherited.
    /// </summary>
    sealed class ColorCW : ContentSerializationWriter<Color>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Color obj)
        {
            context.Set(nameof(Color.A), obj.A);
            context.Set(nameof(Color.R), obj.R);
            context.Set(nameof(Color.G), obj.G);
            context.Set(nameof(Color.B), obj.B);
        }
    }
}
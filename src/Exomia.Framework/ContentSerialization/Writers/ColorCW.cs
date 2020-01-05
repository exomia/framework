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
            context.Set("A", obj.A);
            context.Set("R", obj.R);
            context.Set("G", obj.G);
            context.Set("B", obj.B);
        }
    }
}
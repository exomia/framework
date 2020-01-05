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
    ///     A rectangle fcw. This class cannot be inherited.
    /// </summary>
    sealed class RectangleFCW : ContentSerializationWriter<RectangleF>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, RectangleF obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
            context.Set("Width", obj.Width);
            context.Set("Height", obj.Height);
        }
    }

    /// <summary>
    ///     A rectangle cw. This class cannot be inherited.
    /// </summary>
    sealed class RectangleCW : ContentSerializationWriter<Rectangle>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Rectangle obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
            context.Set("Width", obj.Width);
            context.Set("Height", obj.Height);
        }
    }
}
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
            context.Set(nameof(RectangleF.X), obj.X);
            context.Set(nameof(RectangleF.Y), obj.Y);
            context.Set(nameof(RectangleF.Width), obj.Width);
            context.Set(nameof(RectangleF.Height), obj.Height);
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
            context.Set(nameof(Rectangle.X), obj.X);
            context.Set(nameof(Rectangle.Y), obj.Y);
            context.Set(nameof(Rectangle.Width), obj.Width);
            context.Set(nameof(Rectangle.Height), obj.Height);
        }
    }
}
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
    ///     A rectangle fcr. This class cannot be inherited.
    /// </summary>
    sealed class RectangleFCR : ContentSerializationReader<RectangleF>
    {
        /// <inheritdoc />
        public override RectangleF ReadContext(ContentSerializationContext context)
        {
            return new RectangleF
            {
                X      = context.Get<float>(nameof(RectangleF.X)),
                Y      = context.Get<float>(nameof(RectangleF.Y)),
                Width  = context.Get<float>(nameof(RectangleF.Width)),
                Height = context.Get<float>(nameof(RectangleF.Height))
            };
        }
    }

    /// <summary>
    ///     A rectangle carriage return. This class cannot be inherited.
    /// </summary>
    sealed class RectangleCR : ContentSerializationReader<Rectangle>
    {
        /// <inheritdoc />
        public override Rectangle ReadContext(ContentSerializationContext context)
        {
            return new Rectangle
            {
                X      = context.Get<int>(nameof(Rectangle.X)),
                Y      = context.Get<int>(nameof(Rectangle.Y)),
                Width  = context.Get<int>(nameof(Rectangle.Width)),
                Height = context.Get<int>(nameof(Rectangle.Height))
            };
        }
    }
}
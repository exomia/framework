#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Mathematics;

namespace Exomia.Framework.ContentSerialization.Readers
{
    sealed class RectangleContentSerializationReader : ContentSerializationReader<Rectangle>
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
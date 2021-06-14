#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.ContentSerialization.Writers
{
    internal sealed class RectangleContentSerializationWriter : ContentSerializationWriter<Rectangle>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Rectangle obj)
        {
            context.Set(nameof(Rectangle.X),      obj.X);
            context.Set(nameof(Rectangle.Y),      obj.Y);
            context.Set(nameof(Rectangle.Width),  obj.Width);
            context.Set(nameof(Rectangle.Height), obj.Height);
        }
    }
}
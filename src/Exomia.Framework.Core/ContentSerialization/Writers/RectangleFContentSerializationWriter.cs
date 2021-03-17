#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.ContentSerialization.Writers
{
    sealed class RectangleFContentSerializationWriter : ContentSerializationWriter<RectangleF>
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
}
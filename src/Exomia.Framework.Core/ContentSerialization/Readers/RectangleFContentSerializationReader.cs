#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.ContentSerialization.Readers
{
    internal sealed class RectangleFContentSerializationReader : ContentSerializationReader<RectangleF>
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
}
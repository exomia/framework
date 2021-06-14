#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;

namespace Exomia.Framework.Core.ContentSerialization.Writers
{
    internal sealed class Vector2ContentSerializationWriter : ContentSerializationWriter<Vector2>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Vector2 obj)
        {
            context.Set(nameof(Vector2.X), obj.X);
            context.Set(nameof(Vector2.Y), obj.Y);
        }
    }
}
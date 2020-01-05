#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    /// <summary>
    ///     A vector 2 cw. This class cannot be inherited.
    /// </summary>
    sealed class Vector2CW : ContentSerializationWriter<Vector2>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Vector2 obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
        }
    }
}
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
    ///     A vector 3 cw. This class cannot be inherited.
    /// </summary>
    sealed class Vector3CW : ContentSerializationWriter<Vector3>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Vector3 obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
            context.Set("Z", obj.Z);
        }
    }
}
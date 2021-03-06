﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    sealed class Vector3ContentSerializationWriter : ContentSerializationWriter<Vector3>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, Vector3 obj)
        {
            context.Set(nameof(Vector3.X), obj.X);
            context.Set(nameof(Vector3.Y), obj.Y);
            context.Set(nameof(Vector3.Z), obj.Z);
        }
    }
}
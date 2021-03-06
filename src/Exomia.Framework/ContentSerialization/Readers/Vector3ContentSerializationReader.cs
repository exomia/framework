﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    sealed class Vector3ContentSerializationReader : ContentSerializationReader<Vector3>
    {
        /// <inheritdoc />
        public override Vector3 ReadContext(ContentSerializationContext context)
        {
            return new Vector3
            {
                X = context.Get<float>(nameof(Vector3.X)),
                Y = context.Get<float>(nameof(Vector3.Y)),
                Z = context.Get<float>(nameof(Vector3.Z))
            };
        }
    }
}
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
    ///     A vector 2 carriage return. This class cannot be inherited.
    /// </summary>
    sealed class Vector2CR : ContentSerializationReader<Vector2>
    {
        /// <inheritdoc />
        public override Vector2 ReadContext(ContentSerializationContext context)
        {
            return new Vector2 { X = context.Get<float>("X"), Y = context.Get<float>("Y") };
        }
    }
}
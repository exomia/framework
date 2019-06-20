﻿#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    /// <summary>
    ///     A vector 3 carriage return. This class cannot be inherited.
    /// </summary>
    sealed class Vector3CR : AContentSerializationReader<Vector3>
    {
        /// <inheritdoc />
        public override Vector3 ReadContext(ContentSerializationContext context)
        {
            return new Vector3
            {
                X = context.Get<float>("X"), Y = context.Get<float>("Y"), Z = context.Get<float>("Z")
            };
        }
    }
}
﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.ContentSerialization.Readers
{
    sealed class ColorContentSerializationReader : ContentSerializationReader<VkColor>
    {
        /// <inheritdoc />
        public override VkColor ReadContext(ContentSerializationContext context)
        {
            return new VkColor
            {
                A = context.Get<byte>(nameof(Color.A)),
                R = context.Get<byte>(nameof(Color.R)),
                G = context.Get<byte>(nameof(Color.G)),
                B = context.Get<byte>(nameof(Color.B))
            };
        }
    }
}
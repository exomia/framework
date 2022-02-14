#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization.Readers
{
    internal sealed class ColorContentSerializationReader : ContentSerializationReader<VkColor>
    {
        /// <inheritdoc />
        public override VkColor ReadContext(ContentSerializationContext context)
        {
            return new VkColor
            {
                A = context.Get<byte>(nameof(VkColor.A)),
                R = context.Get<byte>(nameof(VkColor.R)),
                G = context.Get<byte>(nameof(VkColor.G)),
                B = context.Get<byte>(nameof(VkColor.B))
            };
        }
    }
}
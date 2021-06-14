#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Core.ContentSerialization.Writers
{
    internal sealed class ColorContentSerializationWriter : ContentSerializationWriter<VkColor>
    {
        /// <inheritdoc />
        public override void WriteContext(ContentSerializationContext context, VkColor obj)
        {
            context.Set(nameof(VkColor.A), obj.A);
            context.Set(nameof(VkColor.R), obj.R);
            context.Set(nameof(VkColor.G), obj.G);
            context.Set(nameof(VkColor.B), obj.B);
        }
    }
}
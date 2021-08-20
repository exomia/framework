#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Vulkan.Configurations
{
    /// <summary> Handler, called for various stages during the vulkan setup. </summary>
    /// <param name="context"> [in,out] If non-null, the context. </param>
    /// <returns> A bool indicating if the setup should progress. </returns>
    public unsafe delegate bool SetupContextHandler(VkContext* context);

    /// <summary> Handler, called for various actions. </summary>
    /// <param name="context"> [in,out] If non-null, the context. </param>
    public unsafe delegate void ContextActionHandler(VkContext* context);
}
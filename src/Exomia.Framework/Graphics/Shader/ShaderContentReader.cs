#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Content;

namespace Exomia.Framework.Graphics.Shader
{
    /// <summary>
    ///     A shader content reader. This class cannot be inherited.
    /// </summary>
    sealed class ShaderContentReader : IContentReader
    {
        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            IGraphicsDevice graphicsDevice =
                contentManager.ServiceRegistry.GetService<IGraphicsDevice>();
            return ShaderHelper.FromStream(graphicsDevice, parameters.Stream);
        }
    }
}
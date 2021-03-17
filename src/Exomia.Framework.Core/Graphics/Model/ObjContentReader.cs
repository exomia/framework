#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Content;
using Exomia.Framework.Core.Content.Loader;

namespace Exomia.Framework.Core.Graphics.Model
{
    /// <summary>
    ///     An object content reader. This class cannot be inherited.
    /// </summary>
    sealed class ObjContentReader : IContentReader
    {
        private static readonly IModelFileLoader<Obj> s_objFileLoader;

        static ObjContentReader()
        {
            s_objFileLoader = new ObjFileLoader();
        }

        /// <inheritdoc />
        public object? ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters)
        {
            return s_objFileLoader.Load(parameters.Stream);
        }
    }
}
#region License
// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.
#endregion

using System.Diagnostics.CodeAnalysis;
using Exomia.Framework.Core.Graphics;

namespace Exomia.Framework.Core.Content.E1.ContentReader;

sealed class E1ContentReaderFactory : IContentReaderFactory
{
    /// <inheritdoc />
    public bool TryCreate(Type type, [NotNullWhen(true)] out IContentReader? reader)
    {
        if (type == typeof(SpriteFont))
        {
            reader = new E1SpriteFontContentReader();
            return true;
        }
        
        if (type == typeof(Texture))
        {
            reader = new E1TextureContentReader();
            return true;
        }

        reader = null;
        return false;
    }
}

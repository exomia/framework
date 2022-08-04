#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

public sealed partial class SpriteFont
{
    internal delegate void RenderFont(
        Texture        texture,
        in Vector2     position,
        in Rectangle   sourceRectangle,
        in VkColor     color,
        float          rotation,
        in Vector2     origin,
        float          scale,
        float          opacity,
        TextureEffects effects,
        float          layerDepth);
}
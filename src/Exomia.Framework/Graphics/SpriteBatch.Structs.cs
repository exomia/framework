#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.InteropServices;
using Exomia.Framework.Mathematics;
using Exomia.Vulkan.Api.Core;

namespace Exomia.Framework.Graphics
{
    public sealed partial class SpriteBatch
    {
        internal struct SpriteInfo
        {
            public RectangleF     Source;
            public RectangleF     Destination;
            public Vector2        Origin;
            public float          Rotation;
            public float          Depth;
            public TextureEffects SpriteEffects;
            public VkColor          Color;
            public float          Opacity;
        }

        internal readonly struct TextureInfo
        {
            public readonly ShaderResourceView View;
            public readonly int                Width;
            public readonly int                Height;
            public readonly long               Ptr64;

            public TextureInfo(ShaderResourceView view, int width, int height)
            {
                View   = view;
                Width  = width;
                Height = height;
                Ptr64  = view.NativePointer.ToInt64();
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
        private struct VertexPositionColorTexture
        {
            [FieldOffset(0)]
            public float X;

            [FieldOffset(4)]
            public float Y;

            [FieldOffset(8)]
            public float Z;

            [FieldOffset(12)]
            public float W;

            [FieldOffset(16)]
            public float R;

            [FieldOffset(20)]
            public float G;

            [FieldOffset(24)]
            public float B;

            [FieldOffset(28)]
            public float A;

            [FieldOffset(32)]
            public float U;

            [FieldOffset(36)]
            public float V;
        }
    }
}
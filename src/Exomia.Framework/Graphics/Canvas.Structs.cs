#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using SharpDX;

namespace Exomia.Framework.Graphics
{
    public sealed partial class Canvas
    {
        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE * 4)]
        private struct Item
        {
            [FieldOffset(VERTEX_STRIDE * 0)]
            public VertexPositionColorTextureMode V1;

            [FieldOffset(VERTEX_STRIDE * 1)]
            public VertexPositionColorTextureMode V2;

            [FieldOffset(VERTEX_STRIDE * 2)]
            public VertexPositionColorTextureMode V3;

            [FieldOffset(VERTEX_STRIDE * 3)]
            public VertexPositionColorTextureMode V4;
        }

        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
        private struct VertexPositionColorTextureMode
        {
            [FieldOffset(0)]
            public float X;

            [FieldOffset(4)]
            public float Y;
            
            [FieldOffset(0)]
            public Vector2 XY;

            [FieldOffset(8)]
            public float Z;

            [FieldOffset(12)]
            public float W;

            [FieldOffset(8)]
            public long ZW;

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

            [FieldOffset(40)]
            public float M;

            [FieldOffset(44)]
            public float O;
        }
    }
}
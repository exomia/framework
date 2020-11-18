#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

namespace Exomia.Framework.Graphics
{
    public sealed partial class Canvas
    {
        [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
        private struct VertexPositionColorTextureMode
        {
            [FieldOffset(0)]
            public float X;

            [FieldOffset(4)]
            public float Y;

            [FieldOffset(8)]
            public readonly float Z;

            [FieldOffset(12)]
            public readonly float W;

            [FieldOffset(16)]
            public float R;

            [FieldOffset(20)]
            public float G;

            [FieldOffset(24)]
            public float B;

            [FieldOffset(28)]
            public float A;

            [FieldOffset(8)]
            public readonly float U;

            [FieldOffset(12)]
            public readonly float V;

            [FieldOffset(32)]
            public float M;
        }
    }
}
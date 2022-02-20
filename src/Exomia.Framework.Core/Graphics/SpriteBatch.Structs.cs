﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Numerics;
using System.Runtime.InteropServices;
using Exomia.Framework.Core.Mathematics;

namespace Exomia.Framework.Core.Graphics;

public sealed unsafe partial class SpriteBatch
{
    internal struct SpriteInfo
    {
        public RectangleF     Source;
        public RectangleF     Destination;
        public Vector2        Origin;
        public float          Rotation;
        public float          Depth;
        public TextureEffects SpriteEffects;
        public VkColor        Color;
        public float          Opacity;
    }

    internal readonly struct TextureInfo
    {
        //public readonly ShaderResourceView View;
        public readonly int  Width;
        public readonly int  Height;
        public readonly long Ptr64;

        public TextureInfo(int width, int height)
        {
            Width  = width;
            Height = height;
            Ptr64  = 0L;
        }
    }

    private struct VkSpriteBatchContext
    {
        public VkPipelineLayout      PipelineLayout;
        public VkDescriptorPool      DescriptorPool;
        public VkDescriptorSetLayout DescriptorSetLayout;
        public VkDescriptorSet*      DescriptorSets;
        public VkCommandBuffer*      CommandBuffers;

        public static VkSpriteBatchContext Create()
        {
            VkSpriteBatchContext context;
            context.PipelineLayout      = VkPipelineLayout.Null;
            context.DescriptorPool      = VkDescriptorPool.Null;
            context.DescriptorSetLayout = VkDescriptorSetLayout.Null;
            context.DescriptorSets      = null;
            context.CommandBuffers      = null;
            return context;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE)]
    private struct VertexPositionColorTexture
    {
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local

        [FieldOffset(0)]  public float   X;
        [FieldOffset(4)]  public float   Y;
        [FieldOffset(8)]  public float   Z;
        [FieldOffset(12)] public float   W;
        [FieldOffset(16)] public float   R;
        [FieldOffset(20)] public float   G;
        [FieldOffset(24)] public float   B;
        [FieldOffset(28)] public float   A;
        [FieldOffset(16)] public VkColor Color;
        [FieldOffset(32)] public float   U;
        [FieldOffset(36)] public float   V;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}, {nameof(Color)}: {Color}, {nameof(U)}: {U}, {nameof(V)}: {V}";
        }

        // ReSharper enable FieldCanBeMadeReadOnly.Local
        // ReSharper enable MemberCanBePrivate.Local
    }
}
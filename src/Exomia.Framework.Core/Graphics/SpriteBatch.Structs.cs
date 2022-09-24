#region License

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

/// <content> A sprite batch. This class cannot be inherited. </content>
public sealed unsafe partial class SpriteBatch
{
    internal struct SpriteInfo
    {
        public RectangleF     Destination;
        public bool           ScaleDestination;
        public Rectangle?     Source;
        public VkColor        Color;
        public float          Rotation;
        public Vector2        Origin;
        public float          Opacity;
        public TextureEffects Effects;
        public float          Depth;
    }

    internal struct TextureInfo
    {
        public readonly ulong            ID;
        public readonly uint             Width;
        public readonly uint             Height;
        public          VkDescriptorSet* DescriptorSets;

        public TextureInfo(ulong id, uint width, uint height)
        {
            ID             = id;
            Width          = width;
            Height         = height;
            DescriptorSets = null;
        }
    }

    private struct VkSpriteBatchContext
    {
        public VkSampler             TextureSampler;
        public VkPipelineLayout      PipelineLayout;
        public VkDescriptorPool      DescriptorPool;
        public VkDescriptorPool      TextureDescriptorPool;
        public VkDescriptorSetLayout DescriptorSetLayout;
        public VkDescriptorSetLayout TextureDescriptorSetLayout;
        public VkDescriptorSet*      DescriptorSets;

        public static VkSpriteBatchContext Create()
        {
            VkSpriteBatchContext context;
            context.TextureSampler             = VkSampler.Null;
            context.PipelineLayout             = VkPipelineLayout.Null;
            context.DescriptorPool             = VkDescriptorPool.Null;
            context.TextureDescriptorPool      = VkDescriptorPool.Null;
            context.DescriptorSetLayout        = VkDescriptorSetLayout.Null;
            context.DescriptorSets             = null;
            context.TextureDescriptorSetLayout = VkDescriptorSetLayout.Null;
            return context;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE, Pack = 4)]
    private struct Vertex
    {
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local

        [FieldOffset(0)]  public float   X;
        [FieldOffset(4)]  public float   Y;
        [FieldOffset(0)]  public Vector2 XY;
        [FieldOffset(8)]  public float   Z;
        [FieldOffset(12)] public float   W;
        [FieldOffset(16)] public float   R;
        [FieldOffset(20)] public float   G;
        [FieldOffset(24)] public float   B;
        [FieldOffset(28)] public float   A;
        [FieldOffset(16)] public VkColor Color;
        [FieldOffset(32)] public float   U;
        [FieldOffset(36)] public float   V;
        [FieldOffset(32)] public Vector2 UV;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}, {nameof(Color)}: {Color}, {nameof(U)}: {U}, {nameof(V)}: {V}";
        }

        // ReSharper enable FieldCanBeMadeReadOnly.Local
        // ReSharper enable MemberCanBePrivate.Local
    }
}
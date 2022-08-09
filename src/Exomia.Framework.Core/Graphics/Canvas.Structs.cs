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

/// <content> A canvas. This class cannot be inherited. </content>
public sealed unsafe partial class Canvas
{
    private struct VkCanvasContext
    {
        public VkSampler             TextureSampler;
        public VkPipelineLayout      PipelineLayout;
        public VkDescriptorPool      UboDescriptorPool;
        public VkDescriptorPool      TextureDescriptorPool;
        public VkDescriptorSetLayout UboDescriptorSetLayout;
        public VkDescriptorSetLayout TextureDescriptorSetLayout;
        public VkDescriptorSet*      UboDescriptorSets;

        public static VkCanvasContext Create()
        {
            VkCanvasContext context;
            context.TextureSampler             = VkSampler.Null;
            context.PipelineLayout             = VkPipelineLayout.Null;
            context.UboDescriptorPool          = VkDescriptorPool.Null;
            context.TextureDescriptorPool      = VkDescriptorPool.Null;
            context.UboDescriptorSetLayout     = VkDescriptorSetLayout.Null;
            context.UboDescriptorSets          = null;
            context.TextureDescriptorSetLayout = VkDescriptorSetLayout.Null;
            return context;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct Item
    {
        private const int OFFSET_TYPE_KIND  = 0;
        private const int OFFSET_VK_COLOR   = OFFSET_TYPE_KIND + 4;
        private const int OFFSET_ROTATION   = OFFSET_VK_COLOR  + (4 * 4);
        private const int OFFSET_ORIGIN     = OFFSET_ROTATION  + 4;
        private const int OFFSET_OPACITY    = OFFSET_ORIGIN    + (4 * 2);
        private const int OFFSET_TYPE_START = OFFSET_OPACITY   + 4;

        public const int NONE_TYPE           = 0;
        public const int ARC_TYPE            = 1;
        public const int LINE_TYPE           = 2;
        public const int POLYGON_TYPE        = 3;
        public const int RECTANGLE_TYPE      = 4;
        public const int TEXTURE_TYPE        = 5;
        public const int TRIANGLE_TYPE       = 6;
        public const int FILL_ARC_TYPE       = 7;
        public const int FILL_POLYGON_TYPE   = 8;
        public const int FILL_RECTANGLE_TYPE = 9;
        public const int FILL_TRIANGLE_TYPE  = 10;

        [FieldOffset(OFFSET_TYPE_KIND)] public int     Type;
        [FieldOffset(OFFSET_VK_COLOR)]  public VkColor Color;
        [FieldOffset(OFFSET_ROTATION)]  public float   Rotation;
        [FieldOffset(OFFSET_ORIGIN)]    public Vector2 Origin;
        [FieldOffset(OFFSET_OPACITY)]   public float   Opacity;

        [FieldOffset(OFFSET_TYPE_START)] public          ArcType       ArcType;
        [FieldOffset(OFFSET_TYPE_START)] public          LineType      LineType;
        [FieldOffset(OFFSET_TYPE_START)] public          PolygonType   PolygonType;
        [FieldOffset(OFFSET_TYPE_START)] public          RectangleType RectangleType;
        [FieldOffset(OFFSET_TYPE_START)] public readonly TextureType   TextureType;
        [FieldOffset(OFFSET_TYPE_START)] public          TriangleType  TriangleType;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct ArcType
    {
        public Arc2  Arc;
        public float LineWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LineType
    {
        public Line2 Line;
        public float LengthFactor;
        public float LineWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PolygonType
    {
        public Vector2* Vertices;
        public int      VerticesCount;
        public float    LineWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RectangleType
    {
        public RectangleF Destination;
        public float      LineWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TextureType
    {
        public readonly TextureInfo    TextureInfo;
        public readonly RectangleF     Destination;
        public readonly bool           ScaleDestination;
        public readonly Rectangle?     SourceRectangle;
        public readonly TextureEffects Effects;
        public readonly float          Mode;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TriangleType
    {
        public Triangle2 Triangle;
        public float     LineWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct TextureInfo
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


    [StructLayout(LayoutKind.Explicit, Size = VERTEX_STRIDE, Pack = 4)]
    private struct VertexPositionColorTextureMode
    {
        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local

        [FieldOffset(0)]  public float   X;
        [FieldOffset(4)]  public float   Y;
        [FieldOffset(0)]  public Vector2 XY;
        [FieldOffset(8)]  public float   Z;
        [FieldOffset(12)] public float   W;
        [FieldOffset(8)]  public long    ZW;
        [FieldOffset(16)] public float   R;
        [FieldOffset(20)] public float   G;
        [FieldOffset(24)] public float   B;
        [FieldOffset(28)] public float   A;
        [FieldOffset(16)] public VkColor Color;
        [FieldOffset(32)] public float   U;
        [FieldOffset(36)] public float   V;
        [FieldOffset(32)] public Vector2 UV;
        [FieldOffset(40)] public float   M;
        [FieldOffset(44)] public float   O;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Z)}: {Z}, {nameof(W)}: {W}, {nameof(Color)}: {Color}, {nameof(U)}: {U}, {nameof(V)}: {V}, {nameof(M)}: {M}, {nameof(O)}: {O}";
        }

        // ReSharper enable FieldCanBeMadeReadOnly.Local
        // ReSharper enable MemberCanBePrivate.Local
    }
}
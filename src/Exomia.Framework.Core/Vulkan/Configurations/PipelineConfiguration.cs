#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkPrimitiveTopology;
using static Exomia.Vulkan.Api.Core.VkPolygonMode;
using static Exomia.Vulkan.Api.Core.VkFrontFace;
using static Exomia.Vulkan.Api.Core.VkStencilOp;
using static Exomia.Vulkan.Api.Core.VkCullModeFlagBits;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkLogicOp;
using static Exomia.Vulkan.Api.Core.VkBlendOp;
using static Exomia.Vulkan.Api.Core.VkBlendFactor;
using static Exomia.Vulkan.Api.Core.VkColorComponentFlagBits;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A pipeline configuration. This class cannot be inherited. </summary>
public sealed class PipelineConfiguration
{
    /// <summary> Gets the vertex input configuration. </summary>
    /// <value> The vertex input configuration. </value>
    public VertexInputConfiguration VertexInput { get; }

    /// <summary> Gets the input assembly configuration. </summary>
    /// <value> The input assembly configuration. </value>
    public InputAssemblyConfiguration InputAssembly { get; } = new InputAssemblyConfiguration();

    /// <summary> Gets the viewport configuration. </summary>
    /// <value> The viewport configuration. </value>
    public ViewportConfiguration Viewport { get; } = new ViewportConfiguration();

    /// <summary> Gets the rasterization configuration. </summary>
    /// <value> The rasterization configuration. </value>
    public RasterizationConfiguration Rasterization { get; } = new RasterizationConfiguration();

    /// <summary> Gets the multisample configuration. </summary>
    /// <value> The multisample configuration. </value>
    public MultisampleConfiguration Multisample { get; } = new MultisampleConfiguration();

    /// <summary> Gets the state of the depth stencil configuration. </summary>
    /// <value> The depth stencil state configuration. </value>
    public DepthStencilStateConfiguration DepthStencilState { get; } = new DepthStencilStateConfiguration();

    /// <summary> Gets the color blend attachment configuration. </summary>
    /// <value> The color blend attachment configuration. </value>
    public ColorBlendAttachmentConfiguration ColorBlendAttachment { get; } = new ColorBlendAttachmentConfiguration();

    /// <summary> Gets the color blend configuration. </summary>
    /// <value> The color blend configuration. </value>
    public ColorBlendConfiguration ColorBlend { get; } = new ColorBlendConfiguration();

    /// <summary> Gets the state of the dynamic configuration. </summary>
    /// <value> The dynamic state configuration. </value>
    public DynamicStateConfiguration DynamicState { get; } = new DynamicStateConfiguration();

    /// <summary> Gets the graphics pipeline configuration. </summary>
    /// <value> The graphics pipeline configuration. </value>
    public GraphicsPipelineConfiguration GraphicsPipeline { get; } = new GraphicsPipelineConfiguration();

    public PipelineConfiguration(VertexInputConfiguration vertexInputConfiguration)
    {
        VertexInput = vertexInputConfiguration ?? throw new ArgumentNullException(nameof(vertexInputConfiguration));
    }
}

/// <summary> An input assembly configuration. This class cannot be inherited. </summary>
public sealed class VertexInputConfiguration
{
    public uint Binding { get; set; }
    public uint Stride  { get; set; }

    public unsafe delegate*<
        VkContext*,                         /* context */
        uint*,                              /* attributesCount */
        VkVertexInputAttributeDescription*, /* pAttributeDescriptions */
        void> CreateVertexInputAttributeDescriptions { get; set; } = null;
}

/// <summary> An input assembly configuration. This class cannot be inherited. </summary>
public sealed class InputAssemblyConfiguration
{
    public VkPipelineInputAssemblyStateCreateFlags Flags                  { get; set; } = 0;
    public VkPrimitiveTopology                     Topology               { get; set; } = VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST;
    public VkBool32                                PrimitiveRestartEnable { get; set; } = VkBool32.False;
}

/// <summary> A viewport configuration. This class cannot be inherited. </summary>
public sealed class ViewportConfiguration
{
    public float MinDepth { get; set; } = 0.0f;
    public float MaxDepth { get; set; } = 1.0f;
}

/// <summary> A rasterization configuration. This class cannot be inherited. </summary>
public sealed class RasterizationConfiguration
{
    public VkPipelineRasterizationStateCreateFlags Flags                   { get; set; } = 0;
    public VkBool32                                DepthClampEnable        { get; set; } = VkBool32.False;
    public VkBool32                                RasterizerDiscardEnable { get; set; } = VkBool32.False;
    public VkPolygonMode                           PolygonMode             { get; set; } = VK_POLYGON_MODE_FILL;
    public VkCullModeFlagBits                      CullMode                { get; set; } = VK_CULL_MODE_BACK_BIT;
    public VkFrontFace                             FrontFace               { get; set; } = VK_FRONT_FACE_CLOCKWISE;
    public VkBool32                                DepthBiasEnable         { get; set; } = VkBool32.False;
    public float                                   DepthBiasConstantFactor { get; set; } = 0.0f;
    public float                                   DepthBiasClamp          { get; set; } = 0.0f;
    public float                                   DepthBiasSlopeFactor    { get; set; } = 0.0f;
    public float                                   LineWidth               { get; set; } = 1.0f;
}

/// <summary> A multisample configuration. This class cannot be inherited. </summary>
public sealed unsafe class MultisampleConfiguration
{
    public VkPipelineMultisampleStateCreateFlags Flags                 { get; set; } = 0;
    public VkSampleCountFlagBits                 RasterizationSamples  { get; set; } = VK_SAMPLE_COUNT_1_BIT;
    public VkBool32                              SampleShadingEnable   { get; set; } = VkBool32.False;
    public float                                 MinSampleShading      { get; set; } = 1.0f;
    public uint*                                 SampleMask            { get; set; } = null;
    public VkBool32                              AlphaToCoverageEnable { get; set; } = VkBool32.False;
    public VkBool32                              AlphaToOneEnable      { get; set; } = VkBool32.False;
}

/// <summary> A depth stencil configuration. This class cannot be inherited. </summary>
public sealed class DepthStencilStateConfiguration
{
    public VkPipelineDepthStencilStateCreateFlagBits Flags                 { get; set; } = 0;
    public VkBool32                                  DepthTestEnable       { get; set; } = VkBool32.True;
    public VkBool32                                  DepthWriteEnable      { get; set; } = VkBool32.True;
    public VkCompareOp                               DepthCompareOp        { get; set; } = VK_COMPARE_OP_LESS_OR_EQUAL;
    public VkBool32                                  DepthBoundsTestEnable { get; set; } = VkBool32.False;
    public VkBool32                                  StencilTestEnable     { get; set; } = VkBool32.False;

    public VkStencilOpState Front { get; set; } = new VkStencilOpState
    {
        failOp      = VK_STENCIL_OP_KEEP,
        passOp      = VK_STENCIL_OP_KEEP,
        depthFailOp = VK_STENCIL_OP_KEEP,
        compareOp   = VK_COMPARE_OP_NEVER,
        compareMask = ~0u,
        writeMask   = ~0u,
        reference   = 0u
    };

    public VkStencilOpState Back { get; set; } = new VkStencilOpState
    {
        failOp      = VK_STENCIL_OP_KEEP,
        passOp      = VK_STENCIL_OP_KEEP,
        depthFailOp = VK_STENCIL_OP_KEEP,
        compareOp   = VK_COMPARE_OP_NEVER,
        compareMask = ~0u,
        writeMask   = ~0u,
        reference   = 0u
    };

    public float MinDepthBounds { get; set; } = 0.0f;
    public float MaxDepthBounds { get; set; } = 1.0f;
}

/// <summary> A color blend attachment configuration. This class cannot be inherited. </summary>
public sealed class ColorBlendAttachmentConfiguration
{
    public VkBool32      BlendEnable         { get; set; } = VkBool32.True;
    public VkBlendFactor SrcColorBlendFactor { get; set; } = VK_BLEND_FACTOR_SRC_ALPHA;
    public VkBlendFactor DstColorBlendFactor { get; set; } = VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA;
    public VkBlendOp     ColorBlendOp        { get; set; } = VK_BLEND_OP_ADD;
    public VkBlendFactor SrcAlphaBlendFactor { get; set; } = VK_BLEND_FACTOR_ONE;
    public VkBlendFactor DstAlphaBlendFactor { get; set; } = VK_BLEND_FACTOR_ZERO;
    public VkBlendOp     AlphaBlendOp        { get; set; } = VK_BLEND_OP_ADD;

    public VkColorComponentFlagBits ColorWriteMask { get; set; } =
        VK_COLOR_COMPONENT_R_BIT | VK_COLOR_COMPONENT_G_BIT | VK_COLOR_COMPONENT_B_BIT | VK_COLOR_COMPONENT_A_BIT;
}

/// <summary> A color blend configuration. This class cannot be inherited. </summary>
public sealed class ColorBlendConfiguration
{
    public VkPipelineColorBlendStateCreateFlagBits Flags          { get; set; } = 0;
    public VkBool32                                LogicOpEnable  { get; set; } = VkBool32.False;
    public VkLogicOp                               LogicOp        { get; set; } = VK_LOGIC_OP_NO_OP;
    public VkColor                                 BlendConstants { get; set; } = VkColors.Zero;
}

/// <summary> A dynamic state configuration. This class cannot be inherited. </summary>
public sealed class DynamicStateConfiguration
{
    public VkDynamicState[]? States { get; set; } = null;
}

public sealed class GraphicsPipelineConfiguration
{
    public VkPipelineCreateFlagBits Flags   { get; set; } = VkPipelineCreateFlagBits.VK_PIPELINE_CREATE_ALLOW_DERIVATIVES_BIT;
    public uint                     Subpass { get; set; } = 0u;
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkAttachmentLoadOp;
using static Exomia.Vulkan.Api.Core.VkAttachmentStoreOp;
using static Exomia.Vulkan.Api.Core.VkImageLayout;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;
using static Exomia.Vulkan.Api.Core.VkPipelineBindPoint;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations;

/// <summary> A render pass configuration. This class cannot be inherited. </summary>
public sealed class RenderPassConfiguration
{
    public VkRenderPassCreateFlagBits Flags { get; set; } = 0;

    public unsafe delegate*<
        SwapchainContext*,     /* context */
        uint*,                 /* dependencyCount */
        VkSubpassDependency2*, /* pDependencies */
        void> CreateAdditionalDependencies { get; set; } = null;

    public unsafe delegate*<
        SwapchainContext*,      /* context */
        uint*,                  /* subpassCount */
        VkSubpassDescription2*, /* pSubpasses */
        void> CreateAdditionalSubpasses { get; set; } = null;

    /// <summary> Gets the color attachment configuration. </summary>
    /// <value> The color attachment configuration. </value>
    public (AttachmentConfiguration attachmentConfiguration, AttachmentReferenceConfiguration attachmentReferenceConfiguration)[] ColorAttachments { get; set; }
        = new (AttachmentConfiguration, AttachmentReferenceConfiguration)[1]
        {
            (new AttachmentConfiguration
            {
                Flags          = 0,
                Samples        = VK_SAMPLE_COUNT_1_BIT,
                LoadOp         = VK_ATTACHMENT_LOAD_OP_CLEAR,
                StoreOp        = VK_ATTACHMENT_STORE_OP_STORE,
                StencilLoadOp  = VK_ATTACHMENT_LOAD_OP_DONT_CARE,
                StencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE,
                InitialLayout  = VK_IMAGE_LAYOUT_UNDEFINED,
                FinalLayout    = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR
            }, new AttachmentReferenceConfiguration
            {
                Layout     = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL,
                AspectMask = VK_IMAGE_ASPECT_COLOR_BIT
            })
        };

    /// <summary> Gets the depth/stencil attachment configuration. </summary>
    /// <value> The depth/stencil attachment configuration. </value>
    public (AttachmentConfiguration attachmentConfiguration, AttachmentReferenceConfiguration attachmentReferenceConfiguration) DepthStencilAttachment { get; }
        = (new AttachmentConfiguration
        {
            Flags          = 0,
            Samples        = VK_SAMPLE_COUNT_1_BIT,
            LoadOp         = VK_ATTACHMENT_LOAD_OP_CLEAR,
            StoreOp        = VK_ATTACHMENT_STORE_OP_DONT_CARE,
            StencilLoadOp  = VK_ATTACHMENT_LOAD_OP_DONT_CARE,
            StencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE,
            InitialLayout  = VK_IMAGE_LAYOUT_UNDEFINED,
            FinalLayout    = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL
        }, new AttachmentReferenceConfiguration
        {
            Layout     = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL,
            AspectMask = VK_IMAGE_ASPECT_DEPTH_BIT | VK_IMAGE_ASPECT_STENCIL_BIT
        });

    /// <summary> Gets the color attachment configuration. </summary>
    /// <value> The color attachment configuration. </value>
    public (AttachmentConfiguration attachmentConfiguration, AttachmentReferenceConfiguration attachmentReferenceConfiguration)[] InputAttachments { get; set; }
        = Array.Empty<(AttachmentConfiguration, AttachmentReferenceConfiguration)>();

    /// <summary> Gets the subpass configuration. </summary>
    /// <value> The subpass configuration. </value>
    public SubpassConfiguration Subpass { get; } = new SubpassConfiguration();
}

/// <summary> An attachment configuration. This class cannot be inherited. </summary>
public sealed unsafe class AttachmentConfiguration
{
    public void*                           Next           { get; set; } = null;
    public VkAttachmentDescriptionFlagBits Flags          { get; set; }
    public VkSampleCountFlagBits           Samples        { get; set; }
    public VkAttachmentLoadOp              LoadOp         { get; set; }
    public VkAttachmentStoreOp             StoreOp        { get; set; }
    public VkAttachmentLoadOp              StencilLoadOp  { get; set; }
    public VkAttachmentStoreOp             StencilStoreOp { get; set; }
    public VkImageLayout                   InitialLayout  { get; set; }
    public VkImageLayout                   FinalLayout    { get; set; }
}

/// <summary> An attachment reference configuration. This class cannot be inherited. </summary>
public sealed unsafe class AttachmentReferenceConfiguration
{
    public void*                 Next       { get; set; } = null;
    public VkImageLayout         Layout     { get; set; }
    public VkImageAspectFlagBits AspectMask { get; set; }
}

/// <summary> A subpass configuration. This class cannot be inherited. </summary>
public sealed unsafe class SubpassConfiguration
{
    public void*                        Next              { get; set; } = null;
    public VkSubpassDescriptionFlagBits Flags             { get; set; } = 0;
    public VkPipelineBindPoint          PipelineBindPoint { get; set; } = VK_PIPELINE_BIND_POINT_GRAPHICS;
    public uint                         ViewMask          { get; set; } = 0u;
}
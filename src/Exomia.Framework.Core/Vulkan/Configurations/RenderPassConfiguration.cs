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
        VkContext*,            /* context */
        uint*,                 /* dependencyCount */
        VkSubpassDependency2*, /* pDependencies */
        void> CreateAdditionalDependencies { get; set; } = null;

    public unsafe delegate*<
        VkContext*,             /* context */
        uint*,                  /* subpassCount */
        VkSubpassDescription2*, /* pSubpasses */
        void> CreateAdditionalSubpasses { get; set; } = null;

    /// <summary> Gets the attachment configuration. </summary>
    /// <value> The attachment configuration. </value>
    public AttachmentConfiguration Attachment { get; } = new AttachmentConfiguration();

    /// <summary> Gets the attachment reference configuration. </summary>
    /// <value> The attachment reference configuration. </value>
    public AttachmentReferenceConfiguration AttachmentReference { get; } = new AttachmentReferenceConfiguration();

    /// <summary> Gets the subpass configuration. </summary>
    /// <value> The subpass configuration. </value>
    public SubpassConfiguration Subpass { get; } = new SubpassConfiguration();
}

/// <summary> An attachment configuration. This class cannot be inherited. </summary>
public sealed class AttachmentConfiguration
{
    public VkAttachmentDescriptionFlagBits Flags          { get; set; } = 0;
    public VkSampleCountFlagBits           Samples        { get; set; } = VK_SAMPLE_COUNT_1_BIT;
    public VkAttachmentLoadOp              LoadOp         { get; set; } = VK_ATTACHMENT_LOAD_OP_CLEAR;
    public VkAttachmentStoreOp             StoreOp        { get; set; } = VK_ATTACHMENT_STORE_OP_STORE;
    public VkAttachmentLoadOp              StencilLoadOp  { get; set; } = VK_ATTACHMENT_LOAD_OP_DONT_CARE;
    public VkAttachmentStoreOp             StencilStoreOp { get; set; } = VK_ATTACHMENT_STORE_OP_DONT_CARE;
    public VkImageLayout                   InitialLayout  { get; set; } = VK_IMAGE_LAYOUT_UNDEFINED;
    public VkImageLayout                   FinalLayout    { get; set; } = VK_IMAGE_LAYOUT_PRESENT_SRC_KHR;
}

/// <summary> An attachment reference configuration. This class cannot be inherited. </summary>
public sealed class AttachmentReferenceConfiguration
{
    public VkImageLayout         Layout     { get; set; } = VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL;
    public VkImageAspectFlagBits AspectMask { get; set; } = VK_IMAGE_ASPECT_COLOR_BIT;
}

/// <summary> A subpass configuration. This class cannot be inherited. </summary>
public sealed class SubpassConfiguration
{
    public VkSubpassDescriptionFlagBits Flags             { get; set; } = 0;
    public VkPipelineBindPoint          PipelineBindPoint { get; set; } = VK_PIPELINE_BIND_POINT_GRAPHICS;
    public uint                         ViewMask          { get; set; } = 0u;

    public unsafe delegate*<
        VkContext*,              /* context */
        uint*,                   /* inputAttachmentCount */
        VkAttachmentReference2*, /* pInputAttachments */
        void> CreateAdditionalInputAttachments { get; set; } = null;
}
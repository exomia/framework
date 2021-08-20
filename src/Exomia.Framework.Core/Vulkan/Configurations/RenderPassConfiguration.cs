#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Vulkan.Api.Core;

#pragma warning disable 1591
namespace Exomia.Framework.Core.Vulkan.Configurations
{
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
        public VkSampleCountFlagBits           Samples        { get; set; } = VkSampleCountFlagBits._1_BIT;
        public VkAttachmentLoadOp              LoadOp         { get; set; } = VkAttachmentLoadOp.CLEAR;
        public VkAttachmentStoreOp             StoreOp        { get; set; } = VkAttachmentStoreOp.STORE;
        public VkAttachmentLoadOp              StencilLoadOp  { get; set; } = VkAttachmentLoadOp.DONT_CARE;
        public VkAttachmentStoreOp             StencilStoreOp { get; set; } = VkAttachmentStoreOp.DONT_CARE;
        public VkImageLayout                   InitialLayout  { get; set; } = VkImageLayout.UNDEFINED;
        public VkImageLayout                   FinalLayout    { get; set; } = VkImageLayout.PRESENT_SRC_KHR;
    }

    /// <summary> An attachment reference configuration. This class cannot be inherited. </summary>
    public sealed class AttachmentReferenceConfiguration
    {
        public VkImageLayout         Layout     { get; set; } = VkImageLayout.COLOR_ATTACHMENT_OPTIMAL;
        public VkImageAspectFlagBits AspectMask { get; set; } = VkImageAspectFlagBits.COLOR_BIT;
    }

    /// <summary> A subpass configuration. This class cannot be inherited. </summary>
    public sealed class SubpassConfiguration
    {
        public VkSubpassDescriptionFlagBits Flags             { get; set; } = 0;
        public VkPipelineBindPoint          PipelineBindPoint { get; set; } = VkPipelineBindPoint.GRAPHICS;
        public uint                         ViewMask          { get; set; } = 0u;

        public unsafe delegate*<
            VkContext*,              /* context */
            uint*,                   /* inputAttachmentCount */
            VkAttachmentReference2*, /* pInputAttachments */
            void> CreateAdditionalInputAttachments
        {
            get;
            set;
        } = null;
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion


using Exomia.Framework.Core.Vulkan.Configurations;
using static Exomia.Vulkan.Api.Core.VkSampleCountFlagBits;
using static Exomia.Vulkan.Api.Core.VkAttachmentLoadOp;
using static Exomia.Vulkan.Api.Core.VkAttachmentStoreOp;
using static Exomia.Vulkan.Api.Core.VkImageLayout;
using static Exomia.Vulkan.Api.Core.VkImageAspectFlagBits;
using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;
using static Exomia.Vulkan.Api.Core.VkAccessFlagBits;
using static Exomia.Vulkan.Api.Core.VkDependencyFlagBits;

namespace Exomia.Framework.Core.Vulkan;

sealed unsafe partial class Vulkan
{
    private static bool CreateRenderPass(
        VkContext*              context,
        RenderPassConfiguration configuration)
    {
        VkAttachmentDescription2* pAttachmentDescription2 = stackalloc VkAttachmentDescription2[2];
        (pAttachmentDescription2 + 0)->sType          = VkAttachmentDescription2.STYPE;
        (pAttachmentDescription2 + 0)->pNext          = null;
        (pAttachmentDescription2 + 0)->flags          = configuration.Attachment.Flags;
        (pAttachmentDescription2 + 0)->format         = context->Format;
        (pAttachmentDescription2 + 0)->samples        = configuration.Attachment.Samples;
        (pAttachmentDescription2 + 0)->loadOp         = configuration.Attachment.LoadOp;
        (pAttachmentDescription2 + 0)->storeOp        = configuration.Attachment.StoreOp;
        (pAttachmentDescription2 + 0)->stencilLoadOp  = configuration.Attachment.StencilLoadOp;
        (pAttachmentDescription2 + 0)->stencilStoreOp = configuration.Attachment.StencilStoreOp;
        (pAttachmentDescription2 + 0)->initialLayout  = configuration.Attachment.InitialLayout;
        (pAttachmentDescription2 + 0)->finalLayout    = configuration.Attachment.FinalLayout;

        (pAttachmentDescription2 + 1)->sType          = VkAttachmentDescription2.STYPE;
        (pAttachmentDescription2 + 1)->pNext          = null;
        (pAttachmentDescription2 + 1)->flags          = 0;
        (pAttachmentDescription2 + 1)->format         = context->DepthStencilFormat;
        (pAttachmentDescription2 + 1)->samples        = VK_SAMPLE_COUNT_1_BIT;
        (pAttachmentDescription2 + 1)->loadOp         = VK_ATTACHMENT_LOAD_OP_CLEAR;
        (pAttachmentDescription2 + 1)->storeOp        = VK_ATTACHMENT_STORE_OP_DONT_CARE;
        (pAttachmentDescription2 + 1)->stencilLoadOp  = VK_ATTACHMENT_LOAD_OP_CLEAR;
        (pAttachmentDescription2 + 1)->stencilStoreOp = VK_ATTACHMENT_STORE_OP_DONT_CARE;
        (pAttachmentDescription2 + 1)->initialLayout  = VK_IMAGE_LAYOUT_UNDEFINED;
        (pAttachmentDescription2 + 1)->finalLayout    = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;

        VkAttachmentReference2 colorAttachmentReference2;
        colorAttachmentReference2.sType      = VkAttachmentReference2.STYPE;
        colorAttachmentReference2.pNext      = null;
        colorAttachmentReference2.attachment = 0u;
        colorAttachmentReference2.layout     = configuration.AttachmentReference.Layout;
        colorAttachmentReference2.aspectMask = configuration.AttachmentReference.AspectMask;

        VkAttachmentReference2 depthStencilAttachmentReference2;
        depthStencilAttachmentReference2.sType      = VkAttachmentReference2.STYPE;
        depthStencilAttachmentReference2.pNext      = null;
        depthStencilAttachmentReference2.attachment = 1u;
        depthStencilAttachmentReference2.layout     = VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL;
        depthStencilAttachmentReference2.aspectMask = VK_IMAGE_ASPECT_DEPTH_BIT | VK_IMAGE_ASPECT_STENCIL_BIT;

        uint additionalSubpassCount = 0u;
        if (configuration.CreateAdditionalSubpasses != null)
        {
            configuration.CreateAdditionalSubpasses(context, &additionalSubpassCount, null);
        }

        VkSubpassDescription2* pSubpasses = stackalloc VkSubpassDescription2[(int)(1u + additionalSubpassCount)];
        pSubpasses->sType                = VkSubpassDescription2.STYPE;
        pSubpasses->pNext                = null;
        pSubpasses->flags                = configuration.Subpass.Flags;
        pSubpasses->pipelineBindPoint    = configuration.Subpass.PipelineBindPoint;
        pSubpasses->viewMask             = configuration.Subpass.ViewMask;
        pSubpasses->inputAttachmentCount = 0u;
        pSubpasses->pInputAttachments    = null;
        if (configuration.Subpass.CreateAdditionalInputAttachments != null)
        {
            configuration.Subpass.CreateAdditionalInputAttachments(context, &pSubpasses->inputAttachmentCount, null);
            VkAttachmentReference2* pInputAttachments = stackalloc VkAttachmentReference2[(int)pSubpasses->inputAttachmentCount];
            configuration.Subpass.CreateAdditionalInputAttachments(context, &pSubpasses->inputAttachmentCount, pInputAttachments);
            pSubpasses->pInputAttachments = pInputAttachments;
        }
        pSubpasses->colorAttachmentCount    = 1u;
        pSubpasses->pColorAttachments       = &colorAttachmentReference2;
        pSubpasses->pResolveAttachments     = null;
        pSubpasses->pDepthStencilAttachment = &depthStencilAttachmentReference2;
        pSubpasses->preserveAttachmentCount = 0u;
        pSubpasses->pPreserveAttachments    = null;

        if (configuration.CreateAdditionalSubpasses != null)
        {
            configuration.CreateAdditionalSubpasses(context, &additionalSubpassCount, pSubpasses + 1);
        }

        uint additionalDependencyCount = 0u;
        if (configuration.CreateAdditionalDependencies != null)
        {
            configuration.CreateAdditionalDependencies(context, &additionalDependencyCount, null);
        }

        VkSubpassDependency2* pDependencies = stackalloc VkSubpassDependency2[(int)(1u + additionalDependencyCount)];
        (pDependencies + 0)->sType           = VkSubpassDependency2.STYPE;
        (pDependencies + 0)->pNext           = null;
        (pDependencies + 0)->srcSubpass      = VK_SUBPASS_EXTERNAL;
        (pDependencies + 0)->dstSubpass      = 0u;
        (pDependencies + 0)->srcStageMask    = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
        (pDependencies + 0)->dstStageMask    = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
        (pDependencies + 0)->srcAccessMask   = 0;
        (pDependencies + 0)->dstAccessMask   = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT | VK_ACCESS_COLOR_ATTACHMENT_READ_BIT | VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;
        (pDependencies + 0)->dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;
        (pDependencies + 0)->viewOffset      = 0;

        if (configuration.CreateAdditionalDependencies != null)
        {
            configuration.CreateAdditionalDependencies(context, &additionalDependencyCount, pDependencies + 1);
        }

        VkRenderPassCreateInfo2 renderPassCreateInfo2;
        renderPassCreateInfo2.sType                   = VkRenderPassCreateInfo2.STYPE;
        renderPassCreateInfo2.pNext                   = null;
        renderPassCreateInfo2.flags                   = configuration.Flags;
        renderPassCreateInfo2.attachmentCount         = 2u;
        renderPassCreateInfo2.pAttachments            = pAttachmentDescription2;
        renderPassCreateInfo2.subpassCount            = 1u + additionalSubpassCount;
        renderPassCreateInfo2.pSubpasses              = pSubpasses;
        renderPassCreateInfo2.dependencyCount         = 1u + additionalDependencyCount;
        renderPassCreateInfo2.pDependencies           = pDependencies;
        renderPassCreateInfo2.correlatedViewMaskCount = 0u;
        renderPassCreateInfo2.pCorrelatedViewMasks    = null;

        vkCreateRenderPass2(context->Device, &renderPassCreateInfo2, null, &context->RenderPass)
            .AssertVkResult();

        return true;
    }
}
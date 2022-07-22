#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Vulkan.Configurations;
using static Exomia.Vulkan.Api.Core.VkPipelineStageFlagBits;
using static Exomia.Vulkan.Api.Core.VkAccessFlagBits;
using static Exomia.Vulkan.Api.Core.VkDependencyFlagBits;

namespace Exomia.Framework.Core.Vulkan;

/// <summary> A render pass. This class cannot be inherited. </summary>
public sealed unsafe class RenderPass : IDisposable
{
    private readonly VkContext*   _vkContext;
    private          VkRenderPass _renderPass;

    private RenderPass(VkContext* vkContext, VkRenderPass renderPass)
    {
        _vkContext  = vkContext;
        _renderPass = renderPass;
    }

    /// <summary> Implicit cast that converts the given <see cref="RenderPass" /> to a <see cref="VkRenderPass" />. </summary>
    /// <param name="renderPass"> The render pass. </param>
    /// <returns> The result of the operation. </returns>
    public static implicit operator VkRenderPass(RenderPass renderPass)
    {
        return renderPass._renderPass;
    }

    /// <summary> Creates a new <see cref="RenderPass" />. </summary>
    /// <param name="vkContext">     [in,out] If non-null, the vk context. </param>
    /// <param name="context">       [in,out] If non-null, the swapchain context. </param>
    /// <param name="configuration"> The configuration. </param>
    /// <returns> The <see cref="RenderPass" />. </returns>
    /// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or illegal values. </exception>
    public static RenderPass Create(
        VkContext*              vkContext,
        SwapchainContext*       context,
        RenderPassConfiguration configuration)
    {
        if (configuration.ColorAttachments.Length <= 0)
        {
            throw new ArgumentException("The color attachments must have at least 1 entry.", nameof(configuration));
        }

        VkAttachmentDescription2* pAttachmentDescription2
            = stackalloc VkAttachmentDescription2[configuration.InputAttachments.Length + configuration.ColorAttachments.Length + 1];

        void AddAttachments(
            ref uint                                                      index,
            (AttachmentConfiguration, AttachmentReferenceConfiguration)[] attachments,
            VkAttachmentReference2*                                       pAttachmentReference2,
            VkFormat                                                      format)
        {
            for (uint i = 0u; i < attachments.Length; i++, index++)
            {
                (AttachmentConfiguration attachmentConfiguration, AttachmentReferenceConfiguration attachmentReferenceConfiguration)
                    = attachments[i];

                // VkAttachmentDescription2
                (pAttachmentDescription2 + index)->sType          = VkAttachmentDescription2.STYPE;
                (pAttachmentDescription2 + index)->pNext          = attachmentConfiguration.Next;
                (pAttachmentDescription2 + index)->flags          = attachmentConfiguration.Flags;
                (pAttachmentDescription2 + index)->format         = format;
                (pAttachmentDescription2 + index)->samples        = attachmentConfiguration.Samples;
                (pAttachmentDescription2 + index)->loadOp         = attachmentConfiguration.LoadOp;
                (pAttachmentDescription2 + index)->storeOp        = attachmentConfiguration.StoreOp;
                (pAttachmentDescription2 + index)->stencilLoadOp  = attachmentConfiguration.StencilLoadOp;
                (pAttachmentDescription2 + index)->stencilStoreOp = attachmentConfiguration.StencilStoreOp;
                (pAttachmentDescription2 + index)->initialLayout  = attachmentConfiguration.InitialLayout;
                (pAttachmentDescription2 + index)->finalLayout    = attachmentConfiguration.FinalLayout;

                // VkAttachmentReference2
                (pAttachmentReference2 + i)->sType      = VkAttachmentReference2.STYPE;
                (pAttachmentReference2 + i)->pNext      = attachmentReferenceConfiguration.Next;
                (pAttachmentReference2 + i)->attachment = index;
                (pAttachmentReference2 + i)->layout     = attachmentReferenceConfiguration.Layout;
                (pAttachmentReference2 + i)->aspectMask = attachmentReferenceConfiguration.AspectMask;
            }
        }

        uint attachmentCount = 0u;

        VkAttachmentReference2* pInputAttachmentReference2 = stackalloc VkAttachmentReference2[configuration.InputAttachments.Length];
        AddAttachments(ref attachmentCount, configuration.InputAttachments, pInputAttachmentReference2, context->Format);

        VkAttachmentReference2* pColorAttachmentReference2 = stackalloc VkAttachmentReference2[configuration.ColorAttachments.Length];
        AddAttachments(ref attachmentCount, configuration.ColorAttachments, pColorAttachmentReference2, context->Format);

        VkAttachmentReference2* pDepthStencilAttachmentReference2 = stackalloc VkAttachmentReference2[1];
        AddAttachments(ref attachmentCount, new[] { configuration.DepthStencilAttachment }, pDepthStencilAttachmentReference2, context->DepthStencilFormat);

        uint additionalSubpassCount = 0u;
        if (configuration.CreateAdditionalSubpasses != null)
        {
            configuration.CreateAdditionalSubpasses(context, &additionalSubpassCount, null);
        }

        VkSubpassDescription2* pSubpasses = stackalloc VkSubpassDescription2[(int)(1u + additionalSubpassCount)];
        pSubpasses->sType                   = VkSubpassDescription2.STYPE;
        pSubpasses->pNext                   = configuration.Subpass.Next;
        pSubpasses->flags                   = configuration.Subpass.Flags;
        pSubpasses->pipelineBindPoint       = configuration.Subpass.PipelineBindPoint;
        pSubpasses->viewMask                = configuration.Subpass.ViewMask;
        pSubpasses->inputAttachmentCount    = (uint)configuration.InputAttachments.Length;
        pSubpasses->pInputAttachments       = pInputAttachmentReference2;
        pSubpasses->colorAttachmentCount    = (uint)configuration.ColorAttachments.Length;
        pSubpasses->pColorAttachments       = pColorAttachmentReference2;
        pSubpasses->pResolveAttachments     = null;
        pSubpasses->pDepthStencilAttachment = pDepthStencilAttachmentReference2;
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
        pDependencies->sType           = VkSubpassDependency2.STYPE;
        pDependencies->pNext           = null;
        pDependencies->srcSubpass      = VK_SUBPASS_EXTERNAL;
        pDependencies->dstSubpass      = 0u;
        pDependencies->srcStageMask    = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
        pDependencies->dstStageMask    = VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT;
        pDependencies->srcAccessMask   = 0;
        pDependencies->dstAccessMask   = VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT | VK_ACCESS_COLOR_ATTACHMENT_READ_BIT | VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT;
        pDependencies->dependencyFlags = VK_DEPENDENCY_BY_REGION_BIT;
        pDependencies->viewOffset      = 0;

        if (configuration.CreateAdditionalDependencies != null)
        {
            configuration.CreateAdditionalDependencies(context, &additionalDependencyCount, pDependencies + 1);
        }

        VkRenderPassCreateInfo2 renderPassCreateInfo2;
        renderPassCreateInfo2.sType                   = VkRenderPassCreateInfo2.STYPE;
        renderPassCreateInfo2.pNext                   = null;
        renderPassCreateInfo2.flags                   = configuration.Flags;
        renderPassCreateInfo2.attachmentCount         = attachmentCount;
        renderPassCreateInfo2.pAttachments            = pAttachmentDescription2;
        renderPassCreateInfo2.subpassCount            = 1u + additionalSubpassCount;
        renderPassCreateInfo2.pSubpasses              = pSubpasses;
        renderPassCreateInfo2.dependencyCount         = 1u + additionalDependencyCount;
        renderPassCreateInfo2.pDependencies           = pDependencies;
        renderPassCreateInfo2.correlatedViewMaskCount = 0u;
        renderPassCreateInfo2.pCorrelatedViewMasks    = null;

        VkRenderPass renderPass;
        vkCreateRenderPass2(vkContext->Device, &renderPassCreateInfo2, null, &renderPass)
            .AssertVkResult();

        return new RenderPass(vkContext, renderPass);
    }

    #region IDisposable Support

    private bool _disposed;

    /// <summary> Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged/managed resources. </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            if (_vkContext->Device != VkDevice.Null && _renderPass != VkRenderPass.Null)
            {
                vkDeviceWaitIdle(_vkContext->Device)
                    .AssertVkResult();

                vkDestroyRenderPass(_vkContext->Device, _renderPass, null);
                _renderPass = VkRenderPass.Null;
            }
        }
        GC.SuppressFinalize(this);
    }

    /// <summary> Finalizes an instance of the <see cref="RenderPass" /> class. </summary>
    ~RenderPass()
    {
        Dispose();
    }

    #endregion
}
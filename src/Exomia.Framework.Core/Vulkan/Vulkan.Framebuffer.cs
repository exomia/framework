#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.Core.Allocators;

namespace Exomia.Framework.Core.Vulkan
{
    sealed unsafe partial class Vulkan
    {
        private static bool CreateFrameBuffers(VkContext* context)
        {
            if (context->SwapchainImageCount == 0u ||
                context->DepthStencilImageView == VkImageView.Null)
            {
                return false;
            }

            context->Framebuffers = Allocator.Allocate<VkFramebuffer>(context->SwapchainImageCount);

            VkImageView* pImageViews = stackalloc VkImageView[2];
            *(pImageViews + 1) = context->DepthStencilImageView;

            for (uint i = 0u; i < context->SwapchainImageCount; i++)
            {
                *pImageViews = *(context->SwapchainImageViews + i);

                VkFramebufferCreateInfo framebufferCreateInfo;
                framebufferCreateInfo.sType           = VkFramebufferCreateInfo.STYPE;
                framebufferCreateInfo.pNext           = null;
                framebufferCreateInfo.flags           = 0;
                framebufferCreateInfo.renderPass      = context->RenderPass;
                framebufferCreateInfo.attachmentCount = 2u;
                framebufferCreateInfo.pAttachments    = pImageViews;
                framebufferCreateInfo.width           = context->Width;
                framebufferCreateInfo.height          = context->Height;
                framebufferCreateInfo.layers          = 1u;

                vkCreateFramebuffer(context->Device, &framebufferCreateInfo, null, context->Framebuffers + i)
                    .AssertVkResult();
            }

            return true;
        }
    }
}
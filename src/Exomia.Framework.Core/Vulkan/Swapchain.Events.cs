namespace Exomia.Framework.Core.Vulkan;

public sealed unsafe partial class Swapchain
{
    /// <summary> Delegate for handling swapchain events. </summary>
    /// <param name="swapchain"> The swapchain. </param>
    /// <param name="vkContext"> [in,out] If non-null, the vk context. </param>
    public delegate void SwapchainEventHandler(Swapchain swapchain, VkContext* vkContext);

    /// <summary> Occurs when the swapchain was recreated. </summary>
    public event SwapchainEventHandler? SwapChainRecreated;

    /// <summary> Occurs when the swapchain is cleaned up. </summary>
    public event SwapchainEventHandler? CleanupSwapChain;
}
namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to initialize a game component.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        ///     This method is called when the component is added to the game.
        /// </summary>
        /// <remarks>
        ///     This method can be used for tasks like querying for services the component needs and setting up non-graphics
        ///     resources.
        /// </remarks>
        void Initialize(IServiceRegistry registry);
    }
}
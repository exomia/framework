namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to define a game component.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        ///     the name of the component
        /// </summary>
        string Name { get; }
    }
}
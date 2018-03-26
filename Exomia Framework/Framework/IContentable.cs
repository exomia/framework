namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to load and unload content.
    /// </summary>
    public interface IContentable
    {
        /// <summary>
        ///     Loads the content.
        /// </summary>
        void LoadContent();

        /// <summary>
        ///     Called when graphics resources need to be unloaded.
        /// </summary>
        void UnloadContent();
    }
}
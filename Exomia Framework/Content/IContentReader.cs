namespace Exomia.Framework.Content
{
    /// <summary>
    ///     An interface to read content
    /// </summary>
    public interface IContentReader
    {
        /// <summary>
        ///     Reads the content of a particular data from a stream.
        /// </summary>
        /// <param name="contentManager">The content manager.</param>
        /// <param name="parameters"></param>
        /// <returns>The data decoded from the stream, or null if the kind of asset is not supported by this content reader.</returns>
        object ReadContent(IContentManager contentManager, ref ContentReaderParameters parameters);
    }
}
namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     An interface to handle content reading
    /// </summary>
    public interface IContentSerializationReader
    {
        /// <summary>
        ///     Returns a new created object from the context
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>new  object</returns>
        object Read(ContentSerializationContext context);
    }
}
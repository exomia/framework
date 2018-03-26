namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     An interface to handle content writing
    /// </summary>
    public interface IContentSerializationWriter
    {
        /// <summary>
        ///     Write the object informations into the context
        /// </summary>
        /// <param name="context">ref Context</param>
        /// <param name="obj">Object</param>
        void Write(ContentSerializationContext context, object obj);
    }
}
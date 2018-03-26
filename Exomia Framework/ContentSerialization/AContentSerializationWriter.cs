namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     abstract implementation for an <see cref="IContentSerializationWriter" />
    /// </summary>
    /// <typeparam name="T">type to write</typeparam>
    public abstract class AContentSerializationWriter<T> : IContentSerializationWriter
    {
        /// <summary>
        ///     <see cref="IContentSerializationWriter" />
        /// </summary>
        public void Write(ContentSerializationContext context, object obj)
        {
            WriteContext(context, (T)obj);
        }

        /// <summary>
        ///     Write the object (of type T) informations into the context
        /// </summary>
        /// <param name="context">ref Context</param>
        /// <param name="obj">Object</param>
        public abstract void WriteContext(ContentSerializationContext context, T obj);
    }
}
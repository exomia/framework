namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     abstract implementation for an <see cref="IContentSerializationReader" />
    /// </summary>
    /// <typeparam name="T">type to read</typeparam>
    public abstract class AContentSerializationReader<T> : IContentSerializationReader
    {
        /// <summary>
        ///     <see cref="IContentSerializationReader" />
        /// </summary>
        public object Read(ContentSerializationContext context)
        {
            return ReadContext(context);
        }

        /// <summary>
        ///     Returns a new created object from the context of type T
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>new T</returns>
        public abstract T ReadContext(ContentSerializationContext context);
    }
}
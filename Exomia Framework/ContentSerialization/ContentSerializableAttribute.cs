using System;

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     used to mark a content serializable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ContentSerializableAttribute : Attribute
    {
        /// <summary>
        ///     ContentSerializableAttribute constructor
        /// </summary>
        /// <param name="reader">the content reader type <see cref="IContentSerializationReader" /></param>
        /// <param name="writer">the content writer type <see cref="IContentSerializationWriter" /></param>
        public ContentSerializableAttribute(Type reader, Type writer)
        {
            Reader = Activator.CreateInstance(reader) as IContentSerializationReader ??
                     throw new TypeLoadException(
                         "cannot create an instance of IContentSerializationReader from type: " +
                         reader.AssemblyQualifiedName);
            Writer = Activator.CreateInstance(writer) as IContentSerializationWriter ??
                     throw new TypeLoadException(
                         "cannot create an instance of IContentSerializationWriter from type: " +
                         writer.AssemblyQualifiedName);
        }

        internal IContentSerializationReader Reader { get; }

        internal IContentSerializationWriter Writer { get; }
    }
}
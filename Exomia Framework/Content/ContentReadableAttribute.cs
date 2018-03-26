using System;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     used to mark a content readable class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class ContentReadableAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentReadableAttribute" /> class.
        /// </summary>
        /// <param name="reader">the content reader type <see cref="IContentReader" /></param>
        public ContentReadableAttribute(Type reader)
        {
            Reader = Activator.CreateInstance(reader) as IContentReader ??
                     throw new TypeLoadException(
                         "cannot create an instance of IContentReader from type: " + reader.AssemblyQualifiedName);
        }

        internal IContentReader Reader { get; }
    }
}
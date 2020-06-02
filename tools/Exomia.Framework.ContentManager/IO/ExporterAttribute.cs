using System;

namespace Exomia.Framework.ContentManager.IO
{
    /// <summary>
    ///     Attribute for exporter. This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExporterAttribute : Attribute
    {
        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <inheritdoc />
        public ExporterAttribute(string name)
        {
            Name = name;
        }
    }
}

using System;
using System.IO;

namespace Exomia.Framework.ContentManager.IO
{
    abstract class Importer<T> : IImporter
        where T : class
    {
        /// <inheritdoc />
        public Type OutType
        {
            get { return typeof(T); }
        }
        
        object? IImporter.Import(Stream stream, ImporterContext context)
        {
            return Import(stream, context);
        }

        public abstract T? Import(Stream stream, ImporterContext context);
    }
}
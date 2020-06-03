using System;

namespace Exomia.Framework.ContentManager.IO
{
    abstract class Exporter<T> : IExporter
    {
        /// <inheritdoc />
        public Type ImportType
        {
            get { return typeof(T); }
        }
        
        bool IExporter.Export(object obj, ExporterContext context)
        {
            return Export((T)obj, context);
        }

        public abstract bool Export(T obj, ExporterContext context);
    }
}

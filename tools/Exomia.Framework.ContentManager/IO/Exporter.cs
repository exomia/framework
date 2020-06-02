namespace Exomia.Framework.ContentManager.IO
{
    abstract class Exporter<T> : IExporter
    {
        bool IExporter.Export(object obj, ExporterContext context)
        {
            return Export((T)obj, context);
        }

        public abstract bool Export(T obj, ExporterContext context);
    }
}

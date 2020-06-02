namespace Exomia.Framework.ContentManager.IO
{
    abstract class Importer<T> : IImporter
        where T : class
    {
        object? IImporter.Import(object item, ImporterContext context)
        {
            return Import(item, context);
        }

        public abstract T? Import(object item, ImporterContext context);
    }
}
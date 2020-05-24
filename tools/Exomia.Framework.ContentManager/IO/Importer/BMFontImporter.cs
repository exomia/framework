// ReSharper disable InconsistentNaming

namespace Exomia.Framework.ContentManager.IO.Importer
{
    /// <summary>
    ///     A bm font importer. This class cannot be inherited.
    /// </summary>
    sealed class BMFontImporter : IImporter
    {
        public static BMFontImporter Default = new BMFontImporter();
        private BMFontImporter() { }
    }
}
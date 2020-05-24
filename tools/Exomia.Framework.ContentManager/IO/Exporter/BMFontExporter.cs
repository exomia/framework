// ReSharper disable InconsistentNaming

namespace Exomia.Framework.ContentManager.IO.Exporter
{
    /// <summary>
    ///     A bm font exporter. This class cannot be inherited.
    /// </summary>
    sealed class BMFontExporter : IExporter
    {
        public static BMFontExporter Default = new BMFontExporter();

        private BMFontExporter() { }
    }
}
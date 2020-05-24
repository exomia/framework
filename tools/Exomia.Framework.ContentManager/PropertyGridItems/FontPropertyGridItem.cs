using System;
using Exomia.Framework.ContentManager.IO.Exporter;
using Exomia.Framework.ContentManager.IO.Importer;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A font property grid item. This class cannot be inherited.
    /// </summary>
    sealed class FontPropertyGridItem : ItemPropertyGridItem
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        public FontPropertyGridItem(Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider)
            : base(nameProvider, virtualPathProvider, BMFontImporter.Default, BMFontExporter.Default) { }
    }
}
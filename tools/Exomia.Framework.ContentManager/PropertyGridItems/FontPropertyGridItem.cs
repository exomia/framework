#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using Exomia.Framework.ContentManager.Fonts;
using Exomia.Framework.ContentManager.IO;
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
        /// <param name="fontDescription">     The font description. </param>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        public FontPropertyGridItem(FontDescription        fontDescription,
                                    Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider)
            : base(
                fontDescription, nameProvider, virtualPathProvider,
                new IImporter[] { BMFontImporter.Default },
                new IExporter[] { SpiteFontExporter.Default })
        {
            Importer = BMFontImporter.Default;
            Exporter = SpiteFontExporter.Default;
        }
    }
}
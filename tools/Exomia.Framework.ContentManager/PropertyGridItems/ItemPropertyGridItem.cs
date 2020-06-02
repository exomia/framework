#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using Exomia.Framework.ContentManager.Converters;
using Exomia.Framework.ContentManager.IO;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A font property grid item. This class cannot be inherited.
    /// </summary>
    class ItemPropertyGridItem : PropertyGridItem
    {
        [Browsable(false)]
        public object Item { get; }

        /// <summary>
        ///     Gets the importers.
        /// </summary>
        /// <value>
        ///     The importers.
        /// </value>
        [Browsable(false)]
        public IImporter[] Importers { get; }

        /// <summary>
        ///     Gets the exporters.
        /// </summary>
        /// <value>
        ///     The exporters.
        /// </value>
        [Browsable(false)]
        public IExporter[] Exporters { get; }

        /// <summary>
        ///     The importer for this item.
        /// </summary>
        /// <value>
        ///     The importer.
        /// </value>
        [Category("Settings")]
        [Description("The importer for this item.")]
        [TypeConverter(typeof(ItemExporterImporterConverter))]
        public IImporter? Importer { get; set; }

        /// <summary>
        ///     The exporter for this item.
        /// </summary>
        /// <value>
        ///     The exporter.
        /// </value>
        [Category("Settings")]
        [Description("The exporter for this item.")]
        [TypeConverter(typeof(ItemExporterImporterConverter))]
        public IExporter? Exporter { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="item">                The item. </param>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        /// <param name="importers">           The importers. </param>
        /// <param name="exporters">           The exporters. </param>
        public ItemPropertyGridItem(object item, 
                                    Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider,
                                    IImporter[]            importers,
                                    IExporter[]            exporters)
            : base(nameProvider, virtualPathProvider)
        {
            Item = item;
            Importers = importers;
            Exporters = exporters;
        }
    }
}
﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Exomia.Framework.ContentManager.Converters;
using Exomia.Framework.ContentManager.IO;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A font property grid item. This class cannot be inherited.
    /// </summary>
    class ItemPropertyGridItem : PropertyGridItem
    {
        private IImporter? _importer;
        private IExporter? _exporter;

        /// <summary>
        ///     Gets the importers.
        /// </summary>
        /// <value>
        ///     The importers.
        /// </value>
        [Browsable(false)]
        public List<IImporter>? Importers { get; }

        /// <summary>
        ///     Gets the exporters.
        /// </summary>
        /// <value>
        ///     The exporters.
        /// </value>
        [Browsable(false)]
        public List<IExporter>? Exporters { get; private set; }

        /// <summary>
        ///     The importer for this item.
        /// </summary>
        /// <value>
        ///     The importer.
        /// </value>
        [Category("Settings")]
        [Description("The build action for this item.")]
        [DisplayName("Build Action")]
        public BuildAction BuildAction { get; set; } = BuildAction.Build;

        /// <summary>
        ///     The importer for this item.
        /// </summary>
        /// <value>
        ///     The importer.
        /// </value>
        [Category("Settings")]
        [Description("The importer for this item.")]
        [TypeConverter(typeof(ItemExporterImporterConverter))]
        public IImporter? Importer
        {
            get { return _importer; }
            set
            {
                _importer = value;
                if (_importer != null)
                {
                    if (_exporter == null || _exporter.ImportType != _importer.OutType)
                    {
                        Exporters = ImporterExporterManager.GetExportersFor(_importer.OutType);
                        if (Exporters.Count > 0)
                        {
                            Exporter = Exporters[0];
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     The exporter for this item.
        /// </summary>
        /// <value>
        ///     The exporter.
        /// </value>
        [Category("Settings")]
        [Description("The exporter for this item.")]
        [TypeConverter(typeof(ItemExporterImporterConverter))]
        public IExporter? Exporter
        {
            get { return _exporter; }
            set { _exporter = value; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        public ItemPropertyGridItem(Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider)
            : base(nameProvider, virtualPathProvider)
        {
            Importers = ImporterExporterManager.GetImporterFor(Path.GetExtension(nameProvider()));
            if (Importers.Count > 0)
            {
                Importer = Importers[0];
            }
        }
    }
}
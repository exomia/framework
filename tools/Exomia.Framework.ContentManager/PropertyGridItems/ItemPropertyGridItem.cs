#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using Exomia.Framework.ContentManager.IO;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A font property grid item. This class cannot be inherited.
    /// </summary>
    class ItemPropertyGridItem : PropertyGridItem
    {
        /// <summary>
        ///     The importer for this item.
        /// </summary>
        /// <value>
        ///     The importer.
        /// </value>
        [Category("Settings"), Description("The importer for this item.")]
        public IImporter Importer { get; set; }

        /// <summary>
        ///     The exporter for this item.
        /// </summary>
        /// <value>
        ///     The exporter.
        /// </value>
        [Category("Settings"), Description("The exporter for this item.")]
        public IExporter Exporter { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        /// <param name="importer">            The importer. </param>
        /// <param name="exporter">            The exporter. </param>
        public ItemPropertyGridItem(Provider.Value<string> nameProvider,
                                    Provider.Value<string> virtualPathProvider,
                                    IImporter    importer,
                                    IExporter    exporter)
            : base(nameProvider, virtualPathProvider)
        {
            Importer = importer;
            Exporter = exporter;
        }
    }
}
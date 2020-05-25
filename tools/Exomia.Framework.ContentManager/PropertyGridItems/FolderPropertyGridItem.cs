#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A folder property grid item.
    /// </summary>
    class FolderPropertyGridItem : PropertyGridItem
    {
        private readonly Provider.Value<int> _totalItemsProvider;

        /// <summary>
        ///     The total item count of this project.
        /// </summary>
        /// <value>
        ///     The total number of items.
        /// </value>
        [Category("Statistics")]
        [Description("The total item count of this project.")]
        [ReadOnly(true)]
        public int TotalItems
        {
            get { return _totalItemsProvider(); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        /// <param name="totalItemsProvider">  The total items provider. </param>
        public FolderPropertyGridItem(Provider.Value<string> nameProvider,
                                      Provider.Value<string> virtualPathProvider,
                                      Provider.Value<int>    totalItemsProvider)
            : base(nameProvider, virtualPathProvider)
        {
            _totalItemsProvider = totalItemsProvider;
        }
    }
}
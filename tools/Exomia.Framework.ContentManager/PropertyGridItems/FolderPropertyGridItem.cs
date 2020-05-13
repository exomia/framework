#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A folder property grid item.
    /// </summary>
    class FolderPropertyGridItem : PropertyGridItem
    {
        private readonly Func<string> _virtualPathProvider;
        private readonly Func<int>    _totalItemsProvider;

        /// <summary>
        ///     The virtual path to this item.
        /// </summary>
        /// <value>
        ///     The full pathname of the virtual file.
        /// </value>
        [Category("Common")]
        [Description("The virtual path to this item.")]
        [ReadOnly(true)]
        public string VirtualPath
        {
            get { return _virtualPathProvider(); }
        }

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
        public FolderPropertyGridItem(Func<string> nameProvider,
                                      Func<string> virtualPathProvider,
                                      Func<int>    totalItemsProvider)
            : base(nameProvider)
        {
            _virtualPathProvider = virtualPathProvider;
            _totalItemsProvider  = totalItemsProvider;
        }
    }
}
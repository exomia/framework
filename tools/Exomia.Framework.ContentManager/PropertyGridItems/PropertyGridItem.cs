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
    ///     A property grid item.
    /// </summary>
    [DefaultProperty("Name")]
    class PropertyGridItem
    {
        private readonly Provider.Value<string> _nameProvider;
        private readonly Provider.Value<string> _virtualPathProvider;

        /// <summary>
        ///     The name of this project.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Category("Common")]
        [Description("The name of this project.")]
        [ReadOnly(true)]
        public string Name
        {
            get { return _nameProvider(); }
        }

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
        ///     Initializes a new instance of the <see cref="PropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        public PropertyGridItem(Provider.Value<string> nameProvider, Provider.Value<string> virtualPathProvider)
        {
            _nameProvider        = nameProvider;
            _virtualPathProvider = virtualPathProvider;
        }
    }
}
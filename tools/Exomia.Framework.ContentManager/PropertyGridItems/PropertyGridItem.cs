﻿using System;
using System.ComponentModel;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A property grid item.
    /// </summary>
    [DefaultProperty("Name")]
    class PropertyGridItem
    {
        private readonly Func<string> _nameProvider;

        /// <summary>
        ///     The name of this project.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Category("Common"), Description("The name of this project."), ReadOnly(true)]
        public string Name
        {
            get { return _nameProvider(); }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyGridItem"/> class.
        /// </summary>
        /// <param name="nameProvider"> The name provider. </param>
        public PropertyGridItem(Func<string> nameProvider)
        {
            _nameProvider = nameProvider;
        }
    }
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using Exomia.Framework.ContentManager.Attributes;
using Exomia.Framework.ContentManager.Converters;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A content property grid item. This class cannot be inherited.
    /// </summary>
    sealed class ContentPropertyGridItem : FolderPropertyGridItem
    {
        private readonly Provider.Value<string> _projectLocationProvider;

        /// <summary>
        ///     The location of the project.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        [Category("Common")]
        [Description("The location of the project.")]
        [ReadOnly(true)]
        public string ProjectLocation
        {
            get { return _projectLocationProvider(); }
        }

        /// <summary>
        ///     The build output folder.
        /// </summary>
        /// <value>
        ///     The pathname of the output folder.
        /// </value>
        [Category("Settings")]
        [Description("The build output folder.")]
        public string? OutputFolder { get; set; }

        /// <summary>
        ///     TODO: REMOVE.
        /// </summary>
        /// <value>
        ///     The test.
        /// </value>
        [Category("test")]
        [Description("...")]
        [ReadOnly(false)]
        [TypeConverter(typeof(ChoicesStringConverter))]
        [Choices("a", "b", "c")]
        public string? Test { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">            The name provider. </param>
        /// <param name="virtualPathProvider">     The virtual path provider. </param>
        /// <param name="totalItemsProvider">      The total items provider. </param>
        /// <param name="projectLocationProvider"> The project location provider. </param>
        public ContentPropertyGridItem(Provider.Value<string> nameProvider,
                                       Provider.Value<string> virtualPathProvider,
                                       Provider.Value<int>    totalItemsProvider,
                                       Provider.Value<string> projectLocationProvider)
            : base(nameProvider, virtualPathProvider, totalItemsProvider)
        {
            _projectLocationProvider = projectLocationProvider;
        }
    }
}
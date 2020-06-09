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
    ///     A content property grid item. This class cannot be inherited.
    /// </summary>
    sealed class ContentPropertyGridItem : FolderPropertyGridItem
    {
        /// <summary>
        ///     The name of the project.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        [Category("Common")]
        [Description("The name of the project.")]
        [ReadOnly(true)]
        public string ProjectName { get; }

        /// <summary>
        ///     The location of the project.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        [Category("Common")]
        [Description("The location of the project.")]
        [ReadOnly(true)]
        public string ProjectLocation { get; }

        /// <summary>
        ///     The build output folder.
        /// </summary>
        /// <value>
        ///     The pathname of the output folder.
        /// </value>
        [Category("Settings")]
        [Description("The build output folder.")]
        public string OutputFolder { get; set; } = "build";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        /// <param name="totalItemsProvider">  The total items provider. </param>
        /// <param name="projectName">         The location. </param>
        /// <param name="projectLocation">     The location. </param>
        public ContentPropertyGridItem(Provider.Value<string> nameProvider,
                                       Provider.Value<string> virtualPathProvider,
                                       Provider.Value<int>    totalItemsProvider,
                                       string                 projectName,
                                       string                 projectLocation)
            : base(nameProvider, virtualPathProvider, totalItemsProvider)
        {
            ProjectName     = projectName;
            ProjectLocation = projectLocation;
        }
    }
}
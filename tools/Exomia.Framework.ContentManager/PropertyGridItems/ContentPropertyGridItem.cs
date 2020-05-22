#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.ComponentModel;
using Exomia.Framework.ContentManager.Attributes;
using Exomia.Framework.ContentManager.Converters;

namespace Exomia.Framework.ContentManager.PropertyGridItems
{
    /// <summary>
    ///     A content property grid item.
    /// </summary>
    class ContentPropertyGridItem : FolderPropertyGridItem
    {
        private readonly Func<string> _projectNameProvider, _projectLocationProvider;

        /// <summary>
        ///     The location of the project.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        [Category("Common")]
        [Description("The name of the project.")]
        [ReadOnly(true)]
        public string ProjectName
        {
            get { return _projectNameProvider(); }
        }

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
        /// <param name="projectNameProvider">     The project name provider. </param>
        /// <param name="projectLocationProvider"> The project location provider. </param>
        public ContentPropertyGridItem(Func<string> nameProvider,
                                       Func<string> virtualPathProvider,
                                       Func<int>    totalItemsProvider,
                                       Func<string> projectNameProvider,
                                       Func<string> projectLocationProvider)
            : base(nameProvider, virtualPathProvider, totalItemsProvider)
        {
            _projectNameProvider     = projectNameProvider;
            _projectLocationProvider = projectLocationProvider;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentPropertyGridItem" /> class.
        /// </summary>
        /// <param name="nameProvider">        The name provider. </param>
        /// <param name="virtualPathProvider"> The virtual path provider. </param>
        /// <param name="totalItemsProvider">  The total items provider. </param>
        /// <param name="projectName">         The location. </param>
        /// <param name="projectLocation">     The location. </param>
        public ContentPropertyGridItem(Func<string> nameProvider,
                                       Func<string> virtualPathProvider,
                                       Func<int>    totalItemsProvider,
                                       string       projectName,
                                       string       projectLocation)
            : base(nameProvider, virtualPathProvider, totalItemsProvider)
        {
            _projectNameProvider     = () => projectName;
            _projectLocationProvider = () => projectLocation;
        }
    }
}
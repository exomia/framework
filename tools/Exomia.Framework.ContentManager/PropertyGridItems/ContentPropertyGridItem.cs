#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;
using System.Drawing.Design;
using Exomia.Framework.ContentManager.Attributes;
using Exomia.Framework.ContentManager.PropertyGridItems.Editor;
using Newtonsoft.Json;

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
        [JsonIgnore]
        public string? ProjectName { get; set; }

        /// <summary>
        ///     The location of the project.
        /// </summary>
        /// <value>
        ///     The location.
        /// </value>
        [Category("Common")]
        [Description("The location of the project.")]
        [ReadOnly(true)]
        [JsonIgnore]
        public string? ProjectLocation { get; set; }

        /// <summary>
        ///     The build output folder.
        /// </summary>
        /// <value>
        ///     The pathname of the output folder.
        /// </value>
        [Category("Settings")]
        [Description("The build output folder.")]
        [FolderNameEditorTitle("Select the build output folder.")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string OutputFolder { get; set; } = "build";
    }
}
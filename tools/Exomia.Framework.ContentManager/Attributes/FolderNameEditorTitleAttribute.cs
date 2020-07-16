#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.ContentManager.Attributes
{
    /// <summary>
    ///     Attribute for folder name editor title.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FolderNameEditorTitleAttribute : Attribute
    {
        /// <summary>
        ///     Gets the title.
        /// </summary>
        /// <value>
        ///     The title.
        /// </value>
        public string Title { get; }

        /// <inheritdoc />
        public FolderNameEditorTitleAttribute(string title)
        {
            Title = title;
        }
    }
}
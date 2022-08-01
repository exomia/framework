#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.ComponentModel;

namespace Exomia.Framework.ContentManager.PropertyGridItems;

/// <summary>
///     A folder property grid item.
/// </summary>
class FolderPropertyGridItem : PropertyGridItem
{
    /// <summary>
    ///     The total item count of this project.
    /// </summary>
    /// <value>
    ///     The total number of items.
    /// </value>
    [Category("Statistics")]
    [Description("The total item count of this project.")]
    [ReadOnly(true)]
    public int TotalItems { get; set; }
}
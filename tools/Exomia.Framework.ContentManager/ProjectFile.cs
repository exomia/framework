#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.IO;
using Exomia.Framework.ContentManager.PropertyGridItems;
using Newtonsoft.Json;

namespace Exomia.Framework.ContentManager;

/// <summary>
///     A project file. This class cannot be inherited.
/// </summary>
sealed class ProjectFile
{
    /// <summary>
    ///     Gets the project name.
    /// </summary>
    /// <value>
    ///     The project name.
    /// </value>
    public string Name { get; }

    /// <summary>
    ///     Gets the project location.
    /// </summary>
    /// <value>
    ///     The project location.
    /// </value>
    public string Location { get; }

    /// <summary>
    ///     Gets the project file path.
    /// </summary>
    /// <value>
    ///     The project file path.
    /// </value>
    public string FilePath { get; }

    /// <summary>
    ///     Gets or sets the content.
    /// </summary>
    /// <value>
    ///     The content.
    /// </value>
    public ContentPropertyGridItem? Content { get; set; }

    /// <summary>
    ///     Gets or sets the resources.
    /// </summary>
    /// <value>
    ///     The resources.
    /// </value>
    public IList<PropertyGridItem> Resources { get; set; } = new List<PropertyGridItem>();

    /// <summary>
    ///     Initializes a new instance of the <see cref="ProjectFile" /> class.
    /// </summary>
    /// <param name="projectName">     The project name. </param>
    /// <param name="projectLocation"> The project location. </param>
    [JsonConstructor]
    public ProjectFile([JsonProperty("Name")]     string projectName,
                       [JsonProperty("Location")] string projectLocation)
    {
        Name     = projectName;
        Location = projectLocation;
        FilePath = Path.Combine(Location, $"{Name}.ecp");
    }

    /// <summary>
    ///     Adds a resource.
    /// </summary>
    /// <typeparam name="T"> Generic type parameter. </typeparam>
    /// <param name="resource"> The resource. </param>
    /// <returns>
    ///     A T.
    /// </returns>
    public T AddResource<T>(T resource) where T : PropertyGridItem
    {
        Resources.Add(resource);
        return resource;
    }
}
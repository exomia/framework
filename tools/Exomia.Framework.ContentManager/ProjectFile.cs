#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;

namespace Exomia.Framework.ContentManager
{
    /// <summary>
    ///     (Serializable) a project file. This class cannot be inherited.
    /// </summary>
    [Serializable]
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
        public string Content { get; set; } = "Content";

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProjectFile" /> class.
        /// </summary>
        /// <param name="projectName">     The project name. </param>
        /// <param name="projectLocation"> The project location. </param>
        public ProjectFile(string projectName, string projectLocation)
        {
            Name     = projectName;
            Location = projectLocation;
            FilePath = Path.Combine(Location, $"{Name}.ecp");
        }
    }
}
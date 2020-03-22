#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;

namespace Exomia.Framework.Graphics.Model
{
    /// <summary>
    ///     A material.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    public readonly struct Material
    {
        /// <summary>
        ///     The ambient.
        /// </summary>
        public readonly float Ambient;

        /// <summary>
        ///     The diffuse.
        /// </summary>
        public readonly float Diffuse;

        /// <summary>
        ///     The specular.
        /// </summary>
        public readonly float Specular;

        /// <summary>
        ///     The shininess.
        /// </summary>
        public readonly float Shininess;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="ambient">   The ambient. </param>
        /// <param name="diffuse">   The diffuse. </param>
        /// <param name="specular">  The specular. </param>
        /// <param name="shininess"> The shininess. </param>
        public Material(float ambient, float diffuse, float specular, float shininess)
        {
            Ambient   = ambient;
            Diffuse   = diffuse;
            Specular  = specular;
            Shininess = shininess;
        }
    };
}
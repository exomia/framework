#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Exomia.Framework.Graphics.Model
{
    /// <summary>
    ///     A mesh. This class cannot be inherited.
    /// </summary>
    public sealed class Mesh
    {
        /// <summary>
        ///     The mesh vertices.
        /// </summary>
        public readonly PositionNormalTexture2D[] Vertices;

        /// <summary>
        ///     The mesh.
        /// </summary>
        public readonly uint[] Indices;

        /// <summary>
        ///     The texture.
        /// </summary>
        public readonly Texture Texture;

        /// <summary>
        ///     The material.
        /// </summary>
        public readonly Material Material;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mesh" /> class.
        /// </summary>
        /// <param name="vertices"> The mesh vertices. </param>
        /// <param name="indices">  The mesh indices. </param>
        /// <param name="texture">  The texture. </param>
        /// <param name="material"> The material. </param>
        private Mesh(PositionNormalTexture2D[] vertices, uint[] indices, Texture texture, Material material)
        {
            Vertices = vertices;
            Indices  = indices;
            Texture  = texture;
            Material = material;
        }

        /// <summary>
        ///     A position normal texture.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 36)]
        public readonly struct PositionNormalTexture2D
        {
            /// <summary>
            ///     The X coordinate.
            /// </summary>
            [FieldOffset(0)]
            public readonly float X;

            /// <summary>
            ///     The Y coordinate.
            /// </summary>
            [FieldOffset(4)]
            public readonly float Y;

            /// <summary>
            ///     The Z coordinate.
            /// </summary>
            [FieldOffset(8)]
            public readonly float Z;

            /// <summary>
            ///     The width.
            /// </summary>
            [FieldOffset(12)]
            public readonly float W;

            /// <summary>
            ///     The nx.
            /// </summary>
            [FieldOffset(16)]
            public readonly float NX;

            /// <summary>
            ///     The ny.
            /// </summary>
            [FieldOffset(20)]
            public readonly float NY;

            /// <summary>
            ///     The nz.
            /// </summary>
            [FieldOffset(24)]
            public readonly float NZ;

            /// <summary>
            ///     The float to process.
            /// </summary>
            [FieldOffset(28)]
            public readonly float U;

            /// <summary>
            ///     The float to process.
            /// </summary>
            [FieldOffset(32)]
            public readonly float V;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Mesh" /> class.
            /// </summary>
            /// <param name="x">  The X coordinate. </param>
            /// <param name="y">  The Y coordinate. </param>
            /// <param name="z">  The Z coordinate. </param>
            /// <param name="w">  The w coordinate. </param>
            /// <param name="nx"> The normal x. </param>
            /// <param name="ny"> The normal y. </param>
            /// <param name="nz"> The normal z. </param>
            /// <param name="u">  The texture u. </param>
            /// <param name="v">  The texture v. </param>
            public PositionNormalTexture2D(float x,
                                           float y,
                                           float z,
                                           float w,
                                           float nx,
                                           float ny,
                                           float nz,
                                           float u,
                                           float v)
            {
                X  = x;
                Y  = y;
                Z  = z;
                W  = w;
                NX = nx;
                NY = ny;
                NZ = nz;
                U  = u;
                V  = v;
            }
        }

        /// <summary>
        ///     Initializes a new <see cref="Mesh" /> instance from the given <see cref="Obj" /> instance.
        /// </summary>
        /// <param name="obj">      The object. </param>
        /// <param name="texture">  The texture. </param>
        /// <param name="material"> The material. </param>
        /// <returns>
        ///     A Mesh.
        /// </returns>
        public static Mesh FromObj(Obj obj, Texture texture, Material material)
        {
            List<PositionNormalTexture2D> vertices = new List<PositionNormalTexture2D>();
            List<uint>                    indices  = new List<uint>();

            uint index = 0;
            foreach (Obj.Face face in obj.Faces)
            {
                PositionNormalTexture2D FromIndex(int i)
                {
                    Obj.Vertex  v = obj.Vertices[face.Vertices[i].V - 1];
                    Obj.Normal  n = obj.Normals[face.Vertices[i].N - 1];
                    Obj.Texture t = obj.Textures[face.Vertices[i].T - 1];
                    return new PositionNormalTexture2D(v.X, v.Y, v.Z, v.W, n.X, n.Y, n.Z, t.U, t.V);
                }

                vertices.Add(FromIndex(0));
                vertices.Add(FromIndex(1));
                uint indexZero = index++;
                for (int i = 2; i < face.Vertices.Length; i++)
                {
                    indices.Add(indexZero);
                    indices.Add(index++);
                    indices.Add(index);
                    vertices.Add(FromIndex(i));
                }
                index++;
            }

            return new Mesh(vertices.ToArray(), indices.ToArray(), texture, material);
        }
    }
}
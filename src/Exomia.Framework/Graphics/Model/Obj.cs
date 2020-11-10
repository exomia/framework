#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Runtime.InteropServices;
using Exomia.Framework.Content;

namespace Exomia.Framework.Graphics.Model
{
    /// <summary>
    ///     An object. This class cannot be inherited.
    /// </summary>
    [ContentReadable(typeof(ObjContentReader))]
    public sealed class Obj
    {
        /// <summary>
        ///     Gets the vertices.
        /// </summary>
        /// <value>
        ///     The vertices.
        /// </value>
        public Vertex[] Vertices { get; }

        /// <summary>
        ///     Gets the normals.
        /// </summary>
        /// <value>
        ///     The normals.
        /// </value>
        public Normal[] Normals { get; }

        /// <summary>
        ///     Gets the textures.
        /// </summary>
        /// <value>
        ///     The textures.
        /// </value>
        public Texture[] Textures { get; }

        /// <summary>
        ///     Gets the faces.
        /// </summary>
        /// <value>
        ///     The faces.
        /// </value>
        public Face[] Faces { get; }

        private Obj(Vertex[] vertices, Normal[] normals, Texture[] textures, Face[] faces)
        {
            Vertices = vertices;
            Normals  = normals;
            Textures = textures;
            Faces    = faces;
        }

        /// <summary>
        ///     A vertex.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct Vertex
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
            ///     Initializes a new instance of the <see cref="Vertex" /> class.
            /// </summary>
            /// <param name="x"> The X coordinate. </param>
            /// <param name="y"> The Y coordinate. </param>
            /// <param name="z"> The Z coordinate. </param>
            /// <param name="w"> The W coordinate. </param>
            public Vertex(float x, float y, float z, float w)
            {
                X = x;
                Y = y;
                Z = z;
                W = w;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{X}/{Y}/{Z}/{W}";
            }
        }

        /// <summary>
        ///     A vertex.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 12)]
        public readonly struct Normal
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
            ///     Initializes a new instance of the <see cref="Normal" /> class.
            /// </summary>
            /// <param name="x"> The X coordinate. </param>
            /// <param name="y"> The Y coordinate. </param>
            /// <param name="z"> The Z coordinate. </param>
            public Normal(float x, float y, float z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{X}/{Y}/{Z}";
            }
        }

        /// <summary>
        ///     A vertex.
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 12)]
        public readonly struct Texture
        {
            /// <summary>
            ///     The U coordinate.
            /// </summary>
            [FieldOffset(0)]
            public readonly float U;

            /// <summary>
            ///     The V coordinate.
            /// </summary>
            [FieldOffset(4)]
            public readonly float V;

            /// <summary>
            ///     The W coordinate.
            /// </summary>
            [FieldOffset(8)]
            public readonly float W;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Texture" /> class.
            /// </summary>
            /// <param name="u"> The U coordinate. </param>
            /// <param name="v"> The V coordinate. </param>
            /// <param name="w"> The W coordinate. </param>
            public Texture(float u, float v, float w)
            {
                U = u;
                V = v;
                W = w;
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{U}/{V}/{W}";
            }
        }

        /// <summary>
        ///     A vertex.
        /// </summary>
        public readonly struct Face
        {
            /// <summary>
            ///     The vertices.
            /// </summary>
            public readonly Vertex[] Vertices;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Obj" /> class.
            /// </summary>
            /// <param name="vertices"> The count of the vertices. </param>
            public Face(int vertices)
            {
                Vertices = new Vertex[vertices];
            }

            /// <summary>
            ///     A face vertex.
            /// </summary>
            [StructLayout(LayoutKind.Explicit, Size = 12)]

            // ReSharper disable once MemberHidesStaticFromOuterClass
            public readonly struct Vertex
            {
                /// <summary>
                ///     The X coordinate.
                /// </summary>
                [FieldOffset(0)]
                public readonly int V;

                /// <summary>
                ///     The Y coordinate.
                /// </summary>
                [FieldOffset(4)]
                public readonly int T;

                /// <summary>
                ///     The Z coordinate.
                /// </summary>
                [FieldOffset(8)]
                public readonly int N;

                /// <summary>
                ///     Initializes a new instance of the <see cref="Obj" /> class.
                /// </summary>
                /// <param name="v"> The X coordinate. </param>
                /// <param name="t"> The Y coordinate. </param>
                /// <param name="n"> The Z coordinate. </param>
                public Vertex(int v, int t, int n)
                {
                    V = v;
                    T = t;
                    N = n;
                }

                /// <inheritdoc />
                public override string ToString()
                {
                    return $"{V}/{T}/{N}";
                }
            }

            /// <inheritdoc />
            public override string ToString()
            {
                return $"{string.Join(", ", Vertices)} [{Vertices.Length}]";
            }
        }

        internal static Obj Create(Vertex[] vertices, Normal[] normals, Texture[] textures, Face[] faces)
        {
            return new Obj(vertices, normals, textures, faces);
        }
    }
}
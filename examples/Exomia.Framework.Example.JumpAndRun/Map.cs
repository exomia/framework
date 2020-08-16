#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Example.JumpAndRun
{
    class Map
    {
        public string    MapId      { get; }
        public Texture   Texture    { get; set; }
        public Grid      Grid       { get; set; }
        public string[]? References { get; set; }

        public Map(string mapId)
        {
            MapId = mapId;
        }
    }

    readonly struct Texture
    {
        public readonly string Asset;
        public readonly int    Columns;
        public readonly int    Rows;
        public readonly int    Width;
        public readonly int    Height;

        public Texture(string asset, int columns, int rows, int width, int height)
        {
            Asset   = asset;
            Columns = columns;
            Rows    = rows;
            Width   = width;
            Height  = height;
        }
    }

    readonly struct Grid
    {
        public readonly int   Columns;
        public readonly int   Row;
        public readonly int   Width;
        public readonly int   Height;
        public readonly int[] Indices;

        public Grid(int columns, int row, int width, int height, int[] indices)
        {
            Columns = columns;
            Row     = row;
            Width   = width;
            Height  = height;
            Indices = indices;
        }
    }
}
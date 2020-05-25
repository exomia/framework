#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable InconsistentNaming

namespace Exomia.Framework.ContentManager.IO.Importer
{
    /// <summary>
    ///     A bm font importer. This class cannot be inherited.
    /// </summary>
    sealed class BMFontImporter : IImporter
    {
        public static BMFontImporter Default = new BMFontImporter();

        /// <inheritdoc />
        public string Name { get; } = "BMFont Importer";

        private BMFontImporter() { }
    }
}
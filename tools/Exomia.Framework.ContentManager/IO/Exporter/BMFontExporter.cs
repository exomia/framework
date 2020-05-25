#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

// ReSharper disable InconsistentNaming

namespace Exomia.Framework.ContentManager.IO.Exporter
{
    /// <summary>
    ///     A bm font exporter. This class cannot be inherited.
    /// </summary>
    sealed class BMFontExporter : IExporter
    {
        public static BMFontExporter Default = new BMFontExporter();

        /// <inheritdoc />
        public string Name { get; } = "BMFont Exporter";

        private BMFontExporter() { }
    }
}
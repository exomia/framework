#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;

namespace Exomia.Framework.ContentManager.IO
{
    abstract class Exporter<T> : IExporter
    {
        /// <inheritdoc />
        public Type ImportType
        {
            get { return typeof(T); }
        }

        bool IExporter.Export(object obj, ExporterContext context)
        {
            return Export((T)obj, context);
        }

        public abstract bool Export(T obj, ExporterContext context);
    }
}
﻿#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.IO;

namespace Exomia.Framework.ContentManager.IO
{
    interface IImporter
    {
        System.Type OutType { get; }
        
        object? Import(Stream stream, ImporterContext context);
    }
}
﻿#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core;

/// <summary> An interface to load and unload content. </summary>
public interface IContentable
{
    /// <summary> Loads the content. </summary>
    void LoadContent();

    /// <summary> Called when graphics resources need to be unloaded. </summary>
    void UnloadContent();
}
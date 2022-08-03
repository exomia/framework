#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Application;

public abstract partial class Application
{
    /// <summary> Called once before <see cref="OnInitialize" /> to perform user-defined initialization. </summary>
    protected virtual void OnBeforeInitialize() { }

    /// <summary>
    ///     Called once after the application is created to perform user- defined initialization.
    ///     Called once before <see cref="OnAfterInitialize" />.
    /// </summary>
    protected virtual void OnInitialize() { }

    /// <summary> Called once before <see cref="LoadContent" /> to perform user-defined initialization. </summary>
    protected virtual void OnAfterInitialize() { }

    private void InitializePendingInitializations()
    {
        while (_pendingInitializables.Count != 0)
        {
            _pendingInitializables[0].Initialize();
            _pendingInitializables.RemoveAt(0);
        }
    }
}
#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Scene;

public abstract partial class SceneBase
{
    /// <summary>
    ///     Called once after the scene is created to perform user- defined initialization.
    /// </summary>
    protected virtual void OnInitialize() { }

    /// <summary> Called than all referenced scenes are loaded. </summary>
    protected virtual void OnReferenceScenesLoaded() { }

    internal void Initialize()
    {
        if (!_isInitialized && _state != SceneState.Initializing)
        {
            State = SceneState.Initializing;

            OnInitialize();

            while (_pendingInitializables.Count != 0)
            {
                _pendingInitializables[0].Initialize();
                _pendingInitializables.RemoveAt(0);
            }

            State          = SceneState.StandBy;
            _isInitialized = true;
        }
    }

    internal void ReferenceScenesLoaded()
    {
        OnReferenceScenesLoaded();
    }
}
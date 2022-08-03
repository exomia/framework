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
    /// <summary> Executes the load content action. </summary>
    protected virtual void OnLoadContent() { }

    /// <summary> Executes the unload content action. </summary>
    protected virtual void OnUnloadContent() { }

    internal void LoadContent()
    {
        if (_isInitialized && !_isContentLoaded && _state != SceneState.ContentLoading)
        {
            State = SceneState.ContentLoading;
            OnLoadContent();

            lock (_contentableComponents)
            {
                _currentlyContentableComponents.AddRange(_contentableComponents);
            }

            // ReSharper disable once InconsistentlySynchronizedField
            foreach (IContentable contentable in _currentlyContentableComponents)
            {
                contentable.LoadContent();
            }

            // ReSharper disable once InconsistentlySynchronizedField
            _currentlyContentableComponents.Clear();
            _isContentLoaded = true;
            State            = SceneState.Ready;
        }
    }

    internal void UnloadContent()
    {
        if (_isContentLoaded && _state == SceneState.Ready)
        {
            State = SceneState.ContentUnloading;
            OnUnloadContent();

            lock (_contentableComponents)
            {
                _currentlyContentableComponents.AddRange(_contentableComponents);
            }

            // ReSharper disable once InconsistentlySynchronizedField
            foreach (IContentable contentable in _currentlyContentableComponents)
            {
                contentable.UnloadContent();
            }

            // ReSharper disable once InconsistentlySynchronizedField
            _currentlyContentableComponents.Clear();
            _isContentLoaded = false;

            State = SceneState.StandBy;
        }
    }
}
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
    /// <summary> Called once to perform user-defined loading. </summary>
    protected virtual void OnLoadContent() { }

    /// <summary> Called once to perform user-defined unloading. </summary>
    protected virtual void OnUnloadContent() { }

    private void LoadContent()
    {
        if (!_isContentLoaded)
        {
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
        }
    }

    private void UnloadContent()
    {
        if (_isContentLoaded)
        {
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
        }
    }
}
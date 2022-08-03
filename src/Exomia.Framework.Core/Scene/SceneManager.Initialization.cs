#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.Scene;

sealed partial class SceneManager
{
    /// <inheritdoc />
    void IInitializable.Initialize()
    {
        if (!_isInitialized)
        {
            lock (_pendingInitializableScenes)
            {
                _pendingInitializableScenes[0].Initialize();
                _pendingInitializableScenes[0].LoadContent();
                _pendingInitializableScenes.RemoveAt(0);

                while (_pendingInitializableScenes.Count != 0)
                {
                    _pendingInitializableScenes[0].Initialize();
                    _pendingInitializableScenes.RemoveAt(0);
                }
            }

            _isInitialized = true;
        }
    }
}
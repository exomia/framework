#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System.Diagnostics.CodeAnalysis;
using Exomia.Framework.Core.Input;

namespace Exomia.Framework.Core.Scene;

sealed partial class SceneManager : ISceneManager, IInitializable, IUpdateable, IRenderable, IDisposable
{
    private const int INITIAL_QUEUE_SIZE = 16;

    private readonly Dictionary<string, SceneBase> _scenes;
    private readonly List<SceneBase>               _pendingInitializableScenes;
    private readonly List<SceneBase>               _currentScenes;
    private readonly List<SceneBase>               _currentDrawableScenes;
    private readonly List<SceneBase>               _currentUpdateableScenes;
    private readonly List<SceneBase>               _scenesToUnload;
    private readonly IInputDevice                  _inputDevice;
    private          bool                          _isInitialized;
    private          bool                          _enabled;
    private          int                           _updateOrder;
    private          int                           _drawOrder;
    private          bool                          _visible;

    public SceneManager(IInputDevice inputDevice, IEnumerable<(bool, SceneBase)> sceneCollection)
    {
        _inputDevice = inputDevice ?? throw new ArgumentNullException(nameof(inputDevice));

        _scenes        = new Dictionary<string, SceneBase>(INITIAL_QUEUE_SIZE);
        _currentScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);

        _currentUpdateableScenes    = new List<SceneBase>(INITIAL_QUEUE_SIZE);
        _currentDrawableScenes      = new List<SceneBase>(INITIAL_QUEUE_SIZE);
        _pendingInitializableScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);
        _scenesToUnload             = new List<SceneBase>(INITIAL_QUEUE_SIZE);

        foreach ((bool initialize, SceneBase scene) in sceneCollection)
        {
            if (_scenes.ContainsKey(scene.Key)) { throw new NotSupportedException("A scene with the same key already exists!"); }

            scene.SceneManager = this;
            _scenes.Add(scene.Key, scene);

            if (initialize)
            {
                lock (_pendingInitializableScenes)
                {
                    _pendingInitializableScenes.Add(scene);
                }
            }
        }
    }

    /// <inheritdoc />
    public bool GetScene(string key, [NotNullWhen(true)] out SceneBase? scene)
    {
        return _scenes.TryGetValue(key, out scene);
    }

    /// <inheritdoc />
    public SceneState GetSceneState(string key)
    {
        if (_scenes.TryGetValue(key, out SceneBase? scene))
        {
            return scene!.State;
        }
        throw new NullReferenceException($"No scene with key: '{key}' found.");
    }

    /// <inheritdoc />
    public bool RemoveScene(string key)
    {
        if (!_scenes.TryGetValue(key, out SceneBase? scene))
        {
            return false;
        }

        HideScene(scene);

        _scenes.Remove(key);
        scene.UnloadContent();
        scene.Dispose();

        return true;
    }

    /// <inheritdoc />
    public ShowSceneResult ShowScene(SceneBase scene, params object[] payload)
    {
        lock (this)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (scene.State)
            {
                case SceneState.ContentLoading:
                case SceneState.Ready:
                    break;
                default: throw new Exception($"Scene is in wrong state to be shown {scene.State}");
            }

            SceneBase? oldScene = null;
            if (_currentScenes.Count > 0)
            {
                oldScene = _currentScenes[_currentScenes.Count - 1];

                // ReSharper disable once SuspiciousTypeConversion.Global
                if (oldScene is IInputHandler oldInputHandler)
                {
                    oldInputHandler.UnregisterInput(_inputDevice);
                }
            }
            scene.Show(oldScene, payload);

            if (!scene.IsOverlayScene)
            {
                lock (_currentScenes)
                {
                    _scenesToUnload.AddRange(_currentScenes);
                    _currentScenes.Clear();
                }
                Task.Factory.StartNew(
                    () =>
                    {
                        for (int i = _scenesToUnload.Count - 1; i >= 0; --i)
                        {
                            SceneBase uScene = _scenesToUnload[i];
                            if (uScene.Key == scene.Key)
                            {
                                _scenesToUnload.RemoveAt(i);
                                continue;
                            }

                            bool isReferenced = false;
                            for (int k = scene.ReferenceScenes.Length - 1; k >= 0; --k)
                            {
                                string referenceSceneKey = scene.ReferenceScenes[k];
                                if (referenceSceneKey == uScene.Key)
                                {
                                    isReferenced = true;
                                    break;
                                }
                            }
                            if (!isReferenced)
                            {
                                // ReSharper disable once SuspiciousTypeConversion.Global
                                ((IContentable)uScene).UnloadContent();
                                _scenesToUnload.RemoveAt(i);
                            }
                        }
                    });
            }

            Task.Factory.StartNew(
                () =>
                {
                    for (int i = scene.ReferenceScenes.Length - 1; i >= 0; --i)
                    {
                        string referenceSceneKey = scene.ReferenceScenes[i];
                        if (!GetScene(referenceSceneKey, out SceneBase? rScene))
                        {
                            throw new ArgumentNullException(referenceSceneKey);
                        }
                        if (rScene.State == SceneState.StandBy)
                        {
                            rScene.LoadContent();
                        }
                    }
                    scene.ReferenceScenesLoaded();
                });

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (scene is IInputHandler inputHandler)
            {
                inputHandler.RegisterInput(_inputDevice);
            }

            lock (_currentScenes)
            {
                _currentScenes.Add(scene);
            }

            return scene.State == SceneState.Ready
                ? ShowSceneResult.Success
                : ShowSceneResult.NotReady;
        }
    }

    /// <inheritdoc />
    public ShowSceneResult ShowScene(string key, out SceneBase? scene, params object[] payload)
    {
        ShowSceneResult result = !_scenes.TryGetValue(key, out scene)
            ? ShowSceneResult.NoScene
            : ShowScene(scene, payload);
        return result;
    }

    /// <inheritdoc />
    public bool HideScene(string key)
    {
        return _scenes.TryGetValue(key, out SceneBase? intern) && HideScene(intern);
    }

    public bool HideScene(SceneBase scene)
    {
        lock (_currentScenes)
        {
            if (!_currentScenes.Remove(scene)) { return false; }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (scene is IInputHandler inputHandler)
            {
                inputHandler.UnregisterInput(_inputDevice);
            }

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (_currentScenes.Count > 0 &&
                _currentScenes[_currentScenes.Count - 1] is IInputHandler nextInputHandler)
            {
                nextInputHandler.RegisterInput(_inputDevice);
            }

            return true;
        }
    }

    #region IDisposable Support

    private bool _disposed;

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                lock (_currentScenes)
                {
                    _currentScenes.Clear();
                }

                foreach (SceneBase scene in _scenes.Values)
                {
                    scene.UnloadContent();
                    scene.Dispose();
                }

                _scenes.Clear();
            }
            _disposed = true;
        }
    }

    ~SceneManager()
    {
        Dispose(false);
    }

    #endregion
}
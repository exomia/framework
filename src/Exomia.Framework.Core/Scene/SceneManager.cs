#region License

// Copyright (c) 2018-2021, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;

namespace Exomia.Framework.Core.Scene
{
    internal sealed class SceneManager : ISceneManager, IInitializable, IUpdateable, IDrawable, IDisposable
    {
        private const int INITIAL_QUEUE_SIZE = 16;

        /// <summary> Occurs when Enabled Changed. </summary>
        public event EventHandler? EnabledChanged;

        /// <summary> Occurs when Update Order Changed. </summary>
        public event EventHandler? UpdateOrderChanged;

        /// <summary> Occurs when the <see cref="DrawOrder" /> property changes. </summary>
        public event EventHandler? DrawOrderChanged;

        /// <summary> Occurs when the <see cref="Visible" /> property changes. </summary>
        public event EventHandler? VisibleChanged;

        private readonly List<SceneBase>               _currentDrawableScenes;
        private readonly List<SceneBase>               _currentScenes;
        private readonly List<SceneBase>               _currentUpdateableScenes;
        private readonly List<SceneBase>               _pendingInitializableScenes;
        private readonly Dictionary<string, SceneBase> _scenes;
        private readonly List<SceneBase>               _scenesToUnload;
        private readonly IInputDevice                  _inputDevice;
        private          bool                          _isInitialized;
        private          bool                          _enabled;
        private          int                           _updateOrder;
        private          int                           _drawOrder;
        private          bool                          _visible;

        /// <inheritdoc/>
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc/>
        public int UpdateOrder
        {
            get { return _updateOrder; }
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc/>
        public int DrawOrder
        {
            get { return _drawOrder; }
            set
            {
                if (_drawOrder != value)
                {
                    _drawOrder = value;
                    DrawOrderChanged?.Invoke();
                }
            }
        }

        /// <inheritdoc/>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (_visible != value)
                {
                    _visible = value;
                    VisibleChanged?.Invoke();
                }
            }
        }

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

        /// <inheritdoc/>
        bool IDrawable.BeginDraw()
        {
            return _visible;
        }

        /// <inheritdoc/>
        void IDrawable.Draw(GameTime gameTime)
        {
            lock (_currentScenes)
            {
                _currentDrawableScenes.AddRange(_currentScenes);
            }
            for (int i = 0; i < _currentDrawableScenes.Count; i++)
            {
                SceneBase scene = _currentDrawableScenes[i];
                if (scene.State == SceneState.Ready && scene.BeginDraw())
                {
                    scene.Draw(gameTime);
                    scene.EndDraw();
                }
            }

            _currentDrawableScenes.Clear();
        }

        /// <inheritdoc/>
        void IDrawable.EndDraw() { }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool GetScene(string key, [NotNullWhen(true)] out SceneBase? scene)
        {
            return _scenes.TryGetValue(key, out scene);
        }

        /// <inheritdoc/>
        public SceneState GetSceneState(string key)
        {
            if (_scenes.TryGetValue(key, out SceneBase? scene))
            {
                return scene!.State;
            }
            throw new NullReferenceException($"no scene with key: '{key}' found.");
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public ShowSceneResult ShowScene(string key, out SceneBase? scene, params object[] payload)
        {
            ShowSceneResult result = !_scenes.TryGetValue(key, out scene)
                ? ShowSceneResult.NoScene
                : ShowScene(scene, payload);
            return result;
        }

        /// <inheritdoc/>
        public bool HideScene(string key)
        {
            return _scenes.TryGetValue(key, out SceneBase? intern) && HideScene(intern);
        }

        /// <inheritdoc/>
        void IUpdateable.Update(GameTime gameTime)
        {
            lock (_currentScenes)
            {
                _currentUpdateableScenes.AddRange(_currentScenes);
            }

            for (int i = _currentUpdateableScenes.Count - 1; i >= 0; i--)
            {
                SceneBase scene = _currentUpdateableScenes[i];
                if (scene.State == SceneState.Ready && scene.Enabled)
                {
                    scene.Update(gameTime);
                }
            }

            _currentUpdateableScenes.Clear();
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

        /// <inheritdoc/>
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
}
#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exomia.Framework.Core.Game;
using Exomia.Framework.Core.Input;

namespace Exomia.Framework.Core.Scene
{
    /// <summary>
    ///     Manager for scenes. This class cannot be inherited.
    /// </summary>
    public sealed class SceneManager : DrawableComponent, ISceneManager
    {
        private const int INITIAL_QUEUE_SIZE = 16;

        private readonly List<ISceneInternal>               _currentDrawableScenes;
        private readonly List<ISceneInternal>               _currentScenes;
        private readonly List<ISceneInternal>               _currentUpdateableScenes;
        private readonly List<ISceneInternal>               _pendingInitializableScenes;
        private readonly Dictionary<string, ISceneInternal> _scenes;
        private readonly List<ISceneInternal>               _scenesToUnload;
        private          IServiceRegistry?                  _registry;
        private          IInputDevice                       _inputDevice;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="SceneManager" /> class.
        /// </summary>
        public SceneManager(IScene startScene, string name = "SceneManager")
            : base(name)
        {
            if (startScene == null) { throw new ArgumentNullException(nameof(startScene)); }
            _scenes        = new Dictionary<string, ISceneInternal>(INITIAL_QUEUE_SIZE);
            _currentScenes = new List<ISceneInternal>(INITIAL_QUEUE_SIZE);

            _currentUpdateableScenes    = new List<ISceneInternal>(INITIAL_QUEUE_SIZE);
            _currentDrawableScenes      = new List<ISceneInternal>(INITIAL_QUEUE_SIZE);
            _pendingInitializableScenes = new List<ISceneInternal>(INITIAL_QUEUE_SIZE);
            _scenesToUnload             = new List<ISceneInternal>(INITIAL_QUEUE_SIZE);

            _inputDevice = null!;

            AddScene(startScene);
        }

        /// <inheritdoc />
        public bool AddScene(IScene scene, bool initialize = true)
        {
            if (!(scene is ISceneInternal intern))
            {
                throw new InvalidCastException();
            }

            if (string.IsNullOrEmpty(intern.Key) && _scenes.ContainsKey(intern.Key)) { return false; }

            intern.SceneManager = this;

            if (initialize)
            {
                if (!_isInitialized)
                {
                    lock (_pendingInitializableScenes)
                    {
                        _pendingInitializableScenes.Add(intern);
                    }
                }
                else
                {
                    intern.Initialize(_registry!);
                }
            }

            _scenes.Add(intern.Key, intern);

            return true;
        }

        /// <inheritdoc />
        public bool GetScene(string key, out IScene scene)
        {
            bool result = _scenes.TryGetValue(key, out ISceneInternal intern);
            scene = intern;
            return result;
        }

        /// <inheritdoc />
        public SceneState GetSceneState(string key)
        {
            if (_scenes.TryGetValue(key, out ISceneInternal scene))
            {
                return scene.State;
            }
            throw new NullReferenceException($"no scene with key: '{key}' found.");
        }

        /// <inheritdoc />
        public bool RemoveScene(string key)
        {
            if (!_scenes.TryGetValue(key, out ISceneInternal scene))
            {
                return false;
            }

            HideScene(scene);

            _scenes.Remove(key);
            scene.UnloadContent(_registry!);
            scene.Dispose();

            return true;
        }

        /// <inheritdoc />
        public ShowSceneResult ShowScene(IScene s, params object[] payload)
        {
            lock (this)
            {
                if (!(s is ISceneInternal scene)) { return ShowSceneResult.NoScene; }

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (scene.State)
                {
                    case SceneState.ContentLoading:
                    case SceneState.Ready:
                        break;
                    default: throw new Exception($"Scene is in wrong state to be shown {scene.State}");
                }

                IScene? oldScene = null;
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
                                IScene uScene = _scenesToUnload[i];
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
                                    uScene.UnloadContent(_registry!);
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
                            if (!GetScene(referenceSceneKey, out IScene rScene))
                            {
                                throw new ArgumentNullException(referenceSceneKey);
                            }
                            if (rScene.State == SceneState.StandBy)
                            {
                                rScene.LoadContent(_registry!);
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

                return scene.State == SceneState.Ready ? ShowSceneResult.Success : ShowSceneResult.NotReady;
            }
        }

        /// <inheritdoc />
        public ShowSceneResult ShowScene(string key, out IScene scene, params object[] payload)
        {
            ShowSceneResult result = !_scenes.TryGetValue(key, out ISceneInternal intern)
                ? ShowSceneResult.NoScene
                : ShowScene(intern, payload);
            scene = intern;
            return result;
        }

        /// <inheritdoc />
        public bool HideScene(string key)
        {
            return _scenes.TryGetValue(key, out ISceneInternal intern) && HideScene(intern);
        }

        /// <summary>
        ///     Hides the scene.
        /// </summary>
        /// <param name="scene"> The scene. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        public bool HideScene(IScene scene)
        {
            lock (_currentScenes)
            {
                if (!_currentScenes.Remove((ISceneInternal)scene)) { return false; }

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

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            lock (_currentScenes)
            {
                _currentUpdateableScenes.AddRange(_currentScenes);
            }

            for (int i = _currentUpdateableScenes.Count - 1; i >= 0; i--)
            {
                ISceneInternal scene = _currentUpdateableScenes[i];
                if (scene.State == SceneState.Ready && scene.Enabled)
                {
                    scene.Update(gameTime);
                }
            }

            _currentUpdateableScenes.Clear();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            lock (_currentScenes)
            {
                _currentDrawableScenes.AddRange(_currentScenes);
            }
            for (int i = 0; i < _currentDrawableScenes.Count; i++)
            {
                ISceneInternal scene = _currentDrawableScenes[i];
                if (scene.State == SceneState.Ready && scene.BeginDraw())
                {
                    scene.Draw(gameTime);
                    scene.EndDraw();
                }
            }

            _currentDrawableScenes.Clear();
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _registry    = registry;
            _inputDevice = registry.GetService<IInputDevice>();

            lock (_pendingInitializableScenes)
            {
                _pendingInitializableScenes[0].Initialize(registry);
                _pendingInitializableScenes[0].LoadContent(registry);
                _pendingInitializableScenes.RemoveAt(0);

                while (_pendingInitializableScenes.Count != 0)
                {
                    _pendingInitializableScenes[0].Initialize(registry);
                    _pendingInitializableScenes.RemoveAt(0);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnDispose(bool dispose)
        {
            if (dispose)
            {
                lock (_currentScenes)
                {
                    _currentScenes.Clear();
                }

                foreach (ISceneInternal scene in _scenes.Values)
                {
                    scene.UnloadContent(_registry!);
                    scene.Dispose();
                }

                _scenes.Clear();
            }
        }
    }
}
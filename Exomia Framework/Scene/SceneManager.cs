#region MIT License

// Copyright (c) 2018 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Exomia.Framework.Game;
using Exomia.Framework.Input;

namespace Exomia.Framework.Scene
{
    /// <inheritdoc cref="ADrawableComponent" />
    /// <inheritdoc cref="ISceneManager" />
    public sealed class SceneManager : ADrawableComponent, ISceneManager
    {
        private const int INITIAL_QUEUE_SIZE = 16;
        private readonly List<SceneBase> _currentDrawableScenes;
        private readonly List<SceneBase> _currentScenes;

        private readonly List<SceneBase> _currentUpdateableScenes;

        private readonly List<SceneBase> _pendingInitializableScenes;

        private readonly Dictionary<string, SceneBase> _scenes;

        private readonly List<SceneBase> _scenesToUnload;

        private IInputDevice _input;

        private IInputHandler _inputHandler;

        private IServiceRegistry _registry;

        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="SceneManager" /> class.
        /// </summary>
        public SceneManager(Game.Game game, SceneBase startScene, string name = "SceneManager")
            : base(game, name)
        {
            if (startScene == null) { throw new ArgumentNullException(nameof(startScene)); }
            _scenes = new Dictionary<string, SceneBase>(INITIAL_QUEUE_SIZE);
            _currentScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);

            _currentUpdateableScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);
            _currentDrawableScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);
            _pendingInitializableScenes = new List<SceneBase>(INITIAL_QUEUE_SIZE);
            _scenesToUnload = new List<SceneBase>(INITIAL_QUEUE_SIZE);

            AddScene(startScene, true);
        }

        /// <inheritdoc />
        public bool AddScene(SceneBase scene, bool initialize = true)
        {
            if (string.IsNullOrEmpty(scene.Key) && _scenes.ContainsKey(scene.Key)) { return false; }

            if (scene is IScene intern)
            {
                intern.SceneManager = this;
            }

            if (initialize)
            {
                if (!_isInitialized)
                {
                    lock (_pendingInitializableScenes)
                    {
                        _pendingInitializableScenes.Add(scene);
                    }
                }
                else
                {
                    scene.Initialize(_registry);
                }
            }

            _scenes.Add(scene.Key, scene);

            return true;
        }

        /// <inheritdoc />
        public bool RemoveScene(string key)
        {
            if (!_scenes.TryGetValue(key, out SceneBase scene))
            {
                return false;
            }
            _scenes.Remove(key);

            HideScene(scene);

            scene.UnloadContent();
            scene.Dispose();

            return true;
        }

        /// <inheritdoc />
        public bool HideScene(string key)
        {
            return _scenes.TryGetValue(key, out SceneBase scene) && HideScene(scene);
        }

        /// <inheritdoc />
        public ShowSceneResult ShowScene(SceneBase scene)
        {
            lock (this)
            {
                if (scene == null) { return ShowSceneResult.NoScene; }
                switch (scene.State)
                {
                    case SceneState.ContentLoading:
                    case SceneState.Ready:
                        break;
                    default: throw new Exception("Scene is in wrong state to be shown " + scene.State);
                }

                if (!scene.IsOverlayScene)
                {
                    lock (_currentScenes)
                    {
                        _scenesToUnload.AddRange(_currentScenes); //m
                        _currentScenes.Clear();
                    }
                    Task.Factory.StartNew(
                        () =>
                        {
                            int c = _scenesToUnload.Count - 1;
                            for (int i = c; i >= 0; i--)
                            {
                                IScene uScene = _scenesToUnload[i];

                                if (uScene.Key == scene.Key)
                                {
                                    _scenesToUnload.RemoveAt(i);
                                    continue;
                                }

                                bool isReferenced = false;
                                int l = scene.ReferenceScenes.Length;
                                for (int k = 0; k < l; k++)
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
                                    uScene.UnloadContent();
                                    _scenesToUnload.RemoveAt(i);
                                }
                            }
                        });
                }

                scene.Show();

                Task.Factory.StartNew(
                    () =>
                    {
                        int l = scene.ReferenceScenes.Length;
                        for (int i = 0; i < l; i++)
                        {
                            string referenceSceneKey = scene.ReferenceScenes[i];
                            if (!GetScene(referenceSceneKey, out SceneBase rScene))
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

                _inputHandler = (scene as IScene).InputHandler ?? scene;

                lock (_currentScenes)
                {
                    _currentScenes.Add(scene);
                }

                return scene.State == SceneState.Ready ? ShowSceneResult.Success : ShowSceneResult.NotReady;
            }
        }

        /// <inheritdoc />
        public ShowSceneResult ShowScene(string key, out SceneBase scene)
        {
            return !_scenes.TryGetValue(key, out scene) ? ShowSceneResult.NoScene : ShowScene(scene);
        }

        /// <inheritdoc />
        public bool GetScene(string key, out SceneBase scene)
        {
            return _scenes.TryGetValue(key, out scene);
        }

        /// <inheritdoc />
        public SceneState GetSceneState(string key)
        {
            if (_scenes.TryGetValue(key, out SceneBase scene))
            {
                return scene.State;
            }
            throw new NullReferenceException("no scene with key: '" + key + "' found.");
        }

        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            lock (_currentUpdateableScenes)
            {
                _currentUpdateableScenes.AddRange(_currentScenes);
            }

            for (int i = _currentUpdateableScenes.Count - 1; i >= 0; i--)
            {
                IScene scene = _currentUpdateableScenes[i];
                if (scene.Enabled && scene.State == SceneState.Ready)
                {
                    scene.Update(gameTime);
                }
            }

            _currentUpdateableScenes.Clear();
        }

        /// <inheritdoc />
        public override void Draw(GameTime gameTime)
        {
            lock (_currentDrawableScenes)
            {
                _currentDrawableScenes.AddRange(_currentScenes);
            }
            for (int i = 0; i < _currentDrawableScenes.Count; i++)
            {
                IScene scene = _currentDrawableScenes[i];
                if (scene.State == SceneState.Ready)
                {
                    if (scene.BeginDraw())
                    {
                        scene.Draw(gameTime);
                        scene.EndDraw();
                    }
                }
            }

            _currentDrawableScenes.Clear();
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _registry = registry;
            _input = registry.GetService<IInputDevice>() ?? throw new NullReferenceException("No IInputDevice found.");

            _input.MouseMove += Input_MouseMove;
            _input.MouseDown += Input_MouseDown;
            _input.MouseUp += Input_MouseUp;
            _input.MouseWheel += Input_MouseWheel;

            _input.KeyDown += Input_KeyDown;
            _input.KeyUp += Input_KeyUp;
            _input.KeyPress += Input_KeyPress;

            lock (_pendingInitializableScenes)
            {
                //START SCENE
                _pendingInitializableScenes[0].Initialize(registry);
                _pendingInitializableScenes[0].LoadContent();
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
                _currentScenes.Clear();

                _input.MouseMove -= Input_MouseMove;
                _input.MouseDown -= Input_MouseDown;
                _input.MouseUp -= Input_MouseUp;
                _input.MouseWheel -= Input_MouseWheel;

                _input.KeyDown -= Input_KeyDown;
                _input.KeyUp -= Input_KeyUp;
                _input.KeyPress -= Input_KeyPress;

                foreach (IScene scene in _scenes.Values)
                {
                    scene.UnloadContent();
                    scene.Dispose();
                }

                _scenes.Clear();
            }
        }

        private bool HideScene(SceneBase scene)
        {
            lock (_currentScenes)
            {
                if (!_currentScenes.Remove(scene)) { return false; }

                if (_currentScenes.Count > 0)
                {
                    _inputHandler = (_currentScenes[0] as IScene).InputHandler ?? _currentScenes[0];
                }
                else
                {
                    _inputHandler = null;
                }
                return true;
            }
        }

        #region Input Handler

        private void Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _inputHandler?.Input_MouseMove(x, y, buttons, clicks, wheelDelta);
        }

        private void Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _inputHandler?.Input_MouseDown(x, y, buttons, clicks, wheelDelta);
        }

        private void Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _inputHandler?.Input_MouseUp(x, y, buttons, clicks, wheelDelta);
        }

        private void Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _inputHandler?.Input_MouseClick(x, y, buttons, clicks, wheelDelta);
        }

        private void Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _inputHandler?.Input_MouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        private void Input_KeyPress(char key)
        {
            _inputHandler?.Input_KeyPress(key);
        }

        private void Input_KeyUp(int keyValue, bool shift, bool alt, bool ctrl)
        {
            _inputHandler?.Input_KeyUp(keyValue, shift, alt, ctrl);
        }

        private void Input_KeyDown(int keyValue, bool shift, bool alt, bool ctrl)
        {
            _inputHandler?.Input_KeyDown(keyValue, shift, alt, ctrl);
        }

        #endregion
    }
}
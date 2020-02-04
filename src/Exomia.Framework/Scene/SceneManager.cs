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
using System.Windows.Forms;
using Exomia.Framework.Game;
using Exomia.Framework.Input;
using MouseButtons = Exomia.Framework.Input.MouseButtons;

namespace Exomia.Framework.Scene
{
    /// <summary>
    ///     Manager for scenes. This class cannot be inherited.
    /// </summary>
    public sealed class SceneManager : DrawableComponent, ISceneManager
    {
        /// <summary>
        ///     Initial size of the queue.
        /// </summary>
        private const int INITIAL_QUEUE_SIZE = 16;

        /// <summary>
        ///     The current drawable scenes.
        /// </summary>
        private readonly List<ISceneInternal> _currentDrawableScenes;

        /// <summary>
        ///     The current scenes.
        /// </summary>
        private readonly List<ISceneInternal> _currentScenes;

        /// <summary>
        ///     The current updateable scenes.
        /// </summary>
        private readonly List<ISceneInternal> _currentUpdateableScenes;

        /// <summary>
        ///     The pending initializable scenes.
        /// </summary>
        private readonly List<ISceneInternal> _pendingInitializableScenes;

        /// <summary>
        ///     The scenes.
        /// </summary>
        private readonly Dictionary<string, ISceneInternal> _scenes;

        /// <summary>
        ///     The scenes to unload.
        /// </summary>
        private readonly List<ISceneInternal> _scenesToUnload;

        /// <summary>
        ///     The raw input device.
        /// </summary>
        private IRawInputDevice? _rawInputDevice;

        /// <summary>
        ///     The input handler.
        /// </summary>
        private IRawInputHandler? _rawInputHandler;

        /// <summary>
        ///     The registry.
        /// </summary>
        private IServiceRegistry? _registry;

        /// <summary>
        ///     The key modifier.
        /// </summary>
        private KeyModifier _keyModifier = 0;

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

                scene.Show(_currentScenes.Count > 0 ? _currentScenes[_currentScenes.Count - 1] : null, payload);

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

                _rawInputHandler = scene.RawInputHandler ?? scene;

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

                if (_currentScenes.Count > 0)
                {
                    _rawInputHandler = _currentScenes[_currentScenes.Count - 1].RawInputHandler ??
                                       _currentScenes[_currentScenes.Count - 1];
                }
                else
                {
                    _rawInputHandler = null;
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
            _registry = registry;
            _rawInputDevice = registry.GetService<IRawInputDevice>() ??
                              throw new NullReferenceException($"No {nameof(IRawInputDevice)} found.");

            _rawInputDevice.RawKeyEvent   += RawInputDeviceOnRawKeyEvent;
            _rawInputDevice.RawMouseDown  += RawInputDeviceOnRawMouseDown;
            _rawInputDevice.RawMouseUp    += RawInputDeviceOnRawMouseUp;
            _rawInputDevice.RawMouseClick += RawInputDeviceOnRawMouseClick;
            _rawInputDevice.RawMouseMove  += RawInputDeviceOnRawMouseMove;
            _rawInputDevice.RawMouseWheel += RawInputDeviceOnRawMouseWheel;

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

                _rawInputDevice!.RawKeyEvent  -= RawInputDeviceOnRawKeyEvent;
                _rawInputDevice.RawMouseDown  -= RawInputDeviceOnRawMouseDown;
                _rawInputDevice.RawMouseUp    -= RawInputDeviceOnRawMouseUp;
                _rawInputDevice.RawMouseMove  -= RawInputDeviceOnRawMouseMove;
                _rawInputDevice.RawMouseWheel -= RawInputDeviceOnRawMouseWheel;

                foreach (ISceneInternal scene in _scenes.Values)
                {
                    scene.UnloadContent(_registry!);
                    scene.Dispose();
                }

                _scenes.Clear();
            }
        }

        /// <summary>
        ///     Raw input device on raw mouse wheel.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _rawInputHandler?.Input_MouseWheel(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse move.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _rawInputHandler?.Input_MouseMove(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse up.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _rawInputHandler?.Input_MouseUp(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse down.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _rawInputHandler?.Input_MouseDown(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw mouse click.
        /// </summary>
        /// <param name="x">          The x coordinate. </param>
        /// <param name="y">          The y coordinate. </param>
        /// <param name="buttons">    The buttons. </param>
        /// <param name="clicks">     The clicks. </param>
        /// <param name="wheelDelta"> The wheelDelta. </param>
        private void RawInputDeviceOnRawMouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta)
        {
            _rawInputHandler?.Input_MouseClick(x, y, buttons, clicks, wheelDelta);
        }

        /// <summary>
        ///     Raw input device on raw key event.
        /// </summary>
        /// <param name="e"> [in,out] The ref Message to process. </param>
        private void RawInputDeviceOnRawKeyEvent(ref Message e)
        {
            _rawInputHandler?.Input_KeyEvent(ref e);
            if (_rawInputHandler is IInputHandler inputHandler)
            {
                int vKey = (int)e.WParam.ToInt64();

                switch (e.Msg)
                {
                    case Win32Message.WM_KEYDOWN:
                        switch (vKey)
                        {
                            case Key.ShiftKey:
                                _keyModifier |= KeyModifier.Shift;
                                break;
                            case Key.ControlKey:
                                _keyModifier |= KeyModifier.Control;
                                break;
                        }
                        inputHandler.Input_KeyDown(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_KEYUP:
                        switch (vKey)
                        {
                            case Key.ShiftKey:
                                _keyModifier &= ~KeyModifier.Shift;
                                break;
                            case Key.ControlKey:
                                _keyModifier &= ~KeyModifier.Control;
                                break;
                        }
                        inputHandler.Input_KeyUp(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_SYSKEYDOWN:
                        if (vKey == Key.Menu)
                        {
                            _keyModifier |= KeyModifier.Alt;
                        }
                        inputHandler.Input_KeyDown(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_SYSKEYUP:
                        if (vKey == Key.Menu)
                        {
                            _keyModifier &= ~KeyModifier.Alt;
                        }
                        inputHandler.Input_KeyUp(vKey, _keyModifier);
                        break;
                    case Win32Message.WM_UNICHAR:
                    case Win32Message.WM_CHAR:
                        inputHandler.Input_KeyPress((char)vKey);
                        break;
                }
            }
        }
    }
}
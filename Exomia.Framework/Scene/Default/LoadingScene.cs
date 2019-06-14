#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
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

using System;
using System.Threading.Tasks;

namespace Exomia.Framework.Scene.Default
{
    /// <summary>
    ///     A loading scene.
    /// </summary>
    public class LoadingScene : SceneBase
    {
        /// <summary>
        ///     The scene to load.
        /// </summary>
        private readonly SceneBase _sceneToLoad;

        /// <summary>
        ///     The registry.
        /// </summary>
        private IServiceRegistry _registry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadingScene" /> class.
        /// </summary>
        /// <param name="key">         . </param>
        /// <param name="sceneToLoad"> . </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public LoadingScene(string key, SceneBase sceneToLoad)
            : base(key)
        {
            _sceneToLoad = sceneToLoad ?? throw new ArgumentNullException(nameof(sceneToLoad));
        }

        /// <inheritdoc />
        protected override void OnInitialize(IServiceRegistry registry)
        {
            _registry = registry;
        }

        /// <inheritdoc />
        protected override void OnShow(SceneBase comingFrom, object[] payload)
        {
            if (_sceneToLoad.State == SceneState.None)
            {
                _sceneToLoad.SceneStateChanged += _sceneToLoad_SceneStateChanged;
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.Initialize(_registry);
                    });
            }
            else if (_sceneToLoad.State == SceneState.StandBy)
            {
                _sceneToLoad.SceneStateChanged += _sceneToLoad_SceneStateChanged;
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.LoadContent(_registry);
                    });
            }
        }

        /// <summary>
        ///     Scene to load scene state changed.
        /// </summary>
        /// <param name="scene">   The scene. </param>
        /// <param name="current"> The current. </param>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        private void _sceneToLoad_SceneStateChanged(IScene scene, SceneState current)
        {
            if (current == SceneState.StandBy)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.LoadContent(_registry);
                    });
            }
            else if (current == SceneState.Ready)
            {
                _sceneToLoad.SceneStateChanged -= _sceneToLoad_SceneStateChanged;

                if (SceneManager.ShowScene(_sceneToLoad) != ShowSceneResult.Success)
                {
                    throw new Exception($"can't show scene: '{_sceneToLoad.Key}' | State: {_sceneToLoad.State}");
                }
            }
        }
    }
}
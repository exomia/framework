#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

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
        private IServiceRegistry? _registry;

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
        protected override void OnShow(SceneBase? comingFrom, object[] payload)
        {
            if (_sceneToLoad.State == SceneState.None)
            {
                _sceneToLoad.SceneStateChanged += SceneToLoad_SceneStateChanged;
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.Initialize(_registry!);
                    });
            }
            else if (_sceneToLoad.State == SceneState.StandBy)
            {
                _sceneToLoad.SceneStateChanged += SceneToLoad_SceneStateChanged;
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.LoadContent(_registry!);
                    });
            }
        }

        /// <summary>
        ///     Scene to load scene state changed.
        /// </summary>
        /// <param name="scene">   The scene. </param>
        /// <param name="current"> The current. </param>
        /// <exception cref="Exception"> Thrown when an exception error condition occurs. </exception>
        private void SceneToLoad_SceneStateChanged(IScene scene, SceneState current)
        {
            if (current == SceneState.StandBy)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.LoadContent(_registry!);
                    });
            }
            else if (current == SceneState.Ready)
            {
                _sceneToLoad.SceneStateChanged -= SceneToLoad_SceneStateChanged;

                if (SceneManager.ShowScene(_sceneToLoad) != ShowSceneResult.Success)
                {
                    throw new Exception($"can't show scene: '{_sceneToLoad.Key}' | State: {_sceneToLoad.State}");
                }
            }
        }
    }
}
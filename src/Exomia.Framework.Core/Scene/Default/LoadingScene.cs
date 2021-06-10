#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.Threading.Tasks;

namespace Exomia.Framework.Core.Scene.Default
{
    /// <summary> A loading scene. </summary>
    public class LoadingScene : SceneBase
    {
        private readonly SceneBase _sceneToLoad;

        /// <summary> Initializes a new instance of the <see cref="LoadingScene" /> class. </summary>
        /// <param name="key">         The key. </param>
        /// <param name="sceneToLoad"> The scene to load. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        public LoadingScene(string key, SceneBase sceneToLoad)
            : base(key)
        {
            _sceneToLoad = sceneToLoad ?? throw new ArgumentNullException(nameof(sceneToLoad));
        }

        /// <inheritdoc/>
        protected override void OnShow(SceneBase? comingFrom, object[] payload)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (_sceneToLoad.State)
            {
                case SceneState.None:
                    _sceneToLoad.SceneStateChanged += SceneToLoad_SceneStateChanged;
                    Task.Factory.StartNew(_sceneToLoad.Initialize);
                    break;
                case SceneState.StandBy:
                    _sceneToLoad.SceneStateChanged += SceneToLoad_SceneStateChanged;
                    Task.Factory.StartNew(_sceneToLoad.LoadContent);
                    break;
            }
        }

        private void SceneToLoad_SceneStateChanged(SceneBase scene, SceneState current)
        {
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (current)
            {
                case SceneState.StandBy:
                    Task.Factory.StartNew(_sceneToLoad.LoadContent);
                    break;
                case SceneState.Ready:
                {
                    _sceneToLoad.SceneStateChanged -= SceneToLoad_SceneStateChanged;
                    if (_sceneManager!.ShowScene(_sceneToLoad) != ShowSceneResult.Success)
                    {
                        throw new Exception(
                            $"can't show scene: '{_sceneToLoad.Key}' | State: {_sceneToLoad.State}");
                    }
                    break;
                }
            }
        }
    }
}
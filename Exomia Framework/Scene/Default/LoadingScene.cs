using System;
using System.Threading.Tasks;

namespace Exomia.Framework.Scene.Default
{
    /// <summary>
    ///     LoadingScene class
    /// </summary>
    public class LoadingScene : SceneBase
    {
        #region Variables

        #region Statics

        #endregion

        private readonly SceneBase _sceneToLoad;

        #endregion

        #region Constructors

        #region Statics

        #endregion

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadingScene" /> class.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sceneToLoad"></param>
        public LoadingScene(string key, SceneBase sceneToLoad)
            : base(key)
        {
            _sceneToLoad = sceneToLoad ?? throw new ArgumentNullException(nameof(sceneToLoad));
        }

        #endregion

        #region Constants

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        #endregion

        /// <summary>
        ///     <see cref="SceneBase.Show" />
        /// </summary>
        public override void Show()
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
                        _sceneToLoad.LoadContent();
                    });
            }
        }

        private void _sceneToLoad_SceneStateChanged(IScene scene, SceneState current)
        {
            if (current == SceneState.StandBy)
            {
                Task.Factory.StartNew(
                    () =>
                    {
                        _sceneToLoad.LoadContent();
                    });
            }
            else if (current == SceneState.Ready)
            {
                _sceneToLoad.SceneStateChanged -= _sceneToLoad_SceneStateChanged;

                if (_sceneManager.ShowScene(_sceneToLoad) != ShowSceneResult.Success)
                {
                    throw new Exception("can't show scene: '" + _sceneToLoad.Key + "' | State: " + _sceneToLoad.State);
                }
            }
        }

        #endregion
    }
}
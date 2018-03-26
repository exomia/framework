#pragma warning disable 1591

namespace Exomia.Framework.Scene
{
    public enum ShowSceneResult
    {
        NoScene,
        NotReady,
        Success
    }

    public interface ISceneManager
    {
        bool AddScene(SceneBase scene, bool initialize = true);

        bool RemoveScene(string key);

        bool HideScene(string key);

        ShowSceneResult ShowScene(SceneBase scene);
        ShowSceneResult ShowScene(string key, out SceneBase scene);

        bool GetScene(string key, out SceneBase scene);

        SceneState GetSceneState(string key);
    }
}
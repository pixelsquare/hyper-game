using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public interface ISceneLoader
    {
        ILoadingScreen LoadScene(LoadingScreenType screenType, string sceneToLoad, string sceneToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded);
        ILoadingScreen LoadScene(LoadingScreenType screenType, string[] scenesToLoad, string[] scenesToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded);

        void UnloadScene(string sceneToUnload, UnityAction onSceneUnloaded);
        ILoadingScreen UnloadScene(LoadingScreenType screenType, string sceneToUnload, UnityAction onScenesUnloaded);

        void PurgeScenes(UnityAction onSceneUnloaded);

        void LoadSceneAsAdditive(string sceneName);
        void LoadSceneAsAdditive(string sceneName, UnityAction onScenesLoaded);
        bool IsSceneActive(string sceneName);
        void SetActiveScene(string sceneName);
        bool IsSceneLoading(string sceneName);
        bool IsBusy { get; }
    }
}

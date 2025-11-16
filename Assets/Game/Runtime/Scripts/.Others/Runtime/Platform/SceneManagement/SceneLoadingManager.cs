using System.Collections.Generic;
using Kumu.Kulitan.Multiplayer;
using UniRx;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Common
{
    public class SceneLoadingManager : SingletonMonoBehaviour<SceneLoadingManager>
    {
#if ADDRESSABLES_ENABLED
        private readonly ISceneLoader sceneLoader = new SceneLoaderAddressables();
#else
        private readonly ISceneLoader sceneLoader = new SceneLoaderDefault();
#endif
        
        private readonly ISubject<string> _onSceneSetToActive = new Subject<string>();

        public bool IsBusy => sceneLoader.IsBusy || HangoutGameLoader.Instance.IsBusy;

        public ISubject<string> OnSceneSetToActive => _onSceneSetToActive;

        /// <summary>
        ///     Unloads and loads a new scene.
        ///     Displays a generic loading screen.
        /// </summary>
        /// <param name="sceneToLoad">New scene to load</param>
        /// <param name="sceneToUnload">Previous scene to unload</param>
        /// <param name="autoHideLoadingScreen">Set this to false if manually hiding the loading screen</param>
        /// <param name="onScenesLoaded">Callback</param>
        /// <returns></returns>
        public ILoadingScreen LoadScene(string sceneToLoad, string sceneToUnload, bool autoHideLoadingScreen = true,
                                        UnityAction onScenesLoaded = null)
        {
            return LoadScene(LoadingScreenType.GenericLoadingBar, sceneToLoad, sceneToUnload, autoHideLoadingScreen,
                    onScenesLoaded);
        }

        /// <summary>
        ///     Unloads and loads array of scenes.
        ///     Displays a generic loading screen.
        /// </summary>
        /// <param name="scenesToLoad">Array of scenes to load</param>
        /// <param name="scenesToUnload">Array of scenes to unload</param>
        /// <param name="autoHideLoadingScreen">Set this to false if manually hiding the loading screen</param>
        /// <param name="onScenesLoaded">Callback</param>
        public ILoadingScreen LoadScene(string[] scenesToLoad, string[] scenesToUnload,
                                        bool autoHideLoadingScreen = true, UnityAction onScenesLoaded = null)
        {
            return LoadScene(LoadingScreenType.GenericLoadingBar, scenesToLoad, scenesToUnload, autoHideLoadingScreen,
                    onScenesLoaded);
        }

        /// <summary>
        ///     Unloads and loads a new scene.
        /// </summary>
        /// <param name="screenType">Loading screen type to use</param>
        /// <param name="sceneToLoad">New scene to load</param>
        /// <param name="sceneToUnload">Previous scene to unload</param>
        /// <param name="autoHideLoadingScreen">Set this to false if manually hiding the loading screen</param>
        /// <param name="onScenesLoaded">Callback</param>
        public ILoadingScreen LoadScene(LoadingScreenType screenType, string sceneToLoad, string sceneToUnload,
                                        bool autoHideLoadingScreen = true, UnityAction onScenesLoaded = null)
        {
            return sceneLoader.LoadScene(screenType, sceneToLoad, sceneToUnload, autoHideLoadingScreen, onScenesLoaded);
        }

        /// <summary>
        ///     Unloads and loads a many scenes.
        /// </summary>
        /// <param name="screenType">Loading screen type to use</param>
        /// <param name="scenesToLoad">Array of scenes to load</param>
        /// <param name="scenesToUnload">Array of scenes to unload</param>
        /// <param name="autoHideLoadingScreen">Set this to false if manually hiding the loading screen</param>
        /// <param name="onScenesLoaded">Callback</param>
        public ILoadingScreen LoadScene(LoadingScreenType screenType, string[] scenesToLoad, string[] scenesToUnload,
                                        bool autoHideLoadingScreen = true, UnityAction onScenesLoaded = null)
        {
            return sceneLoader.LoadScene(screenType, scenesToLoad, scenesToUnload, autoHideLoadingScreen,
                    onScenesLoaded);
        }

        public void UnloadScene(string sceneToUnload, UnityAction onSceneUnloaded = null)
        {
            sceneLoader.UnloadScene(sceneToUnload, onSceneUnloaded);
        }

        public ILoadingScreen UnloadScene(LoadingScreenType screenType, string sceneToUnload,
                                          UnityAction onScenesUnloaded = null)
        {
            return sceneLoader.UnloadScene(screenType, sceneToUnload, onScenesUnloaded);
        }

        public void PurgeScenes(UnityAction onScenesUnloaded = null)
        {
            sceneLoader.PurgeScenes(onScenesUnloaded);
        }

        /// <summary>
        ///     Unloads any additive scene.
        ///     Will not show any loading screens.
        /// </summary>
        public void UnloadAdditiveScene(string sceneToUnload)
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        /// <summary>
        ///     Loads an additional scene on top of the current scene.
        ///     Will not show any loading screens.
        /// </summary>
        public void LoadSceneAsAdditive(string sceneName)
        {
            sceneLoader.LoadSceneAsAdditive(sceneName);
        }

        /// <summary>
        ///     Loads an additional scene on top of the current scene.
        ///     Will not show any loading screens.
        ///     Includes a callback when scene has loaded.
        /// </summary>
        public void LoadSceneAsAdditive(string sceneName, UnityAction onSceneLoaded)
        {
            sceneLoader.LoadSceneAsAdditive(sceneName, onSceneLoaded);
        }

        public bool IsSceneActive(string sceneName)
        {
            return sceneLoader.IsSceneActive(sceneName);
        }

        public string GetActiveSceneName()
        {
            return SceneManager.GetActiveScene().name;
        }

        public void SetActiveScene(string sceneName)
        {
            sceneLoader.SetActiveScene(sceneName);
            _onSceneSetToActive.OnNext(sceneName);
        }

        public bool IsSceneLoading(string sceneName)
        {
            return sceneLoader.IsSceneLoading(sceneName);
        }

        public void LoadMainScreen(UnityAction onScenesLoaded = null, bool unloadLastScenes = false,
                                   bool autoHideLoadingScreen = true)
        {
            if (IsSceneLoading(SceneNames.MAIN_SCREEN)
             || IsSceneLoaded(SceneNames.MAIN_SCREEN)
             || IsSceneActive(SceneNames.MAIN_SCREEN))
            {
                return;
            }

            if (unloadLastScenes)
            {
                PurgeScenes();
            }

#if MOCK_SOCIALS && USES_MOCKS
            var scenesToLoad = new[] { SceneNames.MAIN_SCREEN, SceneNames.AVATAR_CUSTOMIZATION, SceneNames.USER_PROFILE };
#else
            var scenesToLoad = new[] { SceneNames.MAIN_SCREEN, SceneNames.AVATAR_CUSTOMIZATION, SceneNames.USER_PROFILE_SCREEN };
#endif

            onScenesLoaded += () => SetActiveScene(SceneNames.MAIN_SCREEN);
            LoadScene(scenesToLoad, null, autoHideLoadingScreen, onScenesLoaded);
        }

        /// <summary>
        ///     Unloads all scenes apart from the Persistent scene,
        ///     before loading the MainScreen.
        /// </summary>
        /// <param name="onScenesLoaded">callback once main screen is loaded</param>
        /// <param name="autoHideLoadingScreen">setting to false would need to manually hide loading screen</param>
        public ILoadingScreen LoadMainScreenOnly(UnityAction onScenesLoaded = null, bool autoHideLoadingScreen = false, bool ignoreChecks = false)
        {
            var isSceneLoading = IsSceneLoading(SceneNames.MAIN_SCREEN);
            var isSceneLoaded = IsSceneLoaded(SceneNames.MAIN_SCREEN);
            var isSceneActive = IsSceneActive(SceneNames.MAIN_SCREEN);
            
            if (!ignoreChecks)
            {
                if (isSceneLoading || isSceneLoaded || isSceneActive)
                {
                    return null;
                }
            }

            var sceneCount = SceneManager.sceneCount;
            var scenesToUnload = new List<string>();

            for (var i = 0; i < sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.name.Equals(SceneNames.PERSISTENT_SCENE))
                {
                    scenesToUnload.Add(scene.name);
                }
            }

            var scenesToLoad = new[] { SceneNames.MAIN_SCREEN, SceneNames.AVATAR_CUSTOMIZATION, SceneNames.USER_PROFILE_SCREEN };

#if MOCK_SOCIALS && USES_MOCKS
            return LoadScene(scenesToLoad, scenesToUnload.ToArray(), autoHideLoadingScreen,
                    () =>
                    {
                        SetActiveScene(SceneNames.MAIN_SCREEN);
                        onScenesLoaded?.Invoke();
                    });
#else
            return LoadScene(scenesToLoad, scenesToUnload.ToArray(), autoHideLoadingScreen,
                    () =>
                    {
                        SetActiveScene(SceneNames.MAIN_SCREEN);
                        onScenesLoaded?.Invoke();
                    });
#endif
        }

        public void LoadHangoutSharedScene(UnityAction onScenesLoaded, bool hasMinigame = false)
        {
            if (hasMinigame)
            {
                LoadScene(LoadingScreenType.GenericLoadingBar, new[]
                                { SceneNames.HANGOUT_SHARED_SCENE, SceneNames.HANGOUT_MINIGAME_SHARED_SCENE }
                        , null, true, () =>
                        {
                            onScenesLoaded.Invoke();
                        });
                return;
            }

            LoadScene(LoadingScreenType.GenericLoadingBar, SceneNames.HANGOUT_SHARED_SCENE, null, true, () =>
            {
                onScenesLoaded.Invoke();
            });
        }

        public bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        public Scene GetScene(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Kumu.Kulitan.Common
{
    public class SceneLoaderDefault : ISceneLoader
    {
        private event UnityAction OnSceneOpFinished = delegate { };

        private List<AsyncOperation> sceneOperations = new();
        private List<string> lastScenesLoaded = new();

        public ILoadingScreen LoadScene(LoadingScreenType screenType, string sceneToLoad, string sceneToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded)
        {
            OnSceneOpFinished = onScenesLoaded;
            sceneOperations.Clear();

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                sceneOperations.Add(SceneManager.UnloadSceneAsync(sceneToUnload));
            }

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                sceneOperations.Add(SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive));
            }

            AddLastSceneLoaded(sceneToLoad);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressRoutine(autoHideLoadingScreen));

            return loadingScreen;
        }

        public ILoadingScreen LoadScene(LoadingScreenType screenType, string[] scenesToLoad, string[] scenesToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded)
        {
            OnSceneOpFinished = onScenesLoaded;
            sceneOperations.Clear();

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (scenesToUnload != null)
            {
                foreach (string scene in scenesToUnload)
                {
                    sceneOperations.Add(SceneManager.UnloadSceneAsync(scene));
                }
            }

            if (scenesToLoad != null)
            {
                foreach (string scene in scenesToLoad)
                {
                    sceneOperations.Add(SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive));
                }
            }

            AddLastSceneLoaded(scenesToLoad);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressRoutine(autoHideLoadingScreen));

            return loadingScreen;
        }

        public void UnloadScene(string sceneToUnload, UnityAction onSceneUnloaded)
        {
            OnSceneOpFinished = onSceneUnloaded;
            sceneOperations.Clear();

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                sceneOperations.Add(SceneManager.UnloadSceneAsync(sceneToUnload));
            }

            RemoveFromLastSceneLoaded(sceneToUnload);
        }

        public ILoadingScreen UnloadScene(LoadingScreenType screenType, string sceneToUnload, UnityAction onScenesUnloaded)
        {
            OnSceneOpFinished = onScenesUnloaded;
            sceneOperations.Clear();

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                sceneOperations.Add(SceneManager.UnloadSceneAsync(sceneToUnload));
            }

            RemoveFromLastSceneLoaded(sceneToUnload);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressRoutine(false));

            return loadingScreen;
        }

        public void PurgeScenes(UnityAction onSceneUnloaded)
        {
            OnSceneOpFinished = onSceneUnloaded;

            for (var i = 0; i < lastScenesLoaded.Count; i++)
            {
                sceneOperations.Add(SceneManager.UnloadSceneAsync(lastScenesLoaded[i]));
            }
        }

        public void LoadSceneAsAdditive(string sceneName)
        {
            if (IsSceneActive(sceneName) || string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void LoadSceneAsAdditive(string sceneName, UnityAction onSceneLoaded)
        {
            if (IsSceneActive(sceneName) || string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            OnSceneOpFinished = onSceneLoaded;
            sceneOperations.Clear();

            sceneOperations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressRoutine());
        }

        public bool IsSceneActive(string sceneName)
        {
            return SceneManager.GetSceneByName(sceneName).isLoaded;
        }

        public void SetActiveScene(string sceneName)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }

        public bool IsSceneLoading(string sceneName)
        {
            return lastScenesLoaded.Contains(sceneName);
        }

        public bool IsBusy => CoroutineRunner.IsBusy;

        private void AddLastSceneLoaded(params string[] sceneNames)
        {
            lastScenesLoaded.Clear();

            foreach (string sceneName in sceneNames)
            {
                lastScenesLoaded.Add(sceneName);
            }
        }

        private void RemoveFromLastSceneLoaded(params string[] sceneNames)
        {
            foreach (string sceneName in sceneNames)
            {
                if (lastScenesLoaded.Contains(sceneName))
                {
                    lastScenesLoaded.Remove(sceneName);
                }
            }
        }

        private IEnumerator UpdateSceneLoadProgressRoutine(bool autoHideLoadingScreen = true)
        {
            for (var i = 0; i < sceneOperations.Count; i++)
            {
                while (!sceneOperations[i].isDone)
                {
                    var loadingProgress = 0f;

                    foreach (AsyncOperation operation in sceneOperations)
                    {
                        loadingProgress += operation.progress;
                    }

                    loadingProgress = (loadingProgress / sceneOperations.Count) * 100f;
                    LoadingScreenManager.Instance.UpdateCurrentLoadingProgress(loadingProgress);
                    yield return null;
                }
            }

            if (autoHideLoadingScreen)
            {
                // add a little delay before hiding loading screen
                yield return new WaitForSeconds(0.5f);
                LoadingScreenManager.Instance.HideLoadingScreen();
            }

            OnSceneOpFinished?.Invoke();
        }
    }
}

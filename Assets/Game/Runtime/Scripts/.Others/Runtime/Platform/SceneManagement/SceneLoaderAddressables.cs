#if ADDRESSABLES_ENABLED
using System.Collections;
using System.Collections.Generic;
using Kumu.Kulitan.CDN;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Kumu.Kulitan.Common
{
    public class SceneLoaderAddressables : ISceneLoader
    {
        private event UnityAction OnSceneOpFinished = delegate { };

        private readonly List<SceneInstance> scenesLoaded = new();
        private readonly List<CDNAsyncOperation> operations = new();
        private readonly List<string> lastScenesLoaded = new();
        private bool isInitialized;

        public ILoadingScreen LoadScene(LoadingScreenType screenType, string sceneToLoad, string sceneToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded)
        {
            OnSceneOpFinished = onScenesLoaded;
            operations.Clear();

            if (!string.IsNullOrEmpty(sceneToLoad) && TryLoadUnityScene(sceneToLoad))
            {
                return null;
            }

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                var sceneIdx = scenesLoaded.FindIndex(a => !string.IsNullOrEmpty(a.Scene.name) && a.Scene.name.Equals(sceneToUnload));
                operations.Add(UnloadSceneAsyncOp.Create(scenesLoaded[sceneIdx]));
                scenesLoaded.RemoveAt(sceneIdx);
            }

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                operations.Add(LoadSceneAsyncOp.Create(sceneToLoad, LoadSceneMode.Additive, true));
            }

            AddLastSceneLoaded(sceneToLoad);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadingRoutine(operations.ToArray(), OnSceneOpFinished, autoHideLoadingScreen));

            return loadingScreen;
        }

        public ILoadingScreen LoadScene(LoadingScreenType screenType, string[] scenesToLoad, string[] scenesToUnload, bool autoHideLoadingScreen, UnityAction onScenesLoaded)
        {
            OnSceneOpFinished = onScenesLoaded;
            operations.Clear();

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (scenesToUnload != null)
            {
                foreach (var scene in scenesToUnload)
                {
                    var sceneIdx = scenesLoaded.FindIndex(a => !string.IsNullOrEmpty(a.Scene.name) && a.Scene.name.Equals(scene));

                    if (sceneIdx < 0)
                    {
                        var s = SceneManager.GetSceneByName(scene);

                        if (s.IsValid() && s.isLoaded)
                        {
                            SceneManager.UnloadSceneAsync(s);
                        }
                    }
                    else
                    {
                        operations.Add(UnloadSceneAsyncOp.Create(scenesLoaded[sceneIdx]));
                        scenesLoaded.RemoveAt(sceneIdx);
                    }
                }
            }

            if (scenesToLoad != null)
            {
                foreach (var scene in scenesToLoad)
                {
                    operations.Add(LoadSceneAsyncOp.Create(scene, LoadSceneMode.Additive, true));
                }
            }

            AddLastSceneLoaded(scenesToLoad);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadingRoutine(operations.ToArray(), OnSceneOpFinished, autoHideLoadingScreen));

            return loadingScreen;
        }

        public async void UnloadScene(string sceneToUnload, UnityAction onSceneUnloaded)
        {
            OnSceneOpFinished = onSceneUnloaded;
            operations.Clear();

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                var sceneIdx = scenesLoaded.FindIndex(a => !string.IsNullOrEmpty(a.Scene.name) && a.Scene.name.Equals(sceneToUnload));
                await Addressables.UnloadSceneAsync(scenesLoaded[sceneIdx]).Task;
                scenesLoaded.RemoveAt(sceneIdx);
            }

            RemoveFromLastSceneLoaded(sceneToUnload);
            OnSceneOpFinished?.Invoke();
        }

        public ILoadingScreen UnloadScene(LoadingScreenType screenType, string sceneToUnload, UnityAction onScenesUnloaded)
        {
            OnSceneOpFinished = onScenesUnloaded;
            operations.Clear();

            var loadingScreen = LoadingScreenManager.Instance.ShowLoadingScreen(screenType);

            if (!string.IsNullOrEmpty(sceneToUnload))
            {
                var sceneIdx = scenesLoaded.FindIndex(a => !string.IsNullOrEmpty(a.Scene.name) && a.Scene.name.Equals(sceneToUnload));
                Addressables.UnloadSceneAsync(scenesLoaded[sceneIdx]);
                scenesLoaded.RemoveAt(sceneIdx);
            }

            RemoveFromLastSceneLoaded(sceneToUnload);
            CoroutineRunner.RunCoroutine(UpdateSceneLoadingRoutine(operations.ToArray(), OnSceneOpFinished, false));

            return loadingScreen;
        }

        public void PurgeScenes(UnityAction onScenesUnloaded)
        {
            OnSceneOpFinished = onScenesUnloaded;

            for (var i = 0; i < lastScenesLoaded.Count; i++)
            {
                var sceneIdx = scenesLoaded.FindIndex(a => !string.IsNullOrEmpty(a.Scene.name)
                     && a.Scene.name.Equals(lastScenesLoaded[i]));
                Addressables.UnloadSceneAsync(scenesLoaded[sceneIdx]);
                scenesLoaded.RemoveAt(sceneIdx);
            }
        }

        public void LoadSceneAsAdditive(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || IsSceneActive(sceneName) || TryLoadUnityScene(sceneName))
            {
                return;
            }

            Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        }

        public void LoadSceneAsAdditive(string sceneName, UnityAction onSceneLoaded)
        {
            if (IsSceneActive(sceneName) || string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            OnSceneOpFinished = onSceneLoaded;
            operations.Clear();

            operations.Add(LoadSceneAsyncOp.Create(sceneName, LoadSceneMode.Additive, true));
            CoroutineRunner.RunCoroutine(UpdateSceneLoadingRoutine(operations.ToArray(), OnSceneOpFinished));
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
            lastScenesLoaded.AddRange(sceneNames);
        }

        private void RemoveFromLastSceneLoaded(params string[] sceneNames)
        {
            foreach (var sceneName in sceneNames)
            {
                if (lastScenesLoaded.Contains(sceneName))
                {
                    lastScenesLoaded.Remove(sceneName);
                }
            }
        }

        private bool TryLoadUnityScene(string sceneToLoad)
        {
            if (!string.IsNullOrEmpty(sceneToLoad) && !isInitialized)
            {
                SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
                AddLastSceneLoaded(sceneToLoad);
                LoadingScreenManager.Instance.HideLoadingScreen();
                isInitialized = sceneToLoad.Equals(SceneNames.ASSET_LOADER);
                return true;
            }

            return false;
        }

        private IEnumerator UpdateSceneLoadingRoutine(CDNAsyncOperation[] ops, UnityAction onOpFinished, bool autoHideLoadingScreen = true)
        {
            var totalProgress = 0f;
            var operationsLen = ops.Length;

            LoadingScreenManager.Instance.UpdateCurrentLoadingProgress(0f);

            for (var i = 0; i < operationsLen; i++)
            {
                var lastProgress = 0f;
                var op = ops[i];
                op.StartOp();

                while (!op.IsDone)
                {
                    totalProgress += op.Progress - lastProgress;
                    lastProgress = op.Progress;
                    LoadingScreenManager.Instance.UpdateCurrentLoadingProgress(totalProgress / operationsLen * 100f);
                    yield return null;
                }

                if (op.Result != null && op.Result is SceneInstance sceneInstance
                 && !string.IsNullOrEmpty(sceneInstance.Scene.name))
                {
                    scenesLoaded.Add(sceneInstance);
                    yield return new WaitUntil(() => sceneInstance.Scene.IsValid() && sceneInstance.Scene.isLoaded);
                }
            }

            LoadingScreenManager.Instance.UpdateCurrentLoadingProgress(100f);

            if (autoHideLoadingScreen)
            {
                // add a little delay before hiding loading screen
                yield return new WaitForSeconds(0.5f);
                LoadingScreenManager.Instance.HideLoadingScreen();
            }

            GlobalNotifier.Instance?.Trigger(SceneLoadingEvent.EVENT_NAME);

            onOpFinished?.Invoke();
        }
    }
}
#endif

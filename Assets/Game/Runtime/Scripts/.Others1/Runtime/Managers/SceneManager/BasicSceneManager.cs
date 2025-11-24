using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santelmo.Rinsurv
{
    public class BasicSceneManager : ISceneManager
    {
        private readonly CancellationTokenSource cts = new();
        private readonly List<Scene> _loadedScenes = new();

        public ISceneOp LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentNullException("Null or empty scene name.", nameof(sceneName));
            }

            try
            {
                var task = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToUniTask();
                return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token)
                                             .ContinueWith(() => _loadedScenes.Add(SceneManager.GetSceneByName(sceneName))));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public ISceneOp LoadSceneAdditiveAsync(params string[] sceneNames)
        {
            if (sceneNames == null || sceneNames.Length == 0)
            {
                throw new ArgumentNullException("Null or empty scene names.", nameof(sceneNames));
            }

            try
            {
                var sceneNamesLen = sceneNames.Length;
                var taskOps = new UniTask[sceneNamesLen];

                for (var i = 0; i < sceneNamesLen; i++)
                {
                    var sceneName = sceneNames[i];
                    var task = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
                    taskOps[i] = UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token)
                                        .ContinueWith(() => _loadedScenes.Add(SceneManager.GetSceneByName(sceneName)));
                }

                return SceneOp.Create(UniTask.WhenAll(taskOps));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public ISceneOp UnloadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentNullException("Null or empty scene name.", nameof(sceneName));
            }

            try
            {
                var sceneIndex = _loadedScenes.FindIndex(x =>
                        string.Compare(sceneName, x.name, StringComparison.Ordinal) == 0);

                if (sceneIndex < 0)
                {
                    var unloadTask = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None)?.ToUniTask();
                    return SceneOp.Create(UniTask.RunOnThreadPool(() => unloadTask, cancellationToken: cts.Token));
                }

                var removedSceneName = _loadedScenes[sceneIndex];
                _loadedScenes.RemoveAt(sceneIndex);

                var task = SceneManager.UnloadSceneAsync(removedSceneName, UnloadSceneOptions.None)?.ToUniTask();
                return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token));
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}

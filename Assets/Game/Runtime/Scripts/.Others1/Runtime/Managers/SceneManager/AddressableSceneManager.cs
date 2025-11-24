using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Santelmo.Rinsurv
{
    public class AddressableSceneManager : ISceneManager
    {
        private ISceneManager ISceneManager => this;

        private readonly CancellationTokenSource cts = new();
        private readonly List<SceneInstance> _loadedScenes = new();

        public ISceneOp LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentNullException("Null or empty scene name.", nameof(sceneName));
            }

            try
            {
                if (!IsSceneValid(sceneName))
                {
                    var task = SceneManager.LoadSceneAsync(sceneName).ToUniTask();
                    return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token));
                }
                else
                {
                    var task = Addressables.LoadSceneAsync(sceneName).ToUniTask();
                    return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token)
                                                 .ContinueWith(x => _loadedScenes.Add(x)));
                }
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

                    if (!IsSceneValid(sceneName))
                    {
                        var task = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
                        taskOps[i] = UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token);
                    }
                    else
                    {
                        var task = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToUniTask();
                        taskOps[i] = UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token)
                                            .ContinueWith(x => _loadedScenes.Add(x));
                    }
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
                if (!ISceneManager.IsSceneLoaded(sceneName))
                {
                    return SceneOp.Create(UniTask.CompletedTask);
                }

                if (!IsSceneValid(sceneName))
                {
                    var task = SceneManager.UnloadSceneAsync(sceneName, UnloadSceneOptions.None)?.ToUniTask();
                    return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token));
                }
                else
                {
                    var sceneIndex = _loadedScenes.FindIndex(x =>
                            string.Compare(sceneName, x.Scene.name, StringComparison.Ordinal) == 0);

                    if (sceneIndex < 0)
                    {
                        throw new ArgumentException("Scene does not exist or loaded.", nameof(sceneName));
                    }

                    var removedSceneName = _loadedScenes[sceneIndex];
                    _loadedScenes.RemoveAt(sceneIndex);

                    var task = Addressables.UnloadSceneAsync(removedSceneName, UnloadSceneOptions.None).ToUniTask();
                    return SceneOp.Create(UniTask.RunOnThreadPool(() => task, cancellationToken: cts.Token));
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private bool IsSceneValid(string sceneName)
        {
            return Addressables.ResourceLocators.Any(x => x.Locate(sceneName, typeof(SceneInstance), out _));
        }
    }
}

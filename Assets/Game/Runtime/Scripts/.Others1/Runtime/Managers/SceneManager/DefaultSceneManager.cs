using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Santelmo.Rinsurv
{
    /// <summary>
    ///     Scene Manager used in UBE.
    ///     Which uses CoroutineRunner.
    /// </summary>
    public class DefaultSceneManager : ISceneManager
    {
        public static UnityAction<string[]> OnScenesLoaded = null;

        private readonly List<AsyncOperation> _sceneOperations = new();

        public ISceneOp LoadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentNullException("Null or empty scene name.", nameof(sceneName));
            }

            _sceneOperations.Clear();
            _sceneOperations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single));
            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressCoroutine(_sceneOperations.ToArray(), () =>
            {
                OnScenesLoaded?.Invoke(new[] { sceneName });
            }));

            return null;
        }

        public ISceneOp LoadSceneAdditiveAsync(params string[] sceneNames)
        {
            if (sceneNames == null || sceneNames.Length == 0)
            {
                throw new ArgumentNullException("Null or empty scene names.", nameof(sceneNames));
            }

            _sceneOperations.Clear();

            foreach (var sceneName in sceneNames)
            {
                _sceneOperations.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));
            }

            CoroutineRunner.RunCoroutine(UpdateSceneLoadProgressCoroutine(_sceneOperations.ToArray(), () =>
            {
                OnScenesLoaded?.Invoke(sceneNames);
            }));

            return null;
        }

        public ISceneOp UnloadSceneAsync(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                throw new ArgumentNullException("Null or empty scene name.", nameof(sceneName));
            }

            var scene = SceneManager.GetSceneByName(sceneName);

            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            _sceneOperations.Clear();
            _sceneOperations.Add(SceneManager.UnloadSceneAsync(sceneName));
            return null;
        }

        private IEnumerator UpdateSceneLoadProgressCoroutine(AsyncOperation[] sceneOperations, Action onSceneLoaded)
        {
            var progress = 0f;
            var sceneOps = new List<AsyncOperation>(sceneOperations);

            while (sceneOps.All(x => !x.isDone))
            {
                progress = sceneOps
                          .Sum(x => x.progress) /sceneOps.Count;
                yield return null;
            }

            progress = sceneOps
                      .Sum(x => x.progress) / sceneOps.Count;

            onSceneLoaded?.Invoke();
        }
    }
}

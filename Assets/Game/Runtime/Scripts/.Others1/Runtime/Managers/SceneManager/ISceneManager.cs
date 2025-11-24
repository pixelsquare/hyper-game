using UnityEngine.SceneManagement;

namespace Santelmo.Rinsurv
{
    public interface ISceneManager : IGlobalBinding
    {
        public ISceneOp LoadSceneAsync(string sceneName);

        public ISceneOp LoadSceneAdditiveAsync(params string[] sceneNames);

        public ISceneOp UnloadSceneAsync(string sceneName);

        public bool IsSceneActive(string sceneName)
        {
            return string.CompareOrdinal(sceneName, SceneManager.GetActiveScene().name) == 0;
        }

        public void SetSceneActive(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
        }

        public bool IsSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.isLoaded && scene.IsValid();
        }
    }
}

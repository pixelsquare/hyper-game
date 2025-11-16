using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string[] sceneToLoad;
        [SerializeField] private string[] sceneToUnload;

        public void LoadScene()
        {
            SceneLoadingManager.Instance.LoadScene(sceneToLoad, sceneToUnload);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive(sceneName);
        }

        public void UnloadScene(string sceneName)
        {
            SceneLoadingManager.Instance.UnloadAdditiveScene(sceneName);
        }
    }
}

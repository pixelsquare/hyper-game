using UnityEngine;
using UnityEngine.SceneManagement;

namespace Santelmo.Rinsurv
{
    public class TestSceneLoad : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        [SerializeField] private LoadSceneMode _loadSceneMode;
        
        private void Start()
        {
            SceneManager.LoadSceneAsync(_sceneName, _loadSceneMode);
        }
    }
}

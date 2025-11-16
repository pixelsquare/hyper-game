using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class PersistentSceneManager : MonoBehaviour
    {
        /// <summary>
        /// Used by FSM.
        /// </summary>
        public void LoadAssetLoader()
        {
            SceneLoadingManager.Instance.LoadScene(SceneNames.ASSET_LOADER, null, true,
                () => SceneLoadingManager.Instance.SetActiveScene((SceneNames.ASSET_LOADER)));
        }
        
        /// <summary>
        /// Used by FSM.
        /// </summary> 
        public void LoadSignUpScene()
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive("SignUpScreen");
        }
    }
}

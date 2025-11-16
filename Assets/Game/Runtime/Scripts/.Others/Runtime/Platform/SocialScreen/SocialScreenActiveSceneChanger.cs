using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(ActiveSceneChanger))]
    public class SocialScreenActiveSceneChanger : MonoBehaviour
    {
        private ActiveSceneChanger sceneChanger;
        
        public void ChangeActiveScene()
        {
            var sceneToMakeActive = SceneLoadingManager.Instance.IsSceneLoaded(SceneNames.SOCIAL_SCREEN) ? SceneNames.SOCIAL_SCREEN : SceneNames.USER_PROFILE_SCREEN;
            sceneChanger.ChangeActiveScene(sceneToMakeActive);
        }

        private void Awake()
        {
            sceneChanger = GetComponent<ActiveSceneChanger>();
        }
    }
}

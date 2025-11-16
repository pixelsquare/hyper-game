using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class SocialScreenLoader : MonoBehaviour
    {
        public void LoadSocialScreenScene()
        {
            if (SceneLoadingManager.Instance.IsSceneActive(SceneNames.SOCIAL_SCREEN))
            {
                return;
            }
            SceneLoadingManager.Instance.LoadSceneAsAdditive(SceneNames.SOCIAL_SCREEN);
        }

        public void ResetSocialScreen()
        {
            GlobalNotifier.Instance.Trigger(new ResetSocialScreenEvent());
        }
    }
}

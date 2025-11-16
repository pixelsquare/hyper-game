using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class AvatarSceneLoader : MonoBehaviour
    {
        public void LoadAvatarCustomizationScene()
        {
            if (SceneLoadingManager.Instance.IsSceneActive(SceneNames.AVATAR_CUSTOMIZATION))
            {
                return;
            }
            SceneLoadingManager.Instance.LoadSceneAsAdditive(SceneNames.AVATAR_CUSTOMIZATION);
        }

        public void ResetAvatarSelection()
        {
            GlobalNotifier.Instance.Trigger(new ResetAvatarSelectionEvent());
        }
    }
}

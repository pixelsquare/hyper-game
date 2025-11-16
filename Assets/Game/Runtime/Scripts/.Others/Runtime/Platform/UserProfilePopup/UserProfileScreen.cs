using Kumu.Kulitan.Avatar;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class UserProfileScreen : MonoBehaviour
    {
        [SerializeField] private RawImage avatarPreview;
        [SerializeField] private TMP_Text playerNameLabel;
        [SerializeField] private TMP_Text playerUserNameLabel;
        [SerializeField] private TMP_Text socialInfoLabel;

        [SerializeField] private UnityEvent onInventoryValuesChanged;

        public void Awake()
        {
            UserProfileLocalDataManager.Instance.UserProfileUpdated.Subscribe(Initialize).AddTo(this);
        }
        
        public void Initialize(UserProfile userProfile)
        {
            playerNameLabel.text = userProfile.nickName;
            playerUserNameLabel.text = userProfile.userName;
            SetSocialInfo(userProfile.FollowerCountToDisplay, userProfile.FollowingCountToDisplay);
        }

        public void ShowSocialScreen()
        {
            SceneLoadingManager.Instance.LoadSceneAsAdditive(SceneNames.SOCIAL_SCREEN, () =>
            {
                SceneLoadingManager.Instance.SetActiveScene(SceneNames.SOCIAL_SCREEN);
                GlobalNotifier.Instance.Trigger(new MenuPanelChangedEvent(SceneNames.SOCIAL_SCREEN));
            });
        }

        public void SetSocialInfo(int followers, int following)
        {
            socialInfoLabel.text = $"<b>{followers}</b> Followers\t<b>{following}</b> Following";
        }

        public void OnAvatarLoading(bool isLoading)
        {
            avatarPreview.enabled = !isLoading;
        }

        private void HandleInventoryValueChanged()
        {
            onInventoryValuesChanged?.Invoke();
        }

        private void OnEnable()
        {
            UserInventoryData.OnEquippedItemsUpdated += HandleInventoryValueChanged;
        }

        private void OnDisable()
        {
            UserInventoryData.OnEquippedItemsUpdated -= HandleInventoryValueChanged;
        }
    }
}

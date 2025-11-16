using Kumu.Kulitan.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class SettingsUserPanel : MonoBehaviour
    {
        [SerializeField] private RawImage userAvatarImg;
        [SerializeField] private TMP_Text userIdLabel;
        [SerializeField] private Button userProfileButton;
        [SerializeField] private Button followButton;

        public bool IsInitialized { get; private set; }

        private string accountId;
        private uint playerId;
        private HangoutSettingsPopup settingsPopup;

        public void Initialize(string accountId, uint playerId, string username)
        {
            this.accountId = accountId;
            this.playerId = playerId;
            userIdLabel.text = username;
            IsInitialized = true;
        }

        public void SetAvatarTexture(Texture avatar)
        {
            userAvatarImg.texture = avatar;
        }

        public void SetIsFollowed(bool isFollowed)
        {
            followButton.interactable = !isFollowed;
        }

        private void HandleUserProfileButtonClicked()
        {
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            PopupManager.Instance.OpenScenePopup(SceneNames.USER_PROFILE_POPUP, () =>
            {
                var scene = SceneLoadingManager.Instance.GetScene(SceneNames.USER_PROFILE_POPUP);
                Variables.Scene(scene).Set("AccountId", accountId);
                Variables.Scene(scene).Set("PlayerId", playerId);
                Variables.Scene(scene).Set("PopupState", UserProfilePopup.UserProfileState.Hangout);
                SceneLoadingManager.Instance.SetActiveScene(SceneNames.USER_PROFILE_POPUP);
                GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("InitUserProfile"));
                LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            });
        }

        private void HandleFollowButtonClicked()
        {
        }

        private void Awake()
        {
            settingsPopup = GetComponentInParent<HangoutSettingsPopup>();
        }

        private void OnEnable()
        {
            userProfileButton.onClick.AddListener(HandleUserProfileButtonClicked);
            followButton.onClick.AddListener(HandleFollowButtonClicked);
        }

        private void OnDisable()
        {
            IsInitialized = false;
            userProfileButton.onClick.RemoveListener(HandleUserProfileButtonClicked);
            followButton.onClick.RemoveListener(HandleFollowButtonClicked);
        }
    }
}

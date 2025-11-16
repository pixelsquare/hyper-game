using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.Social;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [IncludeInSettings(true)]
    public class UserProfilePopup : BasePopup
    {
        public enum UserProfileState { Hangout, Social }

        [SerializeField] private Canvas canvas;
        [SerializeField] private RawImage avatarPreview;
        [SerializeField] private TMP_Text socialInfoLabel;
        [SerializeField] private UserDetailsPanel userDetailsPanel;
        [SerializeField] private ReportUserButton reportUserButton;

        [Header("Action Button Panels")]
        [SerializeField] private GameObject blockReportPanel;
        [SerializeField] private GameObject blockedUserPanel;

        [Header("Action Buttons")]
        [SerializeField] private Button joinHangoutButton;
        [SerializeField] private Button followButton;
        [SerializeField] private Button unfollowButton;
        [SerializeField] private GameObject unblockUserButton;
        [SerializeField] private GameObject blockUserButton;

        private string lastActiveSceneName;
        private GameObject mainCameraObj;
        private UserProfileState userProfileState;
        private OtherUserProfile otherUserProfile;

        public void Initialize(OtherUserProfile otherUserProfile)
        {
            this.otherUserProfile = otherUserProfile;

            var userProfile = otherUserProfile.profile;
            var socialState = otherUserProfile.social_state;

            userDetailsPanel.Initialize(userProfile, socialState);
            reportUserButton.Initialize(userProfile.accountId, userProfile.playerId);

            SetSocialInfo(userProfile.FollowerCountToDisplay, userProfile.FollowingCountToDisplay);

            var isBlocked = socialState.HasFlag(SocialState.Blocking);
            SetUserIsBlocked(isBlocked);

            if (!isBlocked)
            {
                SetUserIsFollowing(socialState.HasFlag(SocialState.Following) || socialState.HasFlag(SocialState.Friends));
                SetUserIsFriend(socialState.HasFlag(SocialState.Friends));
            }
            else
            {
                // Disabled social action buttons when blocked.
                joinHangoutButton.gameObject.SetActive(false);
                followButton.gameObject.SetActive(false);
                unfollowButton.gameObject.SetActive(false);
            }
        }

        public void SetUserProfileState(UserProfileState userProfileState)
        {
            this.userProfileState = userProfileState;

            if (userProfileState == UserProfileState.Social)
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 2;
            }
        }

        public void SetUserIsBlocked(bool isBlocked)
        {
            avatarPreview.color = isBlocked ? Color.black : Color.white;

            // Updated block/unblock action buttons.
            unblockUserButton.SetActive(isBlocked);
            blockUserButton.SetActive(!isBlocked);
            
            // Update block user / block report panels.
            blockedUserPanel.SetActive(isBlocked);
            blockReportPanel.SetActive(!isBlocked);
        }

        public void SetUserIsFollowing(bool isFollowing)
        {
            followButton.gameObject.SetActive(!isFollowing);
            unfollowButton.gameObject.SetActive(isFollowing);
        }

        public void SetUserIsFriend(bool isFriend)
        {
            var client = ConnectionManager.Client;
            var otherPlayerRoomId = otherUserProfile.room_id;

            var isSameHangout = client.InRoom && client.CurrentRoom.Name.Equals(otherPlayerRoomId);
            var otherPlayerRoomExist = !string.IsNullOrEmpty(otherUserProfile.room_id);

            var isJoiningAvailable = isFriend && !isSameHangout && otherPlayerRoomExist;
            joinHangoutButton.gameObject.SetActive(isJoiningAvailable);
        }

        public void SetSocialInfo(int followers, int following)
        {
            socialInfoLabel.text = $"<b>{followers}</b> Followers\t<b>{following}</b> Following";
        }

        public void OnAvatarLoading(bool isLoading)
        {
            avatarPreview.enabled = !isLoading;
        }

        public void ClosePopup()
        {
            SceneLoadingManager.Instance.SetActiveScene(lastActiveSceneName);

            PopupManager.Instance.RemoveActivePopup(this);
            OnClosed?.Invoke();
            PopupManager.Instance.CloseScenePopup(SceneNames.USER_PROFILE_POPUP);
        }

        private void HandleJoinRoomButtonClicked()
        {
            SocialManager.Instance.JoinRoomDirect(otherUserProfile.room_id);
            ClosePopup();
        }

        private async void HandleFollowButtonClicked()
        {
            var userProfile = otherUserProfile.profile;

            var request = new FollowUserRequest
            {
                userId = userProfile.accountId
            };

            var result = await Services.SocialService.FollowUserAsync(request);

            if (result.HasError)
            {
                // TODO: Do something ...
                PopupManager.Instance.OpenErrorPopup("Error", $"Failed to unfollow user. [{result.Error.Code}]", "Ok");
                return;
            }

            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("ReinitializeUserProfile"));
        }

        private async void HandleUnfollowButtonClicked()
        {
            var userProfile = otherUserProfile.profile;

            var request = new UnfollowUserRequest
            {
                userId = userProfile.accountId
            };

            var result = await Services.SocialService.UnfollowUserAsync(request);

            if (result.HasError)
            {
                // TODO: Do something ...
                PopupManager.Instance.OpenErrorPopup("Error", $"Failed to follow user. [{result.Error.Code}]", "Ok");
                return;
            }

            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("ReinitializeUserProfile"));
        }

        private void Awake()
        {
            lastActiveSceneName = SceneLoadingManager.Instance.GetActiveSceneName();
        }

        private void OnEnable()
        {
            joinHangoutButton.onClick.AddListener(HandleJoinRoomButtonClicked);
            followButton.onClick.AddListener(HandleFollowButtonClicked);
            unfollowButton.onClick.AddListener(HandleUnfollowButtonClicked);
        }

        private void OnDisable()
        {
            joinHangoutButton.onClick.RemoveListener(HandleJoinRoomButtonClicked);
            followButton.onClick.RemoveListener(HandleFollowButtonClicked);
            unfollowButton.onClick.RemoveListener(HandleUnfollowButtonClicked);
        }

        private void OnDestroy()
        {
            mainCameraObj?.SetActive(true);
        }

        private void Start()
        {
            mainCameraObj = GameObject.FindGameObjectWithTag("HangoutCamera");
            mainCameraObj?.SetActive(false);
        }
    }
}

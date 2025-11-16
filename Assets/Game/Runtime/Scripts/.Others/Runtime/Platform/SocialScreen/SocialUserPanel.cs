using System;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Social;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class SocialUserPanel : MonoBehaviour
    {
        [SerializeField] private Button panelButton;

        [SerializeField] private Image avatarImg;
        [SerializeField] private TMP_Text levelTxt;

        [SerializeField] private TMP_Text playerNameTxt;
        [SerializeField] private TMP_Text userNameTxt;
        [SerializeField] private Transform friendIcon;

        [Header("Favorite")]
        [SerializeField] private FavoriteButton favoriteBtn;

        [Header("Action Buttons")]
        [SerializeField] private Button followBtn;

        [SerializeField] private Button unfollowBtn;
        [SerializeField] private Button joinHangoutBtn;
        [SerializeField] private Transform actionLoadingIcon;

        [Header("UI For Spacer Mode")]
        [SerializeField] private Transform spacerContainer;

        [SerializeField] private TMP_Text spacerLabel;

        private SocialState socialState;
        private SocialScreen.PanelState panelState;
        private UserProfileForSocialScreen userProfileSocial;
        private Action onActionMadeCallback;
        private Action<bool> showSocialsLoading;
        
        private void OnEnable()
        {
            followBtn.onClick.AddListener(HandleFollowButtonClick);
            unfollowBtn.onClick.AddListener(HandleUnfollowButtonClick);
            joinHangoutBtn.onClick.AddListener(HandleJoinHangoutButtonClick);
            panelButton.onClick.AddListener(HandleUserPanelTap);
        }

        private void OnDisable()
        {
            followBtn.onClick.RemoveListener(HandleFollowButtonClick);
            unfollowBtn.onClick.RemoveListener(HandleUnfollowButtonClick);
            joinHangoutBtn.onClick.RemoveListener(HandleJoinHangoutButtonClick);
            panelButton.onClick.RemoveListener(HandleUserPanelTap);
        }

        public void Initialize(UserProfileForSocialScreen userProfileForSocialScreen)
        {
            if (userProfileForSocialScreen.IsSpacer)
            {
                spacerLabel.text = userProfileForSocialScreen.SpacerLabel;
                spacerContainer.gameObject.SetActive(true);
                panelButton.enabled = false;
            }
            else
            {
                panelButton.enabled = true;
                spacerContainer.gameObject.SetActive(false);
                userProfileSocial = userProfileForSocialScreen;
                playerNameTxt.text = userProfileSocial.UserProfile.nickName;
                userNameTxt.text = userProfileSocial.UserProfile.userName;
                socialState = userProfileForSocialScreen.SocialState;
                panelState = userProfileForSocialScreen.PanelState;
                onActionMadeCallback = userProfileForSocialScreen.OnActionMadeCallback;
                showSocialsLoading = userProfileForSocialScreen.ShowSocialsLoading;
                favoriteBtn.Initialize(userProfileSocial.UserProfile.accountId, onActionMadeCallback);
                favoriteBtn.SetIsFavorite(userProfileForSocialScreen.SocialState.HasFlag(SocialState.Favorite));
            }

            UpdateStateUI();
        }

        private void UpdateStateUI()
        {
            var hasActiveRoom = userProfileSocial.RoomId is not null && userProfileSocial.RoomId != string.Empty;
            var showJoinHangoutButton = socialState.HasFlag(SocialState.Friends) && hasActiveRoom;
            friendIcon.gameObject.SetActive(socialState.HasFlag(SocialState.Friends));
            
            switch (panelState)
            {
                case SocialScreen.PanelState.Followers:
                    followBtn.gameObject.SetActive(!socialState.HasFlag(SocialState.Following) && !socialState.HasFlag(SocialState.Friends) && !showJoinHangoutButton);
                    unfollowBtn.gameObject.SetActive(false);
                    joinHangoutBtn.gameObject.SetActive(showJoinHangoutButton);
                    break;

                case SocialScreen.PanelState.Following:
                    followBtn.gameObject.SetActive(false);
                    unfollowBtn.gameObject.SetActive(!showJoinHangoutButton);
                    joinHangoutBtn.gameObject.SetActive(showJoinHangoutButton);
                    break;

                case SocialScreen.PanelState.Friends:
                    followBtn.gameObject.SetActive(false);
                    unfollowBtn.gameObject.SetActive(false);
                    joinHangoutBtn.gameObject.SetActive(showJoinHangoutButton);
                    break;

                case SocialScreen.PanelState.Search:
                    followBtn.gameObject.SetActive(!socialState.HasFlag(SocialState.Following) && !socialState.HasFlag(SocialState.Friends) && !showJoinHangoutButton);
                    unfollowBtn.gameObject.SetActive(socialState.HasFlag(SocialState.Following) || socialState.HasFlag(SocialState.Friends) && !showJoinHangoutButton);
                    joinHangoutBtn.gameObject.SetActive(showJoinHangoutButton);
                    break;
            }
        }

        private async void HandleFollowButtonClick()
        {
            var request = new FollowUserRequest
            {
                userId = userProfileSocial.UserProfile.accountId
            };

            actionLoadingIcon.gameObject.SetActive(true);
            var result = await Services.SocialService.FollowUserAsync(request);
            actionLoadingIcon.gameObject.SetActive(false);

            if (!result.HasError)
            {
                // TODO: Do something ...
                onActionMadeCallback?.Invoke();
            }
        }

        private async void HandleUnfollowButtonClick()
        {
            var request = new UnfollowUserRequest
            {
                userId = userProfileSocial.UserProfile.accountId
            };

            actionLoadingIcon.gameObject.SetActive(true);
            var result = await Services.SocialService.UnfollowUserAsync(request);
            actionLoadingIcon.gameObject.SetActive(false);

            if (!result.HasError)
            {
                // TODO: Do something ...
                onActionMadeCallback?.Invoke();
            }
        }

        private void HandleJoinHangoutButtonClick()
        {
            SocialManager.Instance.JoinRoomDirect(userProfileSocial.RoomId);
        }

        /// <remarks>
        /// Opens the user profile screen of this other user
        /// </remarks>
        private void HandleUserPanelTap()
        {
            showSocialsLoading?.Invoke(true);
            PopupManager.Instance.OpenScenePopup(SceneNames.USER_PROFILE_POPUP, () =>
            {
                var userProfile = userProfileSocial.UserProfile;
                var scene = SceneLoadingManager.Instance.GetScene(SceneNames.USER_PROFILE_POPUP);
                Variables.Scene(scene).Set("AccountId", userProfile.accountId);
                Variables.Scene(scene).Set("PlayerId", userProfile.playerId);
                Variables.Scene(scene).Set("PopupState", UserProfilePopup.UserProfileState.Social);
                SceneLoadingManager.Instance.SetActiveScene(SceneNames.USER_PROFILE_POPUP);
                GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("InitUserProfile"));
                showSocialsLoading?.Invoke(false);
            });
        }
    }
}

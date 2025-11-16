using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Backend
{
    public abstract class FirebaseAuthController : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI txtError;
        [SerializeField] protected Button signInButton;

        protected FSMSendUnityEvent onFirebaseSignInSuccess;

        protected virtual void ShowError(string error)
        {
            txtError.text = error;
            txtError.gameObject.SetActive(true);
        }

        protected virtual void HideError()
        {
            txtError.gameObject.SetActive(false);
        }

        protected virtual void OnButtonClicked()
        {
            ShowSignInWindow();
        }

        protected virtual void ShowSignInWindow()
        {
        }

        protected virtual async void SignInToFirebase(SignInRequest request)
        {
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);

            var auth = Services.AuthService;
            var signInResult = await auth.SignInAsync(request);

            if (signInResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(signInResult.Error));
                LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);
                return;
            }

            var resolvePlayerResult = await auth.ResolvePlayerAsync(new ResolvePlayerRequest());

            if (resolvePlayerResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(resolvePlayerResult.Error));
                LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);
                return;
            }

            var userProfileResult = await Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);

            if (userProfileResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(userProfileResult.Error));
                return;
            }

            if (userProfileResult.Result == null)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(userProfileResult.Error));
                return;
            }

            var userProfile = userProfileResult.Result.UserProfile;
            SignUpVerificationScreen.isUserProfileReady = IsUserProfileIsReady(userProfile);
            if (SignUpVerificationScreen.isUserProfileReady)
            {
                UserProfileLocalDataManager.Instance.UpdateLocalUserProfile(userProfile);
            }
            "login successful".Log();
            GlobalNotifier.Instance.Trigger(onFirebaseSignInSuccess);
        }

        protected bool IsUserProfileIsReady(UserProfile userProfile)
        {
            var nickname = userProfile.nickName;
            if (nickname != null)
            {
                return !string.IsNullOrWhiteSpace(nickname);
            }

            return false;
        }

        protected virtual void Awake()
        {
            onFirebaseSignInSuccess = new FSMSendUnityEvent("OnFirebaseSignInSuccess");
        }

        protected virtual void OnEnable()
        {
            signInButton.onClick.AddListener(OnButtonClicked);
        }

        protected virtual void OnDisable()
        {
            signInButton.onClick.RemoveListener(OnButtonClicked);
        }
    }
}

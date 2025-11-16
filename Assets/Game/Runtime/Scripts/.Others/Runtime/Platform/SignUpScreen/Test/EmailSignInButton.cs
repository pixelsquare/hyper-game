using Firebase.Auth;
using Firebase.Extensions;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class EmailSignInButton : MonoBehaviour
    {
        [SerializeField] private TMP_InputField txtEmail;
        [SerializeField] private TMP_InputField txtPassword;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private Button signInButton;
        [SerializeField] private GameObject emailScreen;
        
        private void OnButtonClicked()
        {
            SignInUser();
        }

        private void ShowError(string error)
        {
            txtError.text = error;
            txtError.gameObject.SetActive(true);
        }

        private void HideError()
        {
            txtError.text = "";
            txtError.gameObject.SetActive(false);
        }

        private async void SignInUser()
        {
            HideError();
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(true);
            var firebaseAuth = FirebaseAuth.DefaultInstance;
            var authService = Services.AuthService;
            var hasError = false;
            var errorMsg = "";

            await firebaseAuth.SignInWithEmailAndPasswordAsync(txtEmail.text, txtPassword.text).ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    hasError = true;
                    errorMsg = "SignInWithEmailAndPasswordAsync was canceled.";
                    return;
                }

                if (task.IsFaulted)
                {
                    hasError = true;
                    errorMsg = "SignInWithEmailAndPasswordAsync encountered an error ";
                    $"error on sign in: {task.Exception}".LogError();
                    return;
                }

                var result = task.Result;
                FirebaseAuthService.firebaseUser = result;
                $"User signed in successfully: {result.Email}".Log();
            });

            if (hasError)
            {
                ShowError(errorMsg);
                LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);
                return;
            }

            await authService.GetUserTokenAsync();
            
            var resolvePlayerResult = await authService.ResolvePlayerAsync(new ResolvePlayerRequest());

            if (resolvePlayerResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(resolvePlayerResult.Error));
                LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);
                return;
            }

            var userProfileResult = await Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());

            emailScreen.SetActive(false);
            LoadingScreenManager.Instance.ShowHideLoadingOverlay(false);

            if (userProfileResult.HasError)
            {
                GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnEmailSignUp"));
                return;
            }
            
            if( string.IsNullOrWhiteSpace(userProfileResult.Result.UserProfile.nickName) )
            {
                GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnEmailSignUp"));
                return;
            }
            
            var userProfile = userProfileResult.Result.UserProfile; 
            
            UserProfileLocalDataManager.Instance.UpdateLocalUserProfile(userProfile);
            
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnEmailSignIn"));
        }

        private void Start()
        {
            #if UNITY_EDITOR
            gameObject.SetActive(true);
            #else
            gameObject.SetActive(false);
            #endif
        }

        private void OnEnable()
        {
            signInButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            signInButton.onClick.RemoveListener(OnButtonClicked);
        }
    }
}

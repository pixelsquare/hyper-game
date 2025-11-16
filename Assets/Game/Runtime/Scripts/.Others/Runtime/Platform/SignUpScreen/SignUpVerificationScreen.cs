using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class SignUpVerificationScreen : MonoBehaviour
    {
        public static bool isUserProfileReady;

        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private VerifCodeInputController verifCodeInputController;
        [SerializeField] private Button btnResendCode;

        [SerializeField] private UnityEvent onOTPReRequest;
        [SerializeField] private UnityEvent onOTPVerifSuccess;
        [SerializeField] private UnityEvent onOTPVerifFailed;

        private LoadingScreenManager LoadingScreenManager => LoadingScreenManager.Instance;

        // init in fsm
        public void Initialize()
        {
            var areaCode = $"+{MobileNumberUtil.GetCurrentCountryCode().code}";
            txtNumber.text = areaCode + SignUpScreen.mobileNumber;
        }

        private void ShowError(string error)
        {
            verifCodeInputController.ClearText();
            txtError.text = error;
            txtError.gameObject.SetActive(true);
        }

        private void HideError()
        {
            txtError.gameObject.SetActive(false);
        }

        private async void LoginOTPAsync(string mobile, string otp)
        {
            HideError();
            verifCodeInputController.DeactivateField();
            LoadingScreenManager.ShowHideLoadingOverlay(true);
            var firebaseAuthService = Services.AuthService;
            var serviceResult = await Services.AuthService.LoginUserSendOtpAsync(new LoginUserSendOtpRequest
            {
                mobile = MobileNumberUtil.Sanitize(mobile),
                otp = otp
            });

            isUserProfileReady = false;

            if (serviceResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(serviceResult.Error));
                LoadingScreenManager.ShowHideLoadingOverlay(false);
                return;
            }

            var resolvePlayerResult = await firebaseAuthService.ResolvePlayerAsync(new ResolvePlayerRequest());

            if (resolvePlayerResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(resolvePlayerResult.Error));
                LoadingScreenManager.ShowHideLoadingOverlay(false);
                return;
            }

            var userProfileResult = await Services.UserProfileService.GetUserProfileAsync(new GetUserProfileRequest());
            LoadingScreenManager.ShowHideLoadingOverlay(false);

            if (userProfileResult.HasError)
            {
                if (userProfileResult.Error.Code == ServiceErrorCodes.APP_VERSION_MISMATCH)
                {
                    ShowError("App version mismatch. Please update to the latest version of Ube.");
                    return;
                }

                onOTPVerifFailed?.Invoke();
                return;
            }

            if (userProfileResult.Result == null)
            {
                onOTPVerifFailed?.Invoke();
                return;
            }

            var userProfile = userProfileResult.Result.UserProfile; 
            CheckIfUserProfileIsReady(userProfile);
            if (isUserProfileReady)
            {
                UserProfileLocalDataManager.Instance.UpdateLocalUserProfile(userProfile);
            }

            onOTPVerifSuccess?.Invoke();
        }

        private void CheckIfUserProfileIsReady(UserProfile userProfile)
        {
            var nickname = userProfile.nickName;
            if (nickname != null)
            {
                isUserProfileReady = !string.IsNullOrWhiteSpace(nickname);
            }
        }

        private void OnVerifCodeUpdated(string value)
        {
            // check verif code validity here
            if (value.Length < 6)
            {
                return;
            }

            if (!Validators.OtpValidator.IsValid(value))
            {
                ShowError("OTP is invalid. Please try again.");
                return;
            }

            LoginOTPAsync(SignUpScreen.mobileNumber, value);
        }

        private void OnResendCodeClicked()
        {
            HideError();
            verifCodeInputController.ClearText();
            onOTPReRequest?.Invoke();
            ShowError("Resending code...");
        }

        private void ResetScreen()
        {
            verifCodeInputController.ClearText();
            HideError();
        }

        private void OnEnable()
        {
            btnResendCode.onClick.AddListener(OnResendCodeClicked);
            verifCodeInputController.OnVerifCodeUpdated += OnVerifCodeUpdated;

            ResetScreen();
        }

        private void OnDisable()
        {
            btnResendCode.onClick.RemoveListener(OnResendCodeClicked);
            verifCodeInputController.OnVerifCodeUpdated -= OnVerifCodeUpdated;
        }
    }
}

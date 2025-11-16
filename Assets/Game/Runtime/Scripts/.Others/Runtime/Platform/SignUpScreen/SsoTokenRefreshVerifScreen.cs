using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class SsoTokenRefreshVerifScreen : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private VerifCodeInputController verifCodeInputController;
        [SerializeField] private Button btnResendCode;
        [SerializeField] private Button btnBack;

        private FSMSendUnityEvent sendOtpSuccessEvent;
        private FSMSendUnityEvent backBtnPressedEvent;

        private LoadingScreenManager LoadingScreenManager => LoadingScreenManager.Instance;
        private GlobalNotifier GlobalNotifier => GlobalNotifier.Instance;

        // init in fsm
        public void Initialize()
        {
            var areaCode = $"+{MobileNumberUtil.GetCurrentCountryCode().code}";
            txtNumber.text = areaCode + SignUpScreen.mobileNumber;
        }

        private async void RefreshLinkRequestOtpAsync(bool isResending)
        {
            HideError();
            LoadingScreenManager.ShowHideLoadingOverlay(true);
            if (isResending)
            {
                ShowError("Resending code...");
            }
            
            var serviceResult =
                await Services.AuthService.RefreshLinkRequestOtpAsync(new RefreshLinkRequestOtpRequest());
            LoadingScreenManager.ShowHideLoadingOverlay(false);

            if (!serviceResult.HasError)
            {
                if (isResending)
                {
                    ShowError("Code resent.");
                }
                return;
            }
       
            ShowError(serviceResult.Error.Message);
        }

        private async void RefreshLinkSendOtpAsync(string otp)
        {
            HideError();
            verifCodeInputController.DeactivateField();

            var request = new RefreshLinkSendOtpRequest()
            {
                otp = otp
            };
            
            LoadingScreenManager.ShowHideLoadingOverlay(true);
            var serviceResult =
                await Services.AuthService.RefreshLinkSendOtpAsync(request);
            LoadingScreenManager.ShowHideLoadingOverlay(false);

            if (serviceResult.HasError)
            {
                ShowError(serviceResult.Error.Message);
                return;
            }
            gameObject.SetActive(false);
            GlobalNotifier.Trigger(sendOtpSuccessEvent);
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
            
            RefreshLinkSendOtpAsync(value);
        }

        private void OnResendCodeClicked()
        {
            HideError();
            verifCodeInputController.ClearText();
            RefreshLinkRequestOtpAsync(true);
        }

        private void SetMobileNumber()
        {
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            txtNumber.text = userProfile.mobile;
        }

        private void ResetScreen()
        {
            verifCodeInputController.ClearText();
            HideError();
            RefreshLinkRequestOtpAsync(false);
        }

        private void OnBackBtnClicked()
        {
            GlobalNotifier.Trigger(backBtnPressedEvent);
        }
        
        private void OnEnable()
        {
            btnResendCode.onClick.AddListener(OnResendCodeClicked);
            btnBack.onClick.AddListener(OnBackBtnClicked);
            verifCodeInputController.OnVerifCodeUpdated += OnVerifCodeUpdated;
            sendOtpSuccessEvent = new FSMSendUnityEvent("OnOtpRequestSuccess");
            backBtnPressedEvent = new FSMSendUnityEvent("Back");
            SetMobileNumber();
            ResetScreen();
        }

        private void OnDisable()
        {
            btnResendCode.onClick.RemoveListener(OnResendCodeClicked);
            btnBack.onClick.RemoveListener(OnBackBtnClicked);
            verifCodeInputController.OnVerifCodeUpdated -= OnVerifCodeUpdated;
        }
    }
}

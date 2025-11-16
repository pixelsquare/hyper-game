using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class SignUpScreen : MonoBehaviour
    {
        public static bool isSignUpState = true;
        public static string mobileNumber = "";

        [Header("UI References")]
        [SerializeField] private Button btnSubmit;
 
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private TextMeshProUGUI verifTxtError;
        [SerializeField] private TextMeshProUGUI txtAreaCode;
    
        [SerializeField] private TMP_InputField inputFieldNumber;
        [SerializeField] private TMP_Text placeholder;

        [SerializeField] private UnityEvent onOtpRequestSuccess;
        [SerializeField] private CountryCodeController countryCodeController;
        
        private LoadingScreenManager LoadingScreenManager => LoadingScreenManager.Instance;
        private GlobalNotifier GlobalNotifier => GlobalNotifier.Instance;
        private FSMSendUnityEvent sendOtpSuccessEvent;

        public void SetScreenState(bool isSignUp)
        {
            isSignUpState = isSignUp;
            inputFieldNumber.text = "";
            HideError();
   
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.MobileNumber));
        }

        // used in FSM
        public void RegisterOrLogin(bool isResent = false)
        {
            SignInAsync(mobileNumber, isResent);
        }

        private void ResetScreen()
        {
            inputFieldNumber.text = "";
            HideError();
        }
        
        private void SetCharLimit(int limit)
        {
            inputFieldNumber.lineLimit = 1;
            inputFieldNumber.characterLimit = limit;
        }

        private void SetPlaceHolderText(string text)
        {
            placeholder.text = text;
        }

        private void ShowError(string error)
        {
            txtError.text = error;
            verifTxtError.text = error;
            txtError.gameObject.SetActive(true);
            verifTxtError.gameObject.SetActive(true);
        }

        private void HideError()
        {
            verifTxtError.gameObject.SetActive(false);
            txtError.gameObject.SetActive(false);
        }

        private void OnSignInPromptClicked()
        {
            isSignUpState = !isSignUpState;
            SetScreenState(isSignUpState);
        }

        private void OnMobileNumberUpdated(string value)
        {
            mobileNumber = value;
            var currentCode = MobileNumberUtil.GetCurrentCountryCode().code;
            HideError();

            if (string.IsNullOrEmpty(mobileNumber))
            {
                btnSubmit.interactable = false;
                return;
            }
            
            var numberToCheck = $"+{currentCode}{mobileNumber}";
            if (!Validators.MobileValidator.IsValid(numberToCheck))
            {
                ShowError("Invalid mobile number.");
                btnSubmit.interactable = false;
                return;
            }

            btnSubmit.interactable = true;
        }

        private void OnEnterClicked()
        {
            HideError();
            var currentCode = MobileNumberUtil.GetCurrentCountryCode().code;

            if (!Validators.MobileValidator.IsValid($"+{currentCode}{mobileNumber}"))
            {
                ShowError("Invalid mobile number.");
                return;
            }

            mobileNumber = inputFieldNumber.text;

            RegisterOrLogin();
        }

        private async void SignInAsync(string mobileNumber, bool isResent = false)
        {
            LoadingScreenManager.ShowHideLoadingOverlay(true);
            var authServiceResult = await Services.AuthService.LoginUserRequestOtpAsync(new LoginUserRequestOtpRequest
            {
                mobile = MobileNumberUtil.Sanitize(mobileNumber)
            });
            LoadingScreenManager.ShowHideLoadingOverlay(false);

            if (authServiceResult.HasError)
            {
                ShowError(BackendUtil.GetDisplayableErrorMessage(authServiceResult.Error));
                return;
            }
            
            if (isResent)
            {
                ShowError("Code resent.");
            }

            GlobalNotifier.Trigger(sendOtpSuccessEvent);
            onOtpRequestSuccess?.Invoke();
        }
        
        private void OnSelectedCodeUpdated(int value)
        {
            ResetScreen();
            MobileNumberUtil.SetCurrentCountryCode(MobileNumberUtil.GetCountryCodes()[value]);
            SetCharLimit(MobileNumberUtil.GetNationalNumberLimit(MobileNumberUtil.GetCurrentCountryCode().alpha));
            SetPlaceHolderText(MobileNumberUtil.GetExampleNationalNumber(MobileNumberUtil.GetCurrentCountryCode().alpha));
        }

        private void Initialize()
        {
            MobileNumberUtil.GetCountryCodes();
            MobileNumberUtil.SetPHAsCurrentCountryCode();
            SetCharLimit(MobileNumberUtil.GetNationalNumberLimit(MobileNumberUtil.GetCurrentCountryCode().alpha));
            SetPlaceHolderText(MobileNumberUtil.GetExampleNationalNumber(MobileNumberUtil.GetCurrentCountryCode().alpha));
            countryCodeController.Initialize();
        }

        private void OnEnable()
        {
            btnSubmit.onClick.AddListener(OnEnterClicked);
            inputFieldNumber.onValueChanged.AddListener(OnMobileNumberUpdated);
            countryCodeController.onSelectedUpdated.AddListener(OnSelectedCodeUpdated);
            ResetScreen();
            Initialize();
        }

        private void OnDisable()
        {
            btnSubmit.onClick.RemoveListener(OnEnterClicked);
            inputFieldNumber.onValueChanged.RemoveListener(OnMobileNumberUpdated);
            countryCodeController.onSelectedUpdated.RemoveListener(OnSelectedCodeUpdated);
        }

        private void Start()
        {
            sendOtpSuccessEvent = new FSMSendUnityEvent("OnOtpRequestSuccess");
            btnSubmit.interactable = false;
        }
    }
}

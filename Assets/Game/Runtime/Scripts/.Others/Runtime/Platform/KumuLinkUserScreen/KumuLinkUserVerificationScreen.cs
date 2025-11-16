using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class KumuLinkUserVerificationScreen : MonoBehaviour
    {
        public static string otpVerifCode;
        
        [SerializeField] private TextMeshProUGUI txtKumuUsername;
        [SerializeField] private TextMeshProUGUI txtError;
        [SerializeField] private VerifCodeInputController verifCodeInputController;
        [SerializeField] private Button btnResendCode;

        // used in the fsm
        public void UpdateLocalLinkedState()
        {
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();
            userProfile.hasLinkedKumuAccount = true;
            UserProfileLocalDataManager.Instance.UpdateLocalUserProfile(userProfile);
        }

        public void ShowError(string error)
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
            if (value.Length < 6)
            {
                return;
            }

            if (!Validators.OtpValidator.IsValid(value))
            {
                ShowError("OTP is invalid. Please try again.");
                return;
            }

            verifCodeInputController.DeactivateField();
            otpVerifCode = value;
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnVerifCodeSubmitted"));
        }
        
        private void OnResendCodeClicked()
        {
            ResetScreen();
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnVerifCodeReRequest"));
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
            txtKumuUsername.text = KumuLinkUserScreen.kumuUsername;
            ResetScreen();
        }

        private void OnDisable()
        {
            btnResendCode.onClick.RemoveListener(OnResendCodeClicked);
            verifCodeInputController.OnVerifCodeUpdated -= OnVerifCodeUpdated;
        }
    }
}

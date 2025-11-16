using System.Linq;
using Kumu.Kulitan.Backend;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Common
{
    public class SubmitButtonHandler : MonoBehaviour
    {
        [SerializeField] private FeedbackCollector feedbackCollector;
        [SerializeField] private Button buttonSubmit;

        public TMP_InputField emailInputField;
        public TMP_InputField wirelessHeadPhoneInputField;
        public TMP_InputField similarAppInputField;
        public TMP_InputField crashesInputField;
        public TMP_InputField commentsInputField;
        public SaveToggleInputFeedback enjoyUbeToggleInput;
        public SaveToggleInputFeedback graphicsToggleInput;
        public SaveToggleInputFeedback intuitiveToggleInput;
        public SaveToggleInputFeedback avatarDesignToggleInput;
        public SaveToggleInputFeedback joinRoomsToggleInput;
        public SaveToggleInputFeedback moveAvatarToggleInput;
        public SaveToggleInputFeedback emotesToggleInput;
        public SaveToggleInputFeedback voiceChatToggleInput;
        public SaveToggleInputFeedback roomDesignToggleInput;
        public SaveToggleInputFeedback phonePerformanceToggleInput;
        public SaveToggleInputFeedback ubeSatifactionToggleInput;
        public SaveToggleInputFeedback ndaToggleInput;

        private static bool TextFieldIsNotEmpty(params string[] strings)
        {
            return strings.All(str => !string.IsNullOrWhiteSpace(str));
        }

        private void EnableSubmitButton()
        {
            var areRequiredFieldsFilled = 
                Validators.EmailValidator.IsValid(emailInputField.text) &&
                TextFieldIsNotEmpty(
                    wirelessHeadPhoneInputField.text,
                    similarAppInputField.text,
                    crashesInputField.text,
                    commentsInputField.text,
                    enjoyUbeToggleInput.toggleValue,
                    graphicsToggleInput.toggleValue,
                    intuitiveToggleInput.toggleValue,
                    avatarDesignToggleInput.toggleValue,
                    joinRoomsToggleInput.toggleValue,
                    moveAvatarToggleInput.toggleValue,
                    emotesToggleInput.toggleValue,
                    voiceChatToggleInput.toggleValue,
                    roomDesignToggleInput.toggleValue,
                    phonePerformanceToggleInput.toggleValue,
                    ubeSatifactionToggleInput.toggleValue,
                    ndaToggleInput.toggleValue);

            buttonSubmit.interactable = areRequiredFieldsFilled;
        }

        private void DisableSubmitButton()
        {
            buttonSubmit.interactable = false;
        }

        private void Start()
        {
            DisableSubmitButton();
        }

        private void Update()
        {
            if (PlayerPrefs.GetInt("Feedback Form") == 1)
            {
                DisableSubmitButton();
            }
            else
            {
                EnableSubmitButton();
            }
        }
    }
}

using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Tracking;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class UsernameScreen : MonoBehaviour
    {
        public static string username;
        
        [SerializeField] private TMP_InputField inputFieldUsername;
        [SerializeField] private Button btnSubmit;
        [SerializeField] private GameObject[] errorIndicators;
 
        [SerializeField] private UnityEvent onUsernameSubmitted;

        private IInputFormatValidator<UserNameFormatDetails> UsernameValidator => Validators.UserNameValidator;
        
        private void UpdateErrorIndicators(UserNameFormatDetails details)
        {
            errorIndicators[0].SetActive(details.IsWithinCharLimits);
            errorIndicators[1].SetActive(details.IsLowercase);
            errorIndicators[2].SetActive(details.HasNoSpaces);
            errorIndicators[3].SetActive(details.HasNoSpecialChars);
        }
            
        private void OnUsernameSubmitted()
        {
            username = inputFieldUsername.text;
            onUsernameSubmitted?.Invoke();
        }

        private void OnUsernameUpdated(string value)
        {
            if (!UsernameValidator.IsValid(value, out var details))
            {
                btnSubmit.interactable = false;
                UpdateErrorIndicators(details);
                return;
            }
            UpdateErrorIndicators(details);
            btnSubmit.interactable = true;
        }

        private void Awake()
        {
            btnSubmit.interactable = false;
        }

        private void OnEnable()
        {
            btnSubmit.onClick.AddListener(OnUsernameSubmitted);
            inputFieldUsername.onValueChanged.AddListener(OnUsernameUpdated);
            inputFieldUsername.text = "";
            GlobalNotifier.Instance.Trigger(new UserJourneyEvent(UserJourneyEvent.Checkpoint.Username));
        }

        private void OnDisable()
        {
            btnSubmit.onClick.RemoveListener(OnUsernameSubmitted);
            inputFieldUsername.onValueChanged.RemoveListener(OnUsernameUpdated);
        }
    }
}

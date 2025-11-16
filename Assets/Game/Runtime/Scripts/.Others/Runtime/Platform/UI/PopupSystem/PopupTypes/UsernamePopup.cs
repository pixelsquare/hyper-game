using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kumu.Kulitan.UI
{
    public class UsernamePopup : BasePopup
    {
        [SerializeField] private int usernameCharLimit = 16;
        [SerializeField] private TMP_InputField inputFieldUsername;
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button submitButton;

        private string username;

        public Action OnSubmit { get; set; }

        public void AddCallback(Action<string> callback)
        {
            OnSubmit += () =>
            {                
                callback?.Invoke(username);
            };
        }

        public void Submit()
        {
            if (String.IsNullOrEmpty(inputFieldUsername.text.Trim()))
            {
                PopupManager.Instance.OpenErrorPopup("Error", "Username not set.", "Okay");
                return;
            }

            username = inputFieldUsername.text;
            OnSubmit?.Invoke();
            Close();
        }

        private void ValidateUsername(string text)
        {
            if (String.IsNullOrEmpty(text.Trim()))
            {
                ShowErrorText("* Username cannot be blank.");
                submitButton.enabled = false;
            }
            else
            {
                submitButton.enabled = true;
                HideErrorText();
            }
        }

        private void ShowErrorText(string error)
        {
            if (!errorText)
            {
                return;
            }
            errorText.text = error;
            errorText.gameObject.SetActive(true);
        }

        private void HideErrorText()
        {
            if (!errorText)
            {
                return;
            }
            errorText.gameObject.SetActive(false);
        }

        private void Initialize()
        {
            HideErrorText();
            inputFieldUsername.characterLimit = usernameCharLimit;
            submitButton.enabled = false;
        }

        private void OnEnable()
        {
            submitButton.onClick.AddListener(Submit);
            inputFieldUsername.onEndEdit.AddListener(ValidateUsername);
            Initialize();
        }

        private void OnDisable()
        {
            submitButton.onClick.RemoveListener(Submit);
            inputFieldUsername.onEndEdit.RemoveListener(ValidateUsername);
        }
    }
}

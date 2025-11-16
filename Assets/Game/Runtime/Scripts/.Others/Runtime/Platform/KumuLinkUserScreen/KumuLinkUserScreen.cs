using System;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class KumuLinkUserScreen : MonoBehaviour
    {
        public static string kumuUsername;

        [SerializeField] private TMP_InputField inputFieldUsername;
        [SerializeField] private Button btnSubmit;
        [SerializeField] private TextMeshProUGUI txtError;

        [SerializeField] private UnityEvent onUsernameSubmitted;
        private IEvent<string> popupClosedEvent; 

        private void ShowError(string error)
        {
            txtError.text = error;
            txtError.gameObject.SetActive(true);
        }
        
        private void HideError()
        {
            txtError.gameObject.SetActive(false);
        }
        
        private void OnUsernameSubmitted()
        {
            kumuUsername = inputFieldUsername.text;
            btnSubmit.interactable = false;
            onUsernameSubmitted?.Invoke();
            GlobalNotifier.Instance.Trigger(new FSMSendUnityEvent("OnUsernameSubmitted"));
        }

        private void OnUsernameUpdated(string value)
        {
            HideError();

            var strlen = value.Length;

            if (strlen <= 0)
            {
                btnSubmit.interactable = false;
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                btnSubmit.interactable = false;
                ShowError("username is invalid");
                return;
            }

            if (!Validators.UsernameLinkValidator.IsValid(value))
            {
                btnSubmit.interactable = false;
                ShowError("username is invalid");
                return;
            }
            
            btnSubmit.interactable = true;
            HideError();
        }
        
        private void ResetScreen()
        {
            inputFieldUsername.text = "";
            HideError();
            btnSubmit.interactable = false;
        }
        
        private void Awake()
        {
            btnSubmit.interactable = false;
            popupClosedEvent = new OnScenePopupClosedEvent("KumuAccountLinkPopup");
        }

        private void OnDestroy()
        {
            GlobalNotifier.Instance?.Trigger(popupClosedEvent);
        }

        private void OnEnable()
        {
            btnSubmit.onClick.AddListener(OnUsernameSubmitted);
            inputFieldUsername.onValueChanged.AddListener(OnUsernameUpdated);
            ResetScreen();
        }

        private void OnDisable()
        {
            btnSubmit.onClick.RemoveListener(OnUsernameSubmitted);
            inputFieldUsername.onValueChanged.RemoveListener(OnUsernameUpdated);
        }
    }
}

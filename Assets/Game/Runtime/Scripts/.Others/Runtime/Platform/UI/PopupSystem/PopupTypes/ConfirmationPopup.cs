using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kumu.Kulitan.UI
{
    public class ConfirmationPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI confirmButtonText;
        [SerializeField] private TextMeshProUGUI cancelButtonText;

        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public Action OnConfirm { get; set; }
        public Action OnCancel { get; set; }

        private bool isClosing;

        public void SetDetails(string title, string message, string confirmButton, string cancelButton)
        {
            titleText.text = title;
            messageText.text = message;
            confirmButtonText.text = confirmButton;
            cancelButtonText.text = cancelButton;
        }

        public void Confirm()
        {
            if (isClosing)
            {
                return;
            }
            
            isClosing = true;
            OnConfirm?.Invoke();
            base.Close();
        }

        public void Cancel()
        {
            if (isClosing)
            {
                return;
            }
            
            isClosing = true;
            OnCancel?.Invoke();
            base.Close();
        }

        private void OnEnable()
        {
            isClosing = false;
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(Cancel);   
        }

        private void OnDisable()
        {
            confirmButton.onClick.RemoveListener(Cancel);
            cancelButton.onClick.RemoveListener(Cancel);
        }
    }
}

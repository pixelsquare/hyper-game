using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Kumu.Kulitan.UI
{
    public class GenericTextPopup : BasePopup
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI buttonText;

        [SerializeField] private Button closeButton;

        public void SetDetails(string title, string message, string button)
        {
            titleText.text = title;
            messageText.text = message;
            if (button != null && !string.IsNullOrEmpty(button))
            {
                buttonText.text = button;
            }
            else
            {
                closeButton.gameObject.SetActive(false);
            }
        }

        public void OnPopupClose()
        {
            base.Close();
        }

        private void OnEnable()
        {
            closeButton.onClick.AddListener(OnPopupClose);   
        }

        private void OnDisable()
        {
            closeButton.onClick.RemoveListener(OnPopupClose);
        }
    }
}

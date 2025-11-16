using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.UI
{
    public class KumuLinkUserButtonController : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI buttonText;

        [SerializeField] private string enabledBtnText = "LINK KUMU ACCOUNT";
        [SerializeField] private string linkedAcctText = "KUMU ACCOUNT LINKED";
            
        private void SetButtonText(string msg)
        {
            buttonText.text = msg;
        }

        private void EnableAccountLinking()
        {
            button.interactable = true;
            SetButtonText(enabledBtnText);
        }

        private void UpdateLinkButtonState()
        {
            var cachedUserProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();
            if (cachedUserProfile.hasLinkedKumuAccount)
            {
                button.interactable = false;
                SetButtonText(linkedAcctText);
                return;
            }

            EnableAccountLinking();
        }

        private void OnEnable()
        {
            UpdateLinkButtonState();
        }
    }
}

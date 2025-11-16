using Kumu.Kulitan.Common;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class MainScreenSignOutController : MonoBehaviour
    {
        [SerializeField] private string textBoxMessage = "Are you sure you want to sign out?";
        [SerializeField] private string confirmButtonText = "Sign Out";
        [SerializeField] private string cancelButtonText = "Cancel";
        
        private ConfirmationPopup signOutConfirmationPopup;
        
        public void AttemptSignOut()
        {
            if (signOutConfirmationPopup != null)
            {
                return;
            }

            signOutConfirmationPopup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup("SIGN OUT", 
                textBoxMessage, confirmButtonText, cancelButtonText);
            signOutConfirmationPopup.OnConfirm += OnSignOutConfirm;
        }

        private void OnSignOutConfirm()
        {
            signOutConfirmationPopup = null;
            SignOutManager.Instance.UbeSignOut();
        }
    }
}

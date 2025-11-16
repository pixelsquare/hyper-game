using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class UnblockPlayerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private UnityEvent onUserUnblocked;

        private string accountId;
        private uint playerId;

        public void Initialize(string accountId, uint playerId)
        {
            this.accountId = accountId;
            this.playerId = playerId;
        }

        private void HandleButtonClicked()
        {
            UnblockUser();
        }

        private async void UnblockUser()
        {
            var request = new UnblockPlayerRequest { userId = accountId };
            var result = await Services.ModerationService.UnblockPlayerAsync(request);

            if (result.HasError)
            {
                var errorPopup = PopupManager.Instance.OpenErrorPopup("Error", result.Error.Message, "Ok");
                errorPopup.OnClosed = () => { onUserUnblocked?.Invoke(); };
                return;
            }

            var popup = PopupManager.Instance.OpenTextPopup("Success", "You have successfully unblocked the user.", "Ok");
            popup.OnClosed = () => { onUserUnblocked?.Invoke(); };

            GlobalNotifier.Instance.Trigger(new PlayerBlockedEvent(accountId, playerId, false));
        }

        private void OnEnable()
        {
            button.onClick.AddListener(HandleButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(HandleButtonClicked);
        }
    }
}

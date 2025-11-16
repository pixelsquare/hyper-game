using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    [RequireComponent(typeof(Button))]
    public class BlockPlayerButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private UnityEvent onUserBlocked;

        private string accountId;
        private uint playerId;

        public void Initialize(string accountId, uint playerId)
        {
            this.accountId = accountId;
            this.playerId = playerId;
        }

        private void HandleButtonClicked()
        {
            var popup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup("Block this user?",
                    "They won't be able to send you\nmessages through voice or text.\n\nUbe won't let them know that you blocked them.", "Block", "Cancel");
            popup.OnConfirm += BlockUser;
        }

        private async void BlockUser()
        {
            var request = new BlockPlayerRequest { userId = accountId };
            var result = await Services.ModerationService.BlockPlayerAsync(request);

            if (result.HasError)
            {
                var errorPopup = PopupManager.Instance.OpenErrorPopup("Error", result.Error.Message, "Ok");
                errorPopup.OnClosed += () => { onUserBlocked?.Invoke(); };
                return;
            }

            var popup = PopupManager.Instance.OpenTextPopup("Success", "You have successfully blocked the user.", "Ok");
            popup.OnClosed += () => { onUserBlocked?.Invoke(); };

            GlobalNotifier.Instance.Trigger(new PlayerBlockedEvent(accountId, playerId, true));
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

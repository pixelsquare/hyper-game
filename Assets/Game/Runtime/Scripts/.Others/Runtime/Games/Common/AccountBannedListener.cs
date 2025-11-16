using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Common
{
    public class AccountBannedListener : MonoBehaviour
    {
        private Slot<string> eventSlot;

        private void OnAcccountBannedEventHandler(IEvent<string> obj)
        {
            var bannedAccountEvent = (AccountBannedEvent)obj;

            var banType = bannedAccountEvent.BanType;
            var bannedObject = bannedAccountEvent.BannedObject;

            PopupManager.Instance.OpenScenePopup(SceneNames.BANNED_ACCOUNT_POPUP, () =>
            {
                var popup = FindObjectOfType<AccountBannedPopup>(true);
                popup.Initialize(banType, bannedObject.causes, bannedObject.until_timestamp);
                popup.Open();
            });
        }

        private async void CheckTextAndVoiceChatBanAsync()
        {
            var scene = SceneLoadingManager.Instance.GetScene(SceneNames.MAIN_SCREEN);

            if (!scene.IsValid())
            {
                return;
            }

            var tokenResult = await Services.AgoraService.GetRTCTokenAsync(new GetRTCTokenRequest { channel = "Main Menu - Account Ban Check" });

            if (tokenResult.HasError)
            {
                $"Agora check failed. [{tokenResult.Error.Code}]".Log();
            }
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(AccountBannedEvent.EVENT_NAME, OnAcccountBannedEventHandler);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        private void Start()
        {
            CheckTextAndVoiceChatBanAsync();
        }
    }
}

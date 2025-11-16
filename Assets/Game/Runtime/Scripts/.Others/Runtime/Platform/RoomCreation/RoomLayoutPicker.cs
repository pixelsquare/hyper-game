using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.UI;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class RoomLayoutPicker : MonoBehaviour
    {
        private Slot<string> eventSlot;
        private RoomLayoutConfig activeRoomConfig;
        private LevelConfigScriptableObject activeLevelConfig;

        public void OnCreateRoomEvent(IEvent<string> callback)
        {
            OpenRoomLayoutPopup();
        }

        private void OpenRoomLayoutPopup()
        {
            PopupManager.Instance.ShowRoomLayoutPopup(roomConfig =>
            {
                activeRoomConfig = roomConfig;
                activeLevelConfig = roomConfig.LevelConfig;
                OpenCreateRoomPopup();
            });
        }

        private void OpenCreateRoomPopup()
        {
            var popup = (CreateRoomPopup)PopupManager.Instance.ShowCreateRoomPopup((roomDetails, levelConfig) =>
            {
                GlobalNotifier.Instance.Trigger(new HangoutCreateRoomBtnClickedEvent(roomDetails, levelConfig));
            });
            popup.RoomLayoutConfig = activeRoomConfig;
            popup.LevelConfig = activeLevelConfig;
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(CreateRoomEvent.EVENT_NAME, OnCreateRoomEvent);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }
    }
}

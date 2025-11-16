using UnityEngine;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using UnityEngine.UI;

namespace Kumu.Kulitan.UI
{
    public class CreateRoomButton : MonoBehaviour
    {
        [SerializeField] private int maxCapacity = 400;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI maxCapLabel;
        
        private CreateRoomEvent createRoomEvent;
        private Slot<string> eventSlot;
        
        public void ShowCreateRoomPopup()
        {
            GlobalNotifier.Instance.Trigger(createRoomEvent);
        }

        private void OnRoomListUpdated(IEvent<string> callback)
        {
            var eventData = (RoomListUpdatedEvent)callback;

            var belowCapacity = eventData.RoomList.Count < maxCapacity;

            button.interactable = belowCapacity;
            maxCapLabel.gameObject.SetActive(!belowCapacity);
        }

        #region Monobehaviour Events
        private void Awake()
        {
            createRoomEvent = new CreateRoomEvent();
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }

        private void OnEnable()
        {
            eventSlot.SubscribeOn(RoomListUpdatedEvent.EVENT_NAME, OnRoomListUpdated);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }

        #endregion
    }
}

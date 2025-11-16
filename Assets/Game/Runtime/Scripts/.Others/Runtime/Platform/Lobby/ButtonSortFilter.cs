using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class ButtonSortFilter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private SerializableDictionary<SortRoomMode, string> labelMap;

        private Slot<string> eventSlot;

        private void OnRoomsSorted(IEvent<string> callback)
        {
            var eventData = (SortFilterRoomEvent)callback;
            $"sorting rooms using {eventData.SortMode}".Log();
            text.text = labelMap[eventData.SortMode];
        }

        private void Start()
        {
            text.text = labelMap[LobbySort.CurrentSortMode];
        }

        private void OnEnable()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(SortFilterRoomEvent.EVENT_NAME, OnRoomsSorted);
        }

        private void OnDisable()
        {
            eventSlot.Dispose();
        }
    }
}

using System;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Lobby
{
    public class LobbyInfoView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private BarThreshold[] bars;
        
        private Slot<string> eventSlot;

        public bool TryRefreshLabel()
        {
            var text = RoomConnectionDetails.Instance.lobbyLabel;
            var hasText = text != null;

            if (hasText)
            {
                label.text = $"{text} Lobby";
            }
            else
            {
                label.text = string.Empty;
            }

            return hasText;
        }

        public void LeaveLobby()
        {
            ConnectionManager.Client.OpLeaveLobby();
            RoomConnectionDetails.Instance.lobbyLabel = null;
            RoomConnectionDetails.Instance.myLobby = null;
        }

        private void OnRoomListUpdated(IEvent<string> callback)
        {
            var eventData = (RoomListUpdatedEvent)callback;
            SetBars(eventData.RoomList.Count);
        }

        private void SetBars(int value)
        {
            foreach (var bar in bars)
            {
                var belowThreshold = bar.threshold > value;
                bar.indicator.SetActive(belowThreshold);
            }
        }

        #region Monobehaviour Events
        private void Awake()
        {
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

        [Serializable]
        private struct BarThreshold
        {
            public GameObject indicator;
            public int threshold;
        }
    }
}

using System.Collections.Generic;
using ExitGames.Client.Photon;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;

namespace Kumu.Kulitan.Tracking.Components
{
    public class VisitorCountTracker
    {
        private HashSet<string> uniquePlayerIdsJoined = new();
        private HashSet<string> playerIdsInRoom = new();

        private int maxConcurrentVisitors;
        private string localPlayerId;
        private bool isRoomOwner;
        private Slot<string> eventSlot;

        public VisitorCountTracker()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInstantiated);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerRemoved);
        }

        ~VisitorCountTracker()
        {
            eventSlot.Dispose();
        }

        public int GetUniqueVisitorsCount()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            currentRoom.CustomProperties.TryGetValue(Constants.UNIQUE_VISITORS_PROP_KEY, out var _uniqueVisitorsCount);
            var result = _uniqueVisitorsCount != null ? (int)_uniqueVisitorsCount : 0;
            return result;
        }

        public int GetMaxConcurrentVisitorsCount()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            currentRoom.CustomProperties.TryGetValue(Constants.MAX_CONCURRENT_VISITORS_PROP_KEY, out var _maxConcurrentCount);
            var result = _maxConcurrentCount != null ? (int)_maxConcurrentCount : 0;
            return result;
        }

        private void OnPlayerInstantiated(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerJoinedEvent)callback;
            localPlayerId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;
            var ownerId = ConnectionManager.Client.CurrentRoom.GetHostDetails().accountId;
            isRoomOwner = localPlayerId == ownerId;

            if (!isRoomOwner)
            {
                return;
            }

            if (!uniquePlayerIdsJoined.Contains(playerInstantiatedEvent.AccountId))
            {
                uniquePlayerIdsJoined.Add(playerInstantiatedEvent.AccountId);
            }

            playerIdsInRoom.Add(playerInstantiatedEvent.AccountId);
            UpdateMaxConcurrentCount();
            UpdateCustomRoomProperties();
        }

        private void OnPlayerRemoved(IEvent<string> callback)
        {
            var playerRemovedEvent = (QuantumPlayerRemovedEvent)callback;
            if (!isRoomOwner)
            {
                return;
            }

            playerIdsInRoom.Remove(playerRemovedEvent.AccountId);
            UpdateCustomRoomProperties();
            
            if (playerRemovedEvent.AccountId == localPlayerId)
            {
                isRoomOwner = false;
                uniquePlayerIdsJoined.Clear();
                playerIdsInRoom.Clear();
            }
        }

        private void UpdateCustomRoomProperties()
        {            
            var currentRoom = ConnectionManager.Client.CurrentRoom;

            if (ConnectionManager.Client.State != ClientState.Joined)
            {
                return;
            }

            var ht = new Hashtable();
            ht.Add(Constants.MAX_CONCURRENT_VISITORS_PROP_KEY, maxConcurrentVisitors);
            ht.Add(Constants.UNIQUE_VISITORS_PROP_KEY, uniquePlayerIdsJoined.Count);
            currentRoom.SetCustomProperties(ht);
        }

        private void UpdateMaxConcurrentCount()
        {
            if (maxConcurrentVisitors <= playerIdsInRoom.Count)
            {
                maxConcurrentVisitors = playerIdsInRoom.Count;
            }
        }
    }
}

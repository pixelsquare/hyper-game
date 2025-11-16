using System.Collections.Generic;
using System.Linq;
using Kumu.Extensions;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Lobby;
using Newtonsoft.Json;
using Photon.Realtime;
using UnityEngine;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutLobbyButtonController : MonoBehaviour
    {
        [Header("Room List Display")]
        [SerializeField] private RoomListGridAdapter gridAdapter;
        [SerializeField] private RoomLayoutConfigs roomLayoutConfigs;

        private Slot<string> eventSlot;
        private readonly List<RoomDetailsHolder> currentRoomDetails = new();

        public void EnableButtons(bool isEnabled)
        {
            gridAdapter.EnableButtons(isEnabled);
        }

        private void UpdateRoomList(Dictionary<string, RoomInfo> roomList)
        {
            currentRoomDetails.Clear();
            
            foreach (var room in roomList.Values)
            {
                if (!room.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
                {
                    "Missing room details prop key.".LogError();
                    continue;
                }
                
                if (!room.CustomProperties.TryGetValue(Constants.HOST_DETAILS_PROP_KEY, out var hostDetailsObj))
                {
                    "Missing host details prop key.".LogError();
                    continue;
                }

                var roomOnwer = "";
                if (!string.IsNullOrEmpty(hostDetailsObj.ToString()))
                {
                    var hostDetails = JsonConvert.DeserializeObject<PlayerDetails>(hostDetailsObj.ToString());
                    roomOnwer = hostDetails.userName;
                }

                var roomDetails = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());

                if (string.IsNullOrEmpty(roomDetails.previewIconAddress))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(roomDetails.roomName))
                {
                    roomDetails.roomName = $"Room {roomDetails.layoutName}";
                }

                var roomId = roomDetails.roomId ?? room.Name;
                
                var newRoomDetails = new RoomDetailsHolder(roomId, roomDetails.roomName, roomDetails.sceneName, 
                    roomOnwer,room.PlayerCount, room.MaxPlayers, roomDetails.previewIconAddress,
                        GetLevelConfig(roomDetails.sceneName));

                currentRoomDetails.Add(newRoomDetails);
            }            
        }        

        private void DisplaySortFilterRoomList(SortRoomMode sortMode, FilterRoomMode filterMode)
        {
            var sortRooms = LobbySort.GetSortRoomList(currentRoomDetails, sortMode);
            var filterRooms = LobbyFilter.GetFilterRoomList(sortRooms, filterMode);
            gridAdapter.SetItems(filterRooms.ToList());
        }

        [ContextMenu("Clear Display")]
        private void ClearCurrentDisplay()
        {
            currentRoomDetails.Clear();
            gridAdapter.SetItems(currentRoomDetails);
            
        }

        private LevelConfigScriptableObject GetLevelConfig(string sceneToLoad)
        {
            var layoutConfig = 
                roomLayoutConfigs.LayoutConfigs.First(item => item.SceneToLoad.Equals(sceneToLoad));
                
            return layoutConfig.LevelConfig;
        }

        private void RoomListUpdated(IEvent<string> callback)
        {
            var roomListUpdatedEvent = (RoomListUpdatedEvent)callback;
            var roomList = roomListUpdatedEvent.RoomList;

            UpdateRoomList(roomList);
            
            DisplaySortFilterRoomList(LobbySort.CurrentSortMode, LobbyFilter.CurrentFilterMode);
        }

        private void OnButtonsEnabled(IEvent<string> callback)
        {
            EnableButtons(true);
        }

        private void OnButtonsDisabled(IEvent<string> callback)
        {
            EnableButtons(false);
        }

        private void OnRoomsSorted(IEvent<string> callback)
        {
            var eventCallback = (SortFilterRoomEvent)callback;

            DisplaySortFilterRoomList(eventCallback.SortMode, eventCallback.FilterMode);
        }

        [ContextMenu("Mock Populate")]
        private void MockPopulate()
        {
            var mockRooms = new RoomDetailsHolder[]
            {
                new("1", "Room A1","", "room owner", 1, 32, "", null),
                new("2", "Room B2","", "room owner",2, 32, "", null),
                new("3", "Room C3","", "room owner",3, 32, "", null),
                new("4", "Room D4","", "room owner",4, 32, "", null),
                new("5", "Room E5","", "room owner",5, 32, "", null),
                new("6", "Room F6","", "room owner",6, 32, "", null),
                new("7", "Room G7","", "room owner",7, 32, "", null),
                new("8", "Room H8","", "room owner",7, 32, "", null),
                new("9", "Room I9","", "room owner",7, 32, "", null),
                new("10", "Room J10","", "room owner",7, 32, "", null),
                new("32", "Room Z32","", "room owner",32, 32, "", null),
            };

            currentRoomDetails.Clear();
            currentRoomDetails.AddRange(mockRooms);
        
            DisplaySortFilterRoomList(LobbySort.CurrentSortMode, LobbyFilter.CurrentFilterMode);
        }
        
        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(RoomListUpdatedEvent.EVENT_NAME, RoomListUpdated);
            eventSlot.SubscribeOn(HangoutJoinRoomBtnClickedEvent.EVENT_NAME, OnButtonsDisabled);
            eventSlot.SubscribeOn(HangoutJoinRoomFailedEvent.EVENT_NAME, OnButtonsEnabled);
            eventSlot.SubscribeOn(SortFilterRoomEvent.EVENT_NAME, OnRoomsSorted);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Lobby
{
    public class LobbyFilter : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<FilterRoomMode, Toggle> filterToggles;

        public const FilterRoomMode FILTER_DEFAULT = FilterRoomMode.MaxCapacity;
        
        public static FilterRoomMode CurrentFilterMode { get; private set; } = FILTER_DEFAULT;

        private FilterRoomMode selectedFilterMode;

        public void OnConfirm()
        {
            CurrentFilterMode = selectedFilterMode;
        }

        public void OnReset()
        {
            SelectDefaults();
        }

        private void OnToggle(bool isOn, FilterRoomMode filterMode)
        {
            selectedFilterMode += isOn
                ? (int)filterMode
                : -(int)filterMode;
        }

        private void Init()
        {
            selectedFilterMode = CurrentFilterMode;

            foreach (var pair in filterToggles)
            {
                var filterMode = pair.Key;
                var isFlagged = (filterMode & CurrentFilterMode) > 0;
                var toggle = pair.Value;
                
                toggle.SetIsOnWithoutNotify(isFlagged);
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener(isOn => OnToggle(isOn, filterMode));
            }
        }

        private void SelectDefaults()
        {
            selectedFilterMode = FILTER_DEFAULT;
            
            foreach (var pair in filterToggles)
            {
                var filterMode = pair.Key;
                var isFlagged = (filterMode & FILTER_DEFAULT) > 0;
                var toggle = pair.Value;
                
                toggle.SetIsOnWithoutNotify(isFlagged);
            }
        }

        private void Start()
        {
            Init();
        }      
        
        public static IEnumerable<RoomDetailsHolder> GetFilterRoomList(IEnumerable<RoomDetailsHolder> rooms, FilterRoomMode filterRoomMode)
        {
            var filterRooms = rooms;
            
            if ((filterRoomMode & FilterRoomMode.MaxCapacity) == 0)
            {
                filterRooms = filterRooms.Where(room => room.roomCount < room.maxPlayersCount).ToList();
            }

            return filterRooms;
        }
    }
        
    [Flags]
    public enum FilterRoomMode
    {
        None = 0b0000,
        MaxCapacity = 0b0001,
        FriendRooms = 0b0010,
        PasswordProtected = 0b0100,
    }
}

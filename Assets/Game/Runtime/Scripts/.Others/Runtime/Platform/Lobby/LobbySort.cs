using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Hangout;
using UnityEngine;
using UnityEngine.UI;

namespace Kumu.Kulitan.Lobby
{
    public class LobbySort : MonoBehaviour
    {
        [SerializeField] private SerializableDictionary<SortRoomMode, Toggle> sortToggles;

        public const SortRoomMode SORT_DEFAULT = SortRoomMode.PlayerCountDescending;

        public static SortRoomMode CurrentSortMode { get; private set; } 
            = SORT_DEFAULT;

        private SortRoomMode selectedSortMode;                

        public void OnConfirm()
        {
            CurrentSortMode = selectedSortMode;
        }

        public void OnReset()
        {
            selectedSortMode = SORT_DEFAULT;
            sortToggles[SORT_DEFAULT].isOn = true;
        }

        private void OnToggle(bool isOn, SortRoomMode sortMode)
        {
            if (isOn)
            {
                selectedSortMode = sortMode;
            }
        }

        private void Init()
        {
            selectedSortMode = CurrentSortMode;
            sortToggles[selectedSortMode].isOn = true;
            
            foreach (var pair in sortToggles)
            {
                var sortMode = pair.Key;
                pair.Value.onValueChanged.AddListener(isOn => OnToggle(isOn, sortMode));
            }            
        }

        private void SelectDefault()
        {
            
        }

        private void Start()
        {
            Init();
        }
        
        public static IEnumerable<RoomDetailsHolder> GetSortRoomList(IEnumerable<RoomDetailsHolder> rooms, SortRoomMode sortRoomMode)
        {
            if (sortRoomMode == SortRoomMode.PlayerCountDescending)
            {
                return rooms.OrderByDescending(room => room.roomCount).ToList();
            }
            if (sortRoomMode == SortRoomMode.PlayerCountAscending)
            {
                return rooms.OrderBy(room => room.roomCount).ToList();
            }
            if (sortRoomMode == SortRoomMode.AlphabeticalAscending)
            {
                return rooms.OrderBy(room => room.roomName).ToList();
            }
            if (sortRoomMode == SortRoomMode.AlphabeticalDescending)
            {
                return rooms.OrderByDescending(room => room.roomName).ToList();
            }

            return rooms;
        }
    }   
    
    public enum SortRoomMode
    {
        PlayerCountAscending,
        PlayerCountDescending,
        AlphabeticalAscending,
        AlphabeticalDescending,        
    }
}

using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.UI;
using Quantum;
using TMPro;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VGPlayerListController : MonoBehaviour // TODO (cj): consolidate with Tony's player list?
    {
        public static List<VirtualGiftGifteeData> allPlayersGifteeData = new();
        
        [SerializeField] private TextMeshProUGUI selectedPlayerText; 
        [SerializeField] private VGGiftListController[] listControllers;

        private Dictionary<string, PlayerRef> players = new();
        private Slot<string> eventSlot;

        private VGPlayerListPopup popup;
        private string selectedGifteeId;

        public void OpenPlayerListPopup()
        {
            popup = (VGPlayerListPopup)PopupManager.Instance.CreatePopup(PopupManager.PopupType.VGPlayerListPopup, 0);
            popup.Initialize(allPlayersGifteeData.ToArray(), selectedGifteeId);
            popup.AddCallback(OnPlayerSelected);
        }

        public void LoadPlayerList()
        {
            ClearDisplay();
            var isSelectedGifteeExisting = false;

            foreach (var player in players)
            {
                var f = QuantumRunner.Default.Game.Frames.Verified;
                var playerData = f.GetPlayerData(player.Value);
                var isLocal = QuantumRunner.Default.Game.PlayerIsLocal(player.Value);
                
                if (isLocal)
                {
                    continue;
                }

                var gifteeData = new VirtualGiftGifteeData
                {
                    id = playerData.accountId,
                    nickname = playerData.nickname,
                    username = playerData.username
                };

                if (selectedGifteeId == gifteeData.id)
                {
                    isSelectedGifteeExisting = true;
                }
                allPlayersGifteeData.Add(gifteeData);
            }

            var selectedGiftee = GetGifteeDataById(selectedGifteeId);

            if (selectedPlayerText != null)
            {
                selectedPlayerText.text = isSelectedGifteeExisting ? selectedGiftee.username : "SELECT USER";
            }

            // set if gift list is blocked or not
            SetGiftsListState(isSelectedGifteeExisting);
            
            if (popup != null)
            {
                popup.Initialize(allPlayersGifteeData.ToArray(), selectedGifteeId);
            }
        }

        public string[] GetPlayerIds()
        {
            return players.Select(p => QuantumRunner.Default.Game.Frames.Verified.GetPlayerData(p.Value))
                          .Select(playerData => playerData.accountId)
                          .ToArray();
        }

        private void OnPlayerSelected(VirtualGiftGifteeData gifteeData)
        {
            selectedGifteeId = gifteeData.id;
            selectedPlayerText.text = gifteeData.username;
            // set if gift list is blocked or not
            SetGiftsListState(true);
            GlobalNotifier.Instance.Trigger(new VGPlayerSelected(new []{gifteeData}));
            popup = null;
        }

        private void ResetPlayerSelected(string removedPlayerId)
        {
            if (string.IsNullOrWhiteSpace(selectedGifteeId) || 
                !selectedGifteeId.Equals(removedPlayerId))
            {
                return;
            }

            selectedGifteeId = "";
        }

        private VirtualGiftGifteeData GetGifteeDataById(string id)
        {
            var gifteeData = allPlayersGifteeData.Find(data => data.id.Equals(id));
            return gifteeData;
        }

        private void ClearDisplay()
        {
            allPlayersGifteeData.Clear();
            if (popup != null)
            {
                popup.Clear();
            }
        }

        private void SetGiftsListState(bool isEnabled)
        {
            foreach (var listController in listControllers)
            {
                listController.SetGiftListBlocker(!isEnabled);
            }
        }

        private void SetGiftsAvailability()
        {
            foreach (var listController in listControllers)
            {
                listController.SetGiftAvailability();
            }
        }

        private void OnPlayerJoined(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerJoinedEvent)callback;
            players.Add(playerInstantiatedEvent.AccountId, playerInstantiatedEvent.PlayerRef);
            LoadPlayerList();
            SetGiftsAvailability();
        }

        private void OnPlayerRemoved(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerRemovedEvent)callback;
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(playerInstantiatedEvent.PlayerRef);
            ResetPlayerSelected(playerData.accountId);
            players.Remove(playerData.accountId);
            LoadPlayerList();
            SetGiftsAvailability();
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoined);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerRemoved);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}

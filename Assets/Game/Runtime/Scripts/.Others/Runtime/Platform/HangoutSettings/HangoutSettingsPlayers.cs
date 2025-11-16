using System;
using System.Collections.Generic;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Multiplayer;
using Newtonsoft.Json;
using UnityEngine;

namespace Kumu.Kulitan.UI
{
    public class HangoutSettingsPlayers : MonoBehaviour
    {
        [SerializeField] private SettingsUserPanel settingsUserPanelRef;
        [SerializeField] private Transform contentPanel;

        private Slot<string> eventSlot;

        private readonly Dictionary<uint, SettingsUserPanel> userPanelMap = new();
        private readonly List<SettingsUserPanel> settingsUserPanelPool = new();

        private void Initialize()
        {
            var room = ConnectionManager.Client.CurrentRoom;

            settingsUserPanelPool.ForEach(a => a.gameObject.SetActive(false));

            foreach (var playerMap in room.Players)
            {
                var player = playerMap.Value;

                if (!player.CustomProperties.TryGetValue(Constants.PLAYER_DETAILS_PROP_KEY, out var playerDetailsObj))
                {
                    continue;
                }

                var playerDetails = JsonConvert.DeserializeObject<PlayerDetails>(playerDetailsObj.ToString());
                var localPlayerId = ConnectionManager.Client.LocalPlayer.UserId;

                if (string.Compare(playerDetails.accountId, localPlayerId, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    continue;
                }

                var userPanel = GetPooledUserPanel();
                userPanel.Initialize(playerDetails.accountId, playerDetails.playerId, playerDetails.userName);
                userPanel.gameObject.SetActive(true);

                userPanelMap.TryAdd(playerDetails.playerId, userPanel);
            }
        }

        private SettingsUserPanel GetPooledUserPanel()
        {
            foreach (var panel in settingsUserPanelPool)
            {
                if (!panel.IsInitialized)
                {
                    return panel;
                }
            }

            var newPanel = Instantiate(settingsUserPanelRef, contentPanel);
            settingsUserPanelPool.Add(newPanel);
            return newPanel;
        }

        private void OnPlayerJoinedEvent(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerJoinedEvent)callback;

            if (QuantumRunner.Default.Game.PlayerIsLocal(playerInstantiatedEvent.PlayerRef))
            {
                return;
            }

            if (!userPanelMap.TryGetValue(playerInstantiatedEvent.PlayerId, out var userPanel))
            {
                userPanel = GetPooledUserPanel();
            }

            userPanel.Initialize(playerInstantiatedEvent.AccountId, playerInstantiatedEvent.PlayerId, playerInstantiatedEvent.Username);
            userPanel.gameObject.SetActive(true);

            userPanelMap.TryAdd(playerInstantiatedEvent.PlayerId, userPanel);
        }

        private void OnPlayerRemovedEvent(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerRemovedEvent)callback;
            var f = QuantumRunner.Default.Game.Frames.Verified;
            var playerData = f.GetPlayerData(playerInstantiatedEvent.PlayerRef);

            if (!userPanelMap.ContainsKey(playerData.playerId))
            {
                return;
            }

            userPanelMap[playerData.playerId].gameObject.SetActive(false);
            userPanelMap.Remove(playerData.playerId);
        }

        private void Awake()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerJoinedEvent);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerRemovedEvent);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }

        private void Start()
        {
            Initialize();
        }
    }
}

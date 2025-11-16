using System;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.UI;
using Quantum;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class Reconnecter : BaseMatchmakingHandler
    {
        [SerializeField] private HangoutConnectionCallback connCallback;
        
        private int retriesCount;

        public void Initialize()
        {
            retriesCount = 0;
        }
        
        private void HandleFailure(string message, short returnCode = -1)
        {
            Debug.LogWarning(message + " " + returnCode);//TODO - leaving this here for now to check for possible network errors as dev continues
            
            retriesCount += 1;
            
            PopupManager.Instance.CloseAllPopups();

            if (returnCode == 32765)
            {
                var popup = PopupManager.Instance.OpenTextPopup("Error", $"Room is already full", "Menu");
                popup.OnClosed += connCallback.ReturnToMenu;
                return;
            }
            
            if (returnCode == 32758)
            {
                var popup = PopupManager.Instance.OpenTextPopup("Error", $"Room does not exist", "Menu");
                popup.OnClosed += connCallback.ReturnToMenu;
                return;
            }

            if (retriesCount >= 3)
            {
                var popup = (ConfirmationPopup)PopupManager.Instance.OpenConfirmationPopup("Error", $"Connection Failed", "Retry", "Menu");
                popup.OnConfirm += connCallback.AttemptReconnect;
                popup.OnCancel += connCallback.ReturnToMenu;
            }
            else
            {
                var popup = PopupManager.Instance.OpenTextPopup("Error", $"Connection Failed", "Menu");
                popup.OnClosed += connCallback.ReturnToMenu;
            }
        }
        
        protected override void StartMatch(RuntimeConfig runtimeConfig)
        {
            var startParams = new QuantumRunner.StartParameters
            {
                RuntimeConfig = runtimeConfig,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
                InitialFrame = 0,
                PlayerCount = RoomConnectionDetails.Instance.maxPlayers,
                LocalPlayerCount = 1,
                RecordingFlags = RecordingFlags.None,
                NetworkClient = ConnectionManager.Client,
            };

            var clientId =
                ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, ConnectionManager.Client);
            QuantumRunner.StartGame(clientId, startParams);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            HandleFailure("[HangoutConnectionCallback] OnCreateRoomFailed " + message);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            ConnectionManager.Instance.CurrentLevelConfig.RerollSeed();
                
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, (callback) =>
            {
                var joinedEventCallback = (QuantumPlayerJoinedEvent)callback;
                if (!joinedEventCallback.IsLocal)
                {
                    return;
                }
                    
                eventSlot.UnSubscribeAllOn(QuantumPlayerJoinedEvent.EVENT_NAME);
            });

            StartMatch(ConnectionManager.Instance.CurrentLevelConfig.Config);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            HandleFailure("[HangoutConnectionCallback] OnJoinRoomFailed " + message, returnCode);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            HandleFailure("[HangoutConnectionCallback] OnJoinRandomFailed " + message);
        }
        
        private void Start()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
        }
        
        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}

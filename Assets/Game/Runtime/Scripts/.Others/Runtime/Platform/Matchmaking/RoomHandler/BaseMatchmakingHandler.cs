using System.Collections.Generic;
using ExitGames.Client.Photon;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.Social;
using Kumu.Kulitan.Tracking;
using Kumu.Kulitan.UI;
using Newtonsoft.Json;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using UnityEngine;
using Constants = Kumu.Kulitan.Common.Constants;

namespace Kumu.Kulitan.Multiplayer
{
    public abstract class BaseMatchmakingHandler : MonoBehaviour, IMatchmakingCallbacks
    {
        private static RoomStatus state = RoomStatus.OUT_OF_ROOM;

        public static RoomStatus State
        {
            get => state;
            private set
            {
                "Setting Room State".Log();
                state = value;
            }
        }

        protected RoomDetails roomDetails;
        protected RuntimeConfig runtimeConfig;
        protected Slot<string> eventSlot;

        public enum RoomStatus
        {
            OUT_OF_ROOM,
            IN_ROOM
        };

        public void CreateRoom()
        {
            if (!ConnectionManager.Client.InLobby)
            {
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Not connected.", "Ok");
                return;
            }

            var enterRoomParam = CreateEnterRoomParam(roomDetails, runtimeConfig);

            RoomConnectionDetails.Instance.enterRoomParams = enterRoomParam;

            if (!ConnectionManager.Client.OpCreateRoom(enterRoomParam))
            {
                "Disconnected by create room failure".Log();
                ConnectionManager.Client.Disconnect();
            }

            Debug.LogWarning("CREATE ROOM STATE " + ConnectionManager.Client.State);
        }

        public virtual void JoinRoom()
        {
            if (!ConnectionManager.Client.InLobby)
            {
                var popup = PopupManager.Instance.OpenErrorPopup("Error", "Not connected.", "Ok");
                return;
            }

            var enterRoomParam = CreateEnterRoomParam(roomDetails, runtimeConfig);

            RoomConnectionDetails.Instance.enterRoomParams = enterRoomParam;

            if (!ConnectionManager.Client.OpJoinRoom(enterRoomParam))
            {
                "Disconnected by join room failure".Log();
                ConnectionManager.Client.Disconnect();
            }

            Debug.LogWarning("JOIN ROOM STATE " + ConnectionManager.Client.State);
        }

    #region MatchmakingCallbacks

        public virtual void OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        public virtual void OnCreatedRoom()
        {
            SetRoomDetails();
            SetHostDetails();
        }

        public virtual void OnCreateRoomFailed(short returnCode, string message)
        {
        }

        public virtual void OnJoinedRoom()
        {
            SetPlayerDetails();
            State = RoomStatus.IN_ROOM;
        }

        public virtual void OnJoinRoomFailed(short returnCode, string message)
        {
        }

        public virtual void OnJoinRandomFailed(short returnCode, string message)
        {
        }

        public virtual void OnLeftRoom()
        {
            CTokenSource.DisposeAll(true);
            State = RoomStatus.OUT_OF_ROOM;
            
            // I would have preferred to call this in an overriden HangoutMatchmakingHandler.OnLeftRoom()
            // but it does not get invoked for whatever reason - cj
            GlobalNotifier.Instance.Trigger(new LeaveHangoutEvent());
        }

    #endregion

        protected virtual EnterRoomParams CreateEnterRoomParam(RoomDetails roomDetails, RuntimeConfig runtimeConfig)
        {
            return new EnterRoomParams
            {
                RoomName = roomDetails.roomId,
                RoomOptions = new RoomOptions
                {
                    PublishUserId = true,
                    IsVisible = !roomDetails.isFriendsOnly,
                    MaxPlayers = CapMaxPlayers(runtimeConfig.maxPlayers), // Can also be extracted outside runtimeConfig
                    Plugins = new[] { "QuantumPlugin" },
                    CustomRoomPropertiesForLobby = new[]
                    {
                        Constants.ROOM_DETAILS_PROP_KEY,
                        Constants.HOST_DETAILS_PROP_KEY,
                        Constants.ROOM_PASS_PROP_KEY
                    },
                    CustomRoomProperties = new Hashtable
                    {
                        { Constants.ROOM_DETAILS_PROP_KEY, JsonConvert.SerializeObject(roomDetails) },
                        { Constants.HOST_DETAILS_PROP_KEY, "" },
                        { Constants.ROOM_PASS_PROP_KEY, "" }
                    },
                    PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000
                }
            };
        }

        protected virtual void StartMatch(RuntimeConfig runtimeConfig)
        {
            var startParams = new QuantumRunner.StartParameters
            {
                RuntimeConfig = runtimeConfig,
                DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
                GameMode = DeterministicGameMode.Multiplayer,
                InitialFrame = 0,
                PlayerCount = ConnectionManager.Client.CurrentRoom.MaxPlayers,
                LocalPlayerCount = 1,
                RecordingFlags = RecordingFlags.None,
                NetworkClient = ConnectionManager.Client
            };

            var clientId =
                    ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, ConnectionManager.Client);
            QuantumRunner.StartGame(clientId, startParams);

            RoomConnectionDetails.Instance.maxPlayers = startParams.PlayerCount;
            RoomConnectionDetails.Instance.clientId = clientId;
        }

        protected virtual void AddCallbackTarget()
        {
            ConnectionManager.Client.AddCallbackTarget(this);
        }

        protected virtual void RemoveCallbackTarget()
        {
            ConnectionManager.Client.RemoveCallbackTarget(this);
        }

        protected virtual void OnEnable()
        {
            AddCallbackTarget();
        }

        protected virtual void OnDisable()
        {
            RemoveCallbackTarget();
        }

        private void SetRoomDetails()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var customProps = currentRoom.CustomProperties;

            if (!customProps.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
            {
                "Unable to update RoomId. Missing room details property.".LogError();
                return;
            }

            // Updates unique room id on room custom properties.
            var details = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());
            details.roomId = currentRoom.Name;
            customProps[Constants.ROOM_DETAILS_PROP_KEY] = JsonConvert.SerializeObject(details);
            currentRoom.SetCustomProperties(customProps);
            
            SocialManager.Instance.RegisterCreatedRoom(details.roomId, details.isFriendsOnly);
        }

        private void SetHostDetails()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var customProps = currentRoom.CustomProperties;

            if (!ConnectionManager.Client.LocalPlayer.IsMasterClient || !customProps.ContainsKey(Constants.HOST_DETAILS_PROP_KEY))
            {
                return;
            }

            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            var playerDetails = new PlayerDetails
            {
                accountId = userProfile.accountId,
                playerId = userProfile.playerId,
                userName = userProfile.userName.ToString(),
                nickName = userProfile.nickName
            };

            customProps[Constants.HOST_DETAILS_PROP_KEY] = JsonConvert.SerializeObject(playerDetails);
            currentRoom.SetCustomProperties(customProps);
        }

        private void SetPlayerDetails()
        {
            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();

            var playerDetails = new PlayerDetails
            {
                accountId = userProfile.accountId,
                playerId = userProfile.playerId,
                userName = userProfile.userName.ToString(),
                nickName = userProfile.nickName
            };

            var currentplayer = ConnectionManager.Client.LocalPlayer;
            var customProps = currentplayer.CustomProperties;

            var playerDetailsString = JsonConvert.SerializeObject(playerDetails);

            if (!customProps.TryAdd(Constants.PLAYER_DETAILS_PROP_KEY, playerDetailsString))
            {
                customProps[Constants.PLAYER_DETAILS_PROP_KEY] = playerDetailsString;
            }

            currentplayer.SetCustomProperties(customProps);
        }

        /// <summary>
        /// Cap max players due to Photon limits when on the free tier.
        /// </summary>
        private byte CapMaxPlayers(int value)
        {
            var maxPlayers = value;
#if UBE_DEV || UBE_STAGING
            maxPlayers = Mathf.Min(20, value);
#endif
            return (byte)maxPlayers;
        }
    }
}

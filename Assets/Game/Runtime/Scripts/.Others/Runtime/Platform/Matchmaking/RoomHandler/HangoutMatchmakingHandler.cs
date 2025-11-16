using System.Linq;
using ExitGames.Client.Photon;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Hangout;
using Kumu.Kulitan.UI;
using Kumu.Kulitan.Social;
using Kumu.Kulitan.Tracking;
using Newtonsoft.Json;
using Photon.Realtime;
using Quantum;
using UnityEngine;
using Constants = Kumu.Kulitan.Common.Constants;

namespace Kumu.Kulitan.Multiplayer
{
    public class HangoutMatchmakingHandler : BaseMatchmakingHandler
    {
        private string sceneToLoad;
        private bool hasMinigame;
        private string previewIconAddressableAddress;

        public void CreateRandomFriendsOnlyRoom()
        {
#if USES_MOCKS
            sceneToLoad = "Diorama-Quantum";
            var roomDetails = new RoomDetails();
            roomDetails.roomId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId + "#" +
                                 Random.Range(0,1000);
            roomDetails.roomName = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId + "'s room";
            roomDetails.layoutName = "";
            roomDetails.sceneName = "Diorama-Quantum";
            roomDetails.previewIconAddress = null;
            roomDetails.isFriendsOnly = true;
            var config = Resources.Load("MockedSocialService/PlaceholderConfig") as LevelConfigScriptableObject;
            InitRoom(roomDetails, config);
            CreateRoom();
#endif
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            
            var currentRoom = ConnectionManager.Client.CurrentRoom;
            var roomDetails = currentRoom.GetRoomDetails();
            var hostDetails = currentRoom.GetHostDetails();

            //following condition is to be able to handle direct room joining
            if (string.IsNullOrEmpty(sceneToLoad))
            {
                sceneToLoad = roomDetails.sceneName;
                
                var lvlConfig = SocialManager.Instance.RoomLayoutConfigs.LayoutConfigs.First(item => item.SceneToLoad.Equals(sceneToLoad)).LevelConfig;
                hasMinigame = lvlConfig.HasMinigame;
                InitRoom(roomDetails, lvlConfig);
            }

            ConnectionManager.Instance.CurrentLevelConfig.RerollSeed();
            SceneLoadingManager.Instance.LoadHangoutSharedScene(() => HangoutGameLoader.Instance.LoadGameScene(sceneToLoad), hasMinigame);
            
            var playerId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;
            if (playerId == hostDetails.accountId)
            {
                GlobalNotifier.Instance.Trigger(new StartHangoutEvent(playerId, currentRoom));
            }
            else
            {
                GlobalNotifier.Instance.Trigger(new JoinHangoutEvent(playerId, currentRoom));
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            var popup = PopupManager.Instance.OpenErrorPopup("Error", "Failed to join room.", "Ok");
            popup.OnClosed += () =>
            {
                GlobalNotifier.Instance.Trigger(new HangoutJoinRoomFailedEvent());

                if (ConnectionManager.Client.IsConnected)
                {
                    ConnectionManager.Client.OpJoinLobby(RoomConnectionDetails.Instance.myLobby);
                }
            };
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            $"Create room failed [{returnCode}]: '{message}'".LogError();
            var popup = PopupManager.Instance.OpenErrorPopup("Error", "Failed to create room.", "Ok");
            popup.OnClosed += () => GlobalNotifier.Instance.Trigger(new HangoutJoinRoomFailedEvent());
        }

        protected override EnterRoomParams CreateEnterRoomParam(RoomDetails roomDetails, RuntimeConfig runtimeConfig)
        {
            roomDetails.previewIconAddress = previewIconAddressableAddress;
            var roomParams = base.CreateEnterRoomParam(roomDetails, runtimeConfig);
            var roomOptions = roomParams.RoomOptions;
            
            //TODO: update whenever the UI is ready for opting for friends-only rooms
            roomOptions.IsVisible = !roomDetails.isFriendsOnly;

            roomOptions.CustomRoomPropertiesForLobby = new[]
            {
                Constants.ROOM_DETAILS_PROP_KEY,
                Constants.HOST_DETAILS_PROP_KEY,
                Constants.ROOM_PASS_PROP_KEY
            };
            roomOptions.CustomRoomProperties = new Hashtable
            {
                { Constants.ROOM_DETAILS_PROP_KEY, JsonConvert.SerializeObject(roomDetails) },
                { Constants.HOST_DETAILS_PROP_KEY, "" },
                { Constants.ROOM_PASS_PROP_KEY, "" }
            };
            return roomParams;
        }

        private void InitRoom(RoomDetails roomDetails, LevelConfigScriptableObject levelConfig)
        {
            this.roomDetails = roomDetails;
            levelConfig.CustomPlayerInitialData.Reset();
            runtimeConfig = levelConfig.Config;
            previewIconAddressableAddress = levelConfig.PreviewIconSpriteAddressableAddress;
            ConnectionManager.Instance.CurrentLevelConfig = levelConfig;
        }

        private void OnCreateRoom(IEvent<string> callback)
        {
            var hangoutRoomCreatedEvent = (HangoutCreateRoomBtnClickedEvent)callback;
            sceneToLoad = hangoutRoomCreatedEvent.LevelConfig.SceneToLoad;
            hasMinigame = hangoutRoomCreatedEvent.LevelConfig.HasMinigame;
            InitRoom(hangoutRoomCreatedEvent.RoomDetails, hangoutRoomCreatedEvent.LevelConfig);
            CreateRoom();
        }

        private void OnJoinRoom(IEvent<string> callback)
        {
            var hangoutRoomJoinedEvent = (HangoutJoinRoomBtnClickedEvent)callback;
            sceneToLoad = hangoutRoomJoinedEvent.LevelConfig.SceneToLoad;
            hasMinigame = hangoutRoomJoinedEvent.LevelConfig.HasMinigame;
            InitRoom(hangoutRoomJoinedEvent.RoomDetails, hangoutRoomJoinedEvent.LevelConfig);
            JoinRoom();
        }

        private void Start()
        {
            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(HangoutJoinRoomBtnClickedEvent.EVENT_NAME, OnJoinRoom);
            eventSlot.SubscribeOn(HangoutCreateRoomBtnClickedEvent.EVENT_NAME, OnCreateRoom);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();
        }
    }
}

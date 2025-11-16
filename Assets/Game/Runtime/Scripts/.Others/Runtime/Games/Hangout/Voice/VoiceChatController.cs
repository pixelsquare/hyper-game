using System;
using agora_gaming_rtc;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Moderation;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if USES_MOCKS
using AgoraIO.Media;
#endif

namespace Kumu.Kulitan.Hangout
{
    /// <summary>
    /// Handles the initialization of voice engine using Agora Voice SDK.
    /// Note: Initializing of Engine should be on Awake to avoid race condition.
    /// </summary>
    public class VoiceChatController : MonoBehaviour
    {
        [SerializeField] private AgoraAppConfigScriptableObject agoraAppConfig;
        [SerializeField] private SpatialAudioHandler spatialAudioHandler;
        [SerializeField] private VoiceMuteHandler muteHandler;
        [SerializeField] private ChatGroupManager chatManager;
        [SerializeField] private Button muteButton;

        [Header("Debug Params")]
        [SerializeField] private bool canConnectWithJustOne;

        [SerializeField] private bool forceJoinError;
        [SerializeField] private bool dontDisconnectOnLonePlayer;

        private string channel;
        private int? actorId;
        private Slot<string> eventSlot;

        private bool hasInitializedLocal;
        private bool attemptedConnection;
        private bool isConnected;

        private void InitializeVoiceEngine()
        {
#if AGORA_RELEASE
            AudioVideoChatHelper.InitializeEngine(agoraAppConfig.ReleaseAppID);
#else
            AudioVideoChatHelper.InitializeEngine(agoraAppConfig.DevAppID);
#endif
            AudioVideoChatHelper.GetRTCEngine().OnJoinChannelSuccess += OnJoinChannelSuccessHandler;
            AudioVideoChatHelper.GetRTCEngine().OnUserJoined += OnUserJoined;
            AudioVideoChatHelper.GetRTCEngine().OnUserOffline += OnUserLeft;
            AudioVideoChatHelper.GetRTCEngine().OnError += OnErrorHandler;
            AudioVideoChatHelper.GetRTCEngine().OnVolumeIndication += spatialAudioHandler.OnVolumeIndicationHandler;
            AudioVideoChatHelper.GetRTCEngine().OnTokenPrivilegeWillExpire += OnTokenWillExpire;
            AudioVideoChatHelper.GetRTCEngine().OnLeaveChannel += OnLeaveChannel;
            AudioVideoChatHelper.GetRTCEngine().OnConnectionStateChanged += OnConnectionStateChanged;

            //Set Spatial Audio position indicator
            AudioVideoChatHelper.EnableSpatialAudio(true);

            ConnectionManager.OnDisconnect += OnDisconnectedFromQuantum;

            AudioSettingsApplicator.Instance.SetInitialVoiceVolume();
        }

        public void OnGlobalVoiceAudio()
        {
            AudioVideoChatHelper.EnableSpatialAudio(true);
        }

        private void OnJoinChannelSuccessHandler(string channelName, uint uID, int elapsed)
        {
            chatManager.AddTextToDisplay("You have connected to voice chat!", ChatMessage.MessageType.Info);

            AudioVideoChatHelper.GetRTCEngine().SetEnableSpeakerphone(true);
            AudioVideoChatHelper.GetRTCEngine().AdjustRecordingSignalVolume(150);

            AudioSettingsApplicator.Instance.SetInitialVoiceVolume();

            muteButton.interactable = true;
            isConnected = true;
        }

        private void OnLeaveChannel(RtcStats stats)
        {
            isConnected = false;

            muteHandler.OnRequestMuteLocalAudio(true);
            muteButton.interactable = false;
            attemptedConnection = false; //reset to ensure token is requested only once per connect/reconnect with 2
            chatManager.AddTextToDisplay("You have left voice chat!", ChatMessage.MessageType.Info);
        }

        private void OnUserJoined(uint uID, int elapsed)
        {
            spatialAudioHandler.OnPlayerJoinedChannel(uID);
        }

        private void OnTokenWillExpire(string token)
        {
            RenewToken();
        }

        private void OnUserLeft(uint uID, USER_OFFLINE_REASON reason)
        {
            spatialAudioHandler.OnPlayerLeaveChannel(uID);
        }

        private void OnConnectionStateChanged(CONNECTION_STATE_TYPE state, CONNECTION_CHANGED_REASON_TYPE reason)
        {
            if (reason == CONNECTION_CHANGED_REASON_TYPE.CONNECTION_CHANGED_TOKEN_EXPIRED)
            {
                RenewToken();
            }
        }

        private void OnErrorHandler(int error, string msg)
        {
            $"[VoiceChatController]: {error} {msg}".LogError();

            if (error != 1017)
            {
                chatManager.AddTextToDisplay($"Voice chat error - {error} | {msg}", ChatMessage.MessageType.Error);
            }
            else
            {
                chatManager.AddTextToDisplay("Recording device issue, please rejoin room if voice chat is not working.", ChatMessage.MessageType.Info);
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                AudioVideoChatHelper.GetRTCEngine().Pause();
            }
            else
            {
                AudioVideoChatHelper.GetRTCEngine().Resume();
            }
        }

        public async void RequestAgoraTokenFromServer(string channel, UnityAction<string> callback)
        {
            chatManager.AddTextToDisplay("Connecting to voice chat...", ChatMessage.MessageType.Info);

            var request = new GetRTCTokenRequest();
            request.channel = channel;

            var tokenResult = await Services.AgoraService.GetRTCTokenAsync(request);

            if (tokenResult.HasError || forceJoinError)
            {
                $"[VoiceChatController]: Error joining room - {tokenResult.Error}".Log();

                var errorMsg = tokenResult.Error.Code switch
                {
                    ServiceErrorCodes.ACCOUNT_BANNED_LOGIN => "You are currently banned from chatting.",
                    ServiceErrorCodes.ACCOUNT_BANNED_AGORA => "You are currently banned from chatting.",
                    _ => "Cannot connect to voice chat. Reconnect from main menu to join."
                };

                chatManager.AddTextToDisplay(errorMsg, ChatMessage.MessageType.Error);
            }
            else
            {
                callback?.Invoke(tokenResult.Result.token);
            }
        }

        private void OnPlayerInitialized(IEvent<string> callback)
        {
            var playerInitEvent = (QuantumPlayerJoinedEvent)callback;
            if (ConnectionManager.Client.CurrentRoom == null)
            {
                return;
            }

            channel = ConnectionManager.Client.CurrentRoom.Name;

            if (playerInitEvent.IsLocal)
            {
                hasInitializedLocal = false;
                isConnected = false;

                //force audio routing
                AudioVideoChatHelper.ForceSettings();

#if USES_MOCKS
                //mock services used, need to be able to join channel with in-app generated token to be able to use rooms
                var f = QuantumRunner.Default.Game.Frames.Verified;
                actorId = f.PlayerToActorId(playerInitEvent.PlayerRef);
                var uid = Convert.ToUInt32(actorId);

                var token = RtcTokenBuilder.buildTokenWithUID(agoraAppConfig.DevAppID,
                        agoraAppConfig.DevCertificate, channel, uid, RtcTokenBuilder.Role.RolePublisher,
                        (uint)ConvertToUnixTimestamp(DateTime.Now) + 600);

                AudioVideoChatHelper.PlayerJoinChannelWithToken(token, channel, "", uid);
                muteHandler.OnRequestMuteLocalAudio(true);
                hasInitializedLocal = true;
                attemptedConnection = true;
#endif

                spatialAudioHandler.SetLocalPlayer(playerInitEvent.PlayerTransform);
            }
            else
            {
                spatialAudioHandler.OnQuantumInitializePlayer(playerInitEvent.PlayerTransform, playerInitEvent.PlayerId, playerInitEvent.PlayerRef._index);
            }

#if !USES_MOCKS
            if (!attemptedConnection && (ConnectionManager.Client.CurrentRoom.Players.Count > 1 || canConnectWithJustOne))
            {
                RequestAgoraTokenFromServer(channel, token =>
                {
                    AudioVideoChatHelper.PlayerJoinChannelWithToken(token, channel, "", UserProfileLocalDataManager.Instance.GetLocalUserProfile().playerId);
                    muteHandler.OnRequestMuteLocalAudio(true);
                    hasInitializedLocal = true;
                });
                attemptedConnection = true;
            }
#endif

            var isBlocked = ModerationManager.Instance.IsPlayerBlocked(playerInitEvent.AccountId);
            AudioVideoChatHelper.MuteOtherUser(playerInitEvent.PlayerId, isBlocked);
        }

        private void OnPlayerDeinitialized(IEvent<string> callback)
        {
            var playerDestroyedEvent = (QuantumPlayerRemovedEvent)callback;
            try
            {
                var f = QuantumRunner.Default.Game.Frames.Verified;
                var actorID = f.PlayerToActorId(playerDestroyedEvent.PlayerRef);
                var uid = Convert.ToUInt32(actorID);
                spatialAudioHandler.OnPlayerLeaveChannel(uid);
            }
            catch (Exception e)
            {
                $"Error in deinitializing player {playerDestroyedEvent.PlayerRef}: {e.GetBaseException().Message}".LogError();
            }

            if (ConnectionManager.Client.CurrentRoom != null && ConnectionManager.Client.CurrentRoom.Players.Count < 2 && !dontDisconnectOnLonePlayer)
            {
                AudioVideoChatHelper.GetRTCEngine().LeaveChannel();
            }
        }

        private void OnPlayerBlocked(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (PlayerBlockedEvent)callback;
            AudioVideoChatHelper.MuteOtherUser(playerInstantiatedEvent.PlayerId, playerInstantiatedEvent.IsBlocked);
        }

        private void OnDisconnectedFromQuantum()
        {
            if (isConnected)
            {
                AudioVideoChatHelper.GetRTCEngine().LeaveChannel();
            }
        }

        private void RenewToken()
        {
            if (ConnectionManager.Client.CurrentRoom.Players.Count <= 0)
            {
                return;
            }

            channel = ConnectionManager.Client.CurrentRoom.Name;

#if USES_MOCKS
            var uid = Convert.ToUInt32(actorId);
            var token = RtcTokenBuilder.buildTokenWithUID(agoraAppConfig.DevAppID,
                    agoraAppConfig.DevCertificate, channel, uid, RtcTokenBuilder.Role.RolePublisher,
                    (uint)ConvertToUnixTimestamp(DateTime.Now) + 600);
            AudioVideoChatHelper.RenewToken(token);
#else
            RequestAgoraTokenFromServer(channel, token =>
            {
                AudioVideoChatHelper.RenewToken(token);
                muteHandler.OnRequestMuteLocalAudio(true);
            });
#endif
        }

        //used when generating tokens internally
        public static double ConvertToUnixTimestamp(DateTime date)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        private void Awake()
        {
            InitializeVoiceEngine();

            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInitialized);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerDeinitialized);
            eventSlot.SubscribeOn(PlayerBlockedEvent.EVENT_NAME, OnPlayerBlocked);

            muteButton.interactable = false;
        }

        /// <summary>
        /// Destroys the created Engine aligned to Quantum roomId
        /// </summary>
        private void OnDestroy()
        {
            if (isConnected)
            {
                AudioVideoChatHelper.GetRTCEngine().LeaveChannel();
            }

            if (AudioVideoChatHelper.GetRTCEngine() != null)
            {
                AudioVideoChatHelper.DestroyActiveEngine();
            }

            eventSlot.Dispose();

            AudioVideoChatHelper.GetRTCEngine().OnJoinChannelSuccess -= OnJoinChannelSuccessHandler;
            AudioVideoChatHelper.GetRTCEngine().OnUserJoined -= OnUserJoined;
            AudioVideoChatHelper.GetRTCEngine().OnUserOffline -= OnUserLeft;
            AudioVideoChatHelper.GetRTCEngine().OnError -= OnErrorHandler;
            AudioVideoChatHelper.GetRTCEngine().OnVolumeIndication -= spatialAudioHandler.OnVolumeIndicationHandler;
            AudioVideoChatHelper.GetRTCEngine().OnTokenPrivilegeWillExpire -= OnTokenWillExpire;
            AudioVideoChatHelper.GetRTCEngine().OnLeaveChannel -= OnLeaveChannel;
            AudioVideoChatHelper.GetRTCEngine().OnConnectionStateChanged -= OnConnectionStateChanged;

            ConnectionManager.OnDisconnect -= OnDisconnectedFromQuantum;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
            {
                AudioSettingsApplicator.Instance.SetInitialVoiceVolume();
            }
        }
    }
}

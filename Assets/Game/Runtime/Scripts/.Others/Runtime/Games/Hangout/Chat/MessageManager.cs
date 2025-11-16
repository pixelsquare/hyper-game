using System;
using System.Collections;
using System.Collections.Generic;
using agora_rtm;
using Kumu.Extensions;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Events;
using Kumu.Kulitan.Moderation;
using Kumu.Kulitan.Multiplayer;
using Kumu.Kulitan.Tracking;
using Newtonsoft.Json;
using Quantum;
using TMPro;
using UnityEngine;
using Constants = Kumu.Kulitan.Common.Constants;
using SendMessageOptions = agora_rtm.SendMessageOptions;
#if USES_MOCKS
using AgoraIO.Rtm;
#endif

namespace Kumu.Kulitan.Hangout
{
    public class MessageManager : MonoBehaviour
    {
        [Header("Agora Properties")]
        [SerializeField] private AgoraAppConfigScriptableObject agoraAppConfig;

        [SerializeField]
        private int timeout = 1446455471;

        [Header("Application Properties")]
        [SerializeField]
        private TMP_InputField channelMsgInputBox;

        [SerializeField] private ChatGroupManager groupManager;

        private RtmClient rtmClient;
        private RtmChannel channel;

        private RtmClientEventHandler clientEventHandler;
        private RtmChannelEventHandler channelEventHandler;

        private Slot<string> eventSlot;

        private string username;
        private string playerId;
        private string channelName = "";

        private SerializableDictionary<string, TextChatMapping> playerMapping;
        private readonly HashSet<string> uniquePlayerAccountIds = new(); // Used for analytics;

        private SendMessageOptions _MessageOptions = new()
        {
            enableOfflineMessaging = true,
            enableHistoricalMessaging = true
        };

        public void Login()
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(agoraAppConfig.DevAppID) || string.IsNullOrEmpty(agoraAppConfig.ReleaseAppID))
            {
                "[MessageManager] No username or app ID specified.".LogError();
                return;
            }

#if USES_MOCKS
            var token = RtmTokenBuilder.buildToken(agoraAppConfig.DevAppID, agoraAppConfig.DevCertificate, username, uint.Parse(timeout + ""));
            $"[MessageManager] Generated temporary token: {token}".LogError();
            rtmClient.Login(token, username);
#else
            RequestAgoraTokenFromServer();
#endif
        }

        public void Logout()
        {
            groupManager.AddTextToDisplay("You have logged out of text chat", ChatMessage.MessageType.Info);
            rtmClient.Logout();
        }

        public void JoinChannel()
        {
            channelName = ConnectionManager.Client.CurrentRoom.Name;
            channel = rtmClient.CreateChannel(channelName, channelEventHandler);
            channel.Join();
        }

        public void LeaveChannel()
        {
            if (channel != null)
            {
                channel.Leave();
            }
        }

        public void SendMessageToChannel()
        {
            var msg = channelMsgInputBox.text;

            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            $"SendMessageToChannel: {username}".Log();
            var displayMsg = string.Format($"[{GetDisplayName(username, true)}]: {msg}");

            groupManager.AddTextToDisplay(displayMsg, ChatMessage.MessageType.PlayerMessage);
            channel.SendMessage(rtmClient.CreateMessage(msg));
            SendMessageAnalytics();
        }

        public async void RequestAgoraTokenFromServer()
        {
            var request = new GetRTMTokenRequest();
            var tokenResult = await Services.AgoraService.GetRTMTokenAsync(request);

            if (tokenResult.HasError)
            {
                $"[MessageManager] Failed to join channel - {tokenResult.Error}".Log();

                var errorMsg = tokenResult.Error.Code switch
                {
                    ServiceErrorCodes.ACCOUNT_BANNED_LOGIN => "You are currently banned from chatting.",
                    ServiceErrorCodes.ACCOUNT_BANNED_AGORA => "You are currently banned from chatting.",
                    _ => "Cannot connect to text chat. Reconnect from main menu to join."
                };

                groupManager.AddTextToDisplay(errorMsg, ChatMessage.MessageType.Error);
            }
            else
            {
                rtmClient.Login(tokenResult.Result.token, UserProfileLocalDataManager.Instance.GetLocalUserProfile().userName.ToString());
            }
        }

    #region RTM Callbacks

        private void OnJoinSuccessHandler(int id)
        {
            var msg = "channel:" + channelName + " OnJoinSuccess id = " + id;
            groupManager.AddTextToDisplay("You have successfully joined text chat.", ChatMessage.MessageType.Info);
        }

        private void OnJoinFailureHandler(int id, JOIN_CHANNEL_ERR errorCode)
        {
            $"[MessageManager] Failed to join channel - {errorCode}".LogError();
            groupManager.AddTextToDisplay("Failed to join text chat - can't join room.", ChatMessage.MessageType.Error);
        }

        private void OnClientLoginSuccessHandler(int id)
        {
            "[MessageManager] Login success".Log();
            JoinChannel();
        }

        private void OnClientLoginFailureHandler(int id, LOGIN_ERR_CODE errorCode)
        {
            $"[MessageManager] Failed to log-in - {errorCode}".LogError();
            groupManager.AddTextToDisplay("Failed to join text chat - login fail.", ChatMessage.MessageType.Error);
        }

        private void OnLeaveHandler(int id, LEAVE_CHANNEL_ERR errorCode)
        {
            if (errorCode != LEAVE_CHANNEL_ERR.LEAVE_CHANNEL_ERR_OK)
            {
                $"[MessageManager] Leave channel error - {errorCode}".LogError();
            }
        }

        private void OnChannelMessageReceivedHandler(int id, string userId, TextMessage message)
        {
            $"Received message from {userId} : {message}".Log();

            var playerId = GetPlayerId(userId);
            if (ModerationManager.Instance.IsPlayerBlocked(playerId))
            {
                return;
            }

            groupManager.AddTextToDisplay($"[{GetDisplayName(userId, true)}]: {message.GetText()}", ChatMessage.MessageType.ChannelMessage);
        }

        private void OnMemberJoinedHandler(int id, RtmChannelMember member)
        {
            groupManager.AddTextToDisplay($"{GetDisplayName(member.GetUserId(), true)} ({GetDisplayName(member.GetUserId(), false)}) has joined text chat!", ChatMessage.MessageType.Info);
        }

        private void OnMemberLeftHandler(int id, RtmChannelMember member)
        {
            var memberUsername = GetDisplayName(member.GetUserId(), false);
            groupManager.AddTextToDisplay($"{GetDisplayName(member.GetUserId(), true)} ({memberUsername}) has left text chat.", ChatMessage.MessageType.Info);
            // remove player from mapping once player has disconnected
            $"{memberUsername} removed from mapping".Log();
            playerMapping.Remove(memberUsername);
        }

        private void OnSendMessageResultHandler(int id, long messageId, CHANNEL_MESSAGE_ERR_CODE errorCode)
        {
            if (errorCode != CHANNEL_MESSAGE_ERR_CODE.CHANNEL_MESSAGE_ERR_OK)
            {
                $"[MessageManager] Error sending message - {errorCode}".LogError();
                groupManager.AddTextToDisplay("Error: message not sent", ChatMessage.MessageType.Error);
            }
        }

        private void OnConnectionStateChangedHandler(int id, CONNECTION_STATE state, CONNECTION_CHANGE_REASON reason)
        {
            if (state == CONNECTION_STATE.CONNECTION_STATE_RECONNECTING || state == CONNECTION_STATE.CONNECTION_STATE_CONNECTING)
            {
                groupManager.ToggleInputAvailability(false);
                groupManager.AddTextToDisplay("Connecting to text chat...", ChatMessage.MessageType.Info);
                return;
            }

            if (state == CONNECTION_STATE.CONNECTION_STATE_DISCONNECTED)
            {
                $"[MessageManager] Disconnected - {reason}".LogWarning();
                groupManager.ToggleInputAvailability(false);
                groupManager.AddTextToDisplay("Disconnected from text chat.", ChatMessage.MessageType.Info);
                return;
            }

            if (state == CONNECTION_STATE.CONNECTION_STATE_ABORTED) //shouldn't happen, but adding in as failsafe
            {
                $"[MessageManager] Aborted - {reason}".LogError();
                groupManager.ToggleInputAvailability(false);
                groupManager.AddTextToDisplay("Connecting to text chat...", ChatMessage.MessageType.Info);
                Logout();
                StartCoroutine(WaitThenLogin()); //ensure logged-out before logging in
                return;
            }

            groupManager.ToggleInputAvailability(true);
        }

    #endregion

        private void SendMessageAnalytics()
        {
            var currentRoom = ConnectionManager.Client.CurrentRoom;

            if (!currentRoom.CustomProperties.TryGetValue(Constants.ROOM_DETAILS_PROP_KEY, out var roomDetailsObj))
            {
                "Send Analytics: Failed to parse room details.".LogError();
                return;
            }

            var userProfile = UserProfileLocalDataManager.Instance.GetLocalUserProfile();
            var roomDetals = JsonConvert.DeserializeObject<RoomDetails>(roomDetailsObj.ToString());

            var playerId = userProfile.accountId;
            var playerLevel = 0;
            var hangoutId = roomDetals.roomName;
            var sessionId = currentRoom.Name;
            var maxVisitorsCount = currentRoom.PlayerCount - 1; // Self excluded
            var uniqueVisitorsCount = uniquePlayerAccountIds.Count; // Self excluded

            var sendMessageEvent = new SendMessageEvent
            (
                    playerId,
                    playerLevel,
                    hangoutId,
                    sessionId,
                    maxVisitorsCount,
                    uniqueVisitorsCount
            );

            GlobalNotifier.Instance.Trigger(sendMessageEvent);
        }

        private IEnumerator WaitThenLogin()
        {
            yield return new WaitForSeconds(3f);
            Login();
        }

        private void OnPlayerInstantiated(IEvent<string> callback)
        {
            var playerInstantiatedEvent = (QuantumPlayerJoinedEvent)callback;

            if (rtmClient == null && playerInstantiatedEvent.IsLocal)
            {
                groupManager.AddTextToDisplay("Unity Editor on Mac is not supported. Text chat will be disabled.", ChatMessage.MessageType.Error);
                groupManager.ToggleInputAvailability(false);
                return;
            }

            if (playerInstantiatedEvent.IsLocal)
            {
                username = playerInstantiatedEvent.Username;
                playerId = playerInstantiatedEvent.AccountId;
                groupManager.ToggleInputAvailability(true);
                Login();
            }
            else
            {
                uniquePlayerAccountIds.Add(playerInstantiatedEvent.AccountId);
            }

            if (playerMapping.ContainsKey(playerInstantiatedEvent.Username))
            {
                return;
            }

            var tcm = new TextChatMapping
            {
                entityRef = playerInstantiatedEvent.PlayerEntity,
                username = playerInstantiatedEvent.Username,
                nickname = playerInstantiatedEvent.Nickname,
                accountId = playerInstantiatedEvent.AccountId
            };

            playerMapping.Add(playerInstantiatedEvent.Username, tcm);
            $"{tcm.username} added to mapping".Log();
        }

        private void OnPlayerRemoved(IEvent<string> callback)
        {
            var playerRemovedEvent = (QuantumPlayerRemovedEvent)callback;

            if (playerMapping.ContainsKey(playerRemovedEvent.Username) && playerRemovedEvent.Username == username)
            {
                Logout();
            }
        }

        private string GetDisplayName(string userID, bool isNickname)
        {
            var foundName = "";

            foreach (var tcm in playerMapping.Values)
            {
                if (tcm.username == userID)
                {
                    foundName = isNickname ? tcm.nickname : tcm.username;
                }
            }

            return string.IsNullOrEmpty(foundName) ? userID : foundName;
        }

        private string GetPlayerId(string userId)
        {
            var result = string.Empty;

            foreach (var tcm in playerMapping.Values)
            {
                if (string.Compare(tcm.username, userId, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    result = tcm.accountId;
                }
            }

            return string.IsNullOrEmpty(result) ? userId : result;
        }

        private void Start()
        {
#if !UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
            clientEventHandler = new RtmClientEventHandler();
            channelEventHandler = new RtmChannelEventHandler();

#if AGORA_RELEASE
            rtmClient = new RtmClient(agoraAppConfig.ReleaseAppID, clientEventHandler);
#else
            rtmClient = new RtmClient(agoraAppConfig.DevAppID, clientEventHandler);
#endif
            clientEventHandler.OnLoginSuccess = OnClientLoginSuccessHandler;
            clientEventHandler.OnLoginFailure = OnClientLoginFailureHandler;
            clientEventHandler.OnConnectionStateChanged = OnConnectionStateChangedHandler;

            channelEventHandler.OnJoinSuccess = OnJoinSuccessHandler;
            channelEventHandler.OnJoinFailure = OnJoinFailureHandler;
            channelEventHandler.OnLeave = OnLeaveHandler;
            channelEventHandler.OnMessageReceived = OnChannelMessageReceivedHandler;
            channelEventHandler.OnMemberJoined = OnMemberJoinedHandler;
            channelEventHandler.OnMemberLeft = OnMemberLeftHandler;
            channelEventHandler.OnSendMessageResult = OnSendMessageResultHandler;
#endif
        }

        private void Awake()
        {
            playerMapping = new SerializableDictionary<string, TextChatMapping>();

            eventSlot = new Slot<string>(GlobalNotifier.Instance);
            eventSlot.SubscribeOn(QuantumPlayerJoinedEvent.EVENT_NAME, OnPlayerInstantiated);
            eventSlot.SubscribeOn(QuantumPlayerRemovedEvent.EVENT_NAME, OnPlayerRemoved);
        }

        private void OnDestroy()
        {
            eventSlot.Dispose();

            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }

            if (rtmClient != null)
            {
                rtmClient.Dispose();
                rtmClient = null;
            }
        }

        private void OnApplicationQuit()
        {
            LeaveChannel();
        }

        [Serializable]
        public struct TextChatMapping
        {
            public EntityRef entityRef;
            public string username;
            public string nickname;
            public string accountId;
        }
    }
}

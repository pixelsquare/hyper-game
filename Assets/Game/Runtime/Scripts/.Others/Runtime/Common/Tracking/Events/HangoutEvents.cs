using Kumu.Kulitan.Events;
using Kumu.Kulitan.Multiplayer;
using Photon.Realtime;

namespace Kumu.Kulitan.Tracking
{
    public class StartHangoutEvent : Event<string>
    {
        public const string EVENT_ID = "StartHangout";

        /// <summary>
        /// Invoked when you host a hangout.
        /// </summary>
        public StartHangoutEvent(string playerId, Room room) : base(EVENT_ID)
        {
            var roomDetails = room.GetRoomDetails();

            PlayerId = playerId;
            PlayerLevel = 0; // TODO: Use actual player level
            HangoutId = roomDetails.layoutName;
            SessionId = roomDetails.roomId;
            MaxPlayers = room.MaxPlayers;
            IsPublic = room.IsVisible ? 1 : 0;
            HasPassword = 0; // TODO: Use actual room property 
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int MaxPlayers { get; }
        public int IsPublic { get; }
        public int HasPassword { get; } 
    }
    
    /// <summary>
    /// Invoked when you join a hangout.
    /// </summary>
    public class JoinHangoutEvent : Event<string>
    {
        public const string EVENT_ID = "JoinHangout";

        public JoinHangoutEvent(string playerId, Room room) : base(EVENT_ID)
        {
            var roomDetails = room.GetRoomDetails();
            var hostDetails = room.GetHostDetails();
            
            PlayerId = playerId;
            HostId = hostDetails.accountId;
            PlayerLevel = 0; // TODO: Use actual player level
            HangoutId = roomDetails.layoutName;
            SessionId = roomDetails.roomId;
            MaxPlayers = room.MaxPlayers;
            IsPublic = room.IsVisible ? 1 : 0;
            HasPassword = 0; // TODO: Use actual room property
        }
        
        public string PlayerId { get; }
        public string HostId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int MaxPlayers { get; }
        public int IsPublic { get; }
        public int HasPassword { get; } 
    }

    /// <summary>
    /// Invoked when you leave a hangout you hosted or joined as a guest. Invoked before <see cref="EndHangoutEvent"/> or <see cref="ExitHangoutEvent"/>.
    /// </summary>
    public class LeaveHangoutEvent : Event<string>
    {
        public const string EVENT_ID = "LeaveHangout";

        public LeaveHangoutEvent() : base(EVENT_ID)
        {
            // empty
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int RoomDuration { get; }
    }

    /// <summary>
    /// Invoked when you leave a hangout you hosted.
    /// </summary>
    public class EndHangoutEvent : Event<string>
    {
        public const string EVENT_ID = "EndHangout";

        public EndHangoutEvent(string playerId, EndHangoutDetails hangoutDetails) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = 0; // TODO - use actual player level
            HangoutId = hangoutDetails.hangoutId;
            SessionId = hangoutDetails.sessionId;
            RoomDuration = (int) hangoutDetails.roomDuration;
            TopGifterFirst = hangoutDetails.topGifters[0];
            TopGifterSecond = hangoutDetails.topGifters[1];
            TopGifterThird = hangoutDetails.topGifters[2];
            DiamondsReceived = hangoutDetails.diamondsReceived;
            FollowersGained = hangoutDetails.followersGained;
            MaxVisitorsCount = hangoutDetails.maxVisitorsCount;
            UniqueVisitorsCount = hangoutDetails.uniqueVisitorsCount;
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int RoomDuration { get; }
        public string TopGifterFirst { get; }
        public string TopGifterSecond { get; }
        public string TopGifterThird { get; }
        public int DiamondsReceived { get; }
        public int FollowersGained { get; }
        public int MaxVisitorsCount { get; }
        public int UniqueVisitorsCount { get; }
    }
    
    /// <summary>
    /// Invoked when you leave a hangout you joined as a guest.
    /// </summary>
    public class ExitHangoutEvent : Event<string>
    {
        public const string EVENT_ID = "ExitHangout";

        public ExitHangoutEvent(string playerId, ExitHangoutDetails hangoutDetails) : base(EVENT_ID)
        {
            PlayerId = playerId;
            PlayerLevel = 0; // TODO - use actual player level
            HangoutId = hangoutDetails.hangoutId;
            SessionId = hangoutDetails.sessionId;
            RoomDuration = (int) hangoutDetails.roomDuration;
            DiamondsReceived = hangoutDetails.diamondsReceived;
            FollowersGained = hangoutDetails.followersGained;
            TextChatCount = hangoutDetails.textChatCount;
            VoiceChatDuration = (int) hangoutDetails.voiceChatDuration;
            InteractiveObjectsCount = hangoutDetails.interactiveObjectsCount;
            EmotesSoloCount = hangoutDetails.emotesSoloCount;
            EmotesPairedCount = hangoutDetails.emotesPairedCount;
        }
        
        public string PlayerId { get; }
        public int PlayerLevel { get; }
        public string HangoutId { get; }
        public string SessionId { get; }
        public int RoomDuration { get; }
        public int DiamondsReceived { get; }
        public int FollowersGained { get; }
        public int TextChatCount { get; }
        public int VoiceChatDuration { get; }
        public int InteractiveObjectsCount { get; }
        public int EmotesSoloCount { get; }
        public int EmotesPairedCount { get; }
    }

    public struct EndHangoutDetails
    {
        public readonly string hangoutId;
        public readonly string sessionId;
        public readonly float roomDuration;
        public readonly int diamondsReceived;
        public readonly int followersGained;
        /// <summary>
        /// Guaranteed to be length 3 always. Placement will be "null" if gifters are less than 3.
        /// </summary>
        public readonly string[] topGifters;
        public readonly int maxVisitorsCount;
        public readonly int uniqueVisitorsCount;

        public EndHangoutDetails(string hangoutId, string sessionId, float roomDuration, int diamondsReceived, int followersGained, string[] topGifters, int maxVisitorsCount, int uniqueVisitorsCount)
        {
            this.hangoutId = hangoutId;
            this.sessionId = sessionId;
            this.roomDuration = roomDuration;
            this.diamondsReceived = diamondsReceived;
            this.followersGained = followersGained;
            this.topGifters = topGifters;
            this.maxVisitorsCount = maxVisitorsCount;
            this.uniqueVisitorsCount = uniqueVisitorsCount;
        }
    }
    
    public struct ExitHangoutDetails
    {
        public readonly string hangoutId;
        public readonly string sessionId;
        public readonly float roomDuration;
        public readonly int diamondsReceived;
        public readonly int followersGained;
        public readonly int textChatCount;
        public readonly float voiceChatDuration;
        public readonly int interactiveObjectsCount;
        public readonly int emotesSoloCount;
        public readonly int emotesPairedCount;

        public ExitHangoutDetails(string hangoutId, string sessionId, float roomDuration, int diamondsReceived, int followersGained, int textChatCount, float voiceChatDuration, int interactiveObjectsCount, int emotesSoloCount, int emotesPairedCount)
        {
            this.hangoutId = hangoutId;
            this.sessionId = sessionId;
            this.roomDuration = roomDuration;
            this.diamondsReceived = diamondsReceived;
            this.followersGained = followersGained;
            this.textChatCount = textChatCount;
            this.voiceChatDuration = voiceChatDuration;
            this.interactiveObjectsCount = interactiveObjectsCount;
            this.emotesSoloCount = emotesSoloCount;
            this.emotesPairedCount = emotesPairedCount;
        }
    }
    
    public interface IStartHangoutHandle
    {
        public void OnStartHangoutEvent(StartHangoutEvent eventData);
    }

    public interface IJoinHangoutHandle
    {
        public void OnJoinHangoutEvent(JoinHangoutEvent eventData);
    }

    public interface IEndHangoutHandle
    {
        public void OnEndHangoutEvent(EndHangoutEvent eventData);
    }

    public interface IExitHangoutHandle
    {
        public void OnExitHangoutEvent(ExitHangoutEvent eventData);
    }
}

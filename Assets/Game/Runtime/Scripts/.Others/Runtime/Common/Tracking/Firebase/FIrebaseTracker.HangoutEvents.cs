using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IJoinHangoutHandle
    {
        public void OnJoinHangoutEvent(JoinHangoutEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("host_id", eventData.HostId),
                new("player_level", eventData.PlayerLevel),
                new("hangout_id", eventData.HangoutId),
                new("session_id", eventData.SessionId),
                new("max_players", eventData.MaxPlayers),
                new("is_public", eventData.IsPublic),
                new("has_password", eventData.HasPassword),
            };
            
            FirebaseAnalytics.LogEvent("join_hangout", parameters);
        }
    }

    public partial class FirebaseTracker : IStartHangoutHandle
    {
        public void OnStartHangoutEvent(StartHangoutEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("hangout_id", eventData.HangoutId),
                new("session_id", eventData.SessionId),
                new("max_players", eventData.MaxPlayers),
                new("is_public", eventData.IsPublic),
                new("has_password", eventData.HasPassword),
            };
            
            FirebaseAnalytics.LogEvent("start_hangout", parameters);
        }
    }

    public partial class FirebaseTracker : IEndHangoutHandle
    {
        public void OnEndHangoutEvent(EndHangoutEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new ("player_id", eventData.PlayerId),
                new ("player_level", eventData.PlayerLevel),
                new ("hangout_id", eventData.HangoutId),
                new ("session_id", eventData.SessionId),
                new ("duration", eventData.RoomDuration),
                new ("top_gifter_first", eventData.TopGifterFirst),
                new ("top_gifter_second", eventData.TopGifterSecond),
                new ("top_gifter_third", eventData.TopGifterThird),
                new ("diamonds_received", eventData.DiamondsReceived),
                new ("followers_gained", eventData.FollowersGained),
                new ("max_visitors_count", eventData.MaxVisitorsCount),
                new ("unique_visitors_count", eventData.UniqueVisitorsCount),
            };
            
            FirebaseAnalytics.LogEvent("end_hangout", parameters);
        }
    }

    public partial class FirebaseTracker : IExitHangoutHandle
    {
        public void OnExitHangoutEvent(ExitHangoutEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new ("player_id", eventData.PlayerId),
                new ("player_level", eventData.PlayerLevel),
                new ("hangout_id", eventData.HangoutId),
                new ("session_id", eventData.SessionId),
                new ("duration", eventData.RoomDuration),
                new ("diamonds_received", eventData.DiamondsReceived),
                new ("followers_gained", eventData.FollowersGained),
                new ("text_chat_count", eventData.TextChatCount),
                new ("voice_chat_duration", eventData.VoiceChatDuration),
                new ("interactive_objects_count", eventData.InteractiveObjectsCount),
                new ("emotes_solo_count", eventData.EmotesSoloCount),
                new ("emotes_paired_count", eventData.EmotesPairedCount),
            };
            
            FirebaseAnalytics.LogEvent("exit_hangout", parameters);
        }
    }
}

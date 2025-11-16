using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IStartHangoutHandle
    {
        public void OnStartHangoutEvent(StartHangoutEvent eventData)
        {
            var logMessage = $"<color=#{HEX}>start_hangout event</color>\n" +
                             $"PlayerId: {eventData.PlayerId}\n" +
                             $"PlayerLevel: {eventData.PlayerLevel}\n" +
                             $"HangoutId: {eventData.HangoutId}\n" +
                             $"SessionId: {eventData.SessionId}\n" +
                             $"MaxPlayers: {eventData.MaxPlayers}\n" +
                             $"IsPublic: {eventData.IsPublic}\n" +
                             $"HasPassword: {eventData.HasPassword}"; 
            logMessage.Log();
        }
    }

    public partial class DebugTracker : IJoinHangoutHandle
    {
        public void OnJoinHangoutEvent(JoinHangoutEvent eventData)
        {
            var logMessage = $"<color=#{HEX}>join_hangout event</color>\n" +
                             $"PlayerId: {eventData.PlayerId}\n" +
                             $"HostId: {eventData.HostId}\n" +
                             $"PlayerLevel: {eventData.PlayerLevel}\n" +
                             $"HangoutId: {eventData.HangoutId}\n" +
                             $"SessionId: {eventData.SessionId}\n" +
                             $"MaxPlayers: {eventData.MaxPlayers}\n" +
                             $"IsPublic: {eventData.IsPublic}\n" +
                             $"HasPassword: {eventData.HasPassword}"; 
            logMessage.Log();
        }
    }

    public partial class DebugTracker : IEndHangoutHandle
    {
        public void OnEndHangoutEvent(EndHangoutEvent eventData)
        {
            var logMessage = $"<color=#{HEX}>end_hangout event</color>\n" +
                             $"PlayerId: {eventData.PlayerId}\n" +
                             $"PlayerLevel: {eventData.PlayerLevel}\n" +
                             $"HangoutId: {eventData.HangoutId}\n" +
                             $"SessionId: {eventData.SessionId}\n" +
                             $"Duration: {eventData.RoomDuration}\n" +
                             $"TopGifterFirst: {eventData.TopGifterFirst}\n" +
                             $"TopGifterSecond: {eventData.TopGifterSecond}\n" +
                             $"TopGifterThird: {eventData.TopGifterThird}\n" +
                             $"DiamondsReceived: {eventData.DiamondsReceived}\n" +
                             $"FollowersGained: {eventData.FollowersGained}\n" +
                             $"MaxVistorsCount: {eventData.MaxVisitorsCount}\n" +
                             $"UniqueVisitorsCount: {eventData.UniqueVisitorsCount}";
            logMessage.Log();
        }
    }

    public partial class DebugTracker : IExitHangoutHandle
    {
        public void OnExitHangoutEvent(ExitHangoutEvent eventData)
        {
            var logMessage = $"<color=#{HEX}>exit_hangout event</color>\n" +
                             $"PlayerId: {eventData.PlayerId}\n" +
                             $"PlayerLevel: {eventData.PlayerLevel}\n" +
                             $"HangoutId: {eventData.HangoutId}\n" +
                             $"SessionId: {eventData.SessionId}\n" +
                             $"Duration: {eventData.RoomDuration}\n" +
                             $"DiamondsReceived: {eventData.DiamondsReceived}\n" +
                             $"FollowersGained: {eventData.FollowersGained}\n" +
                             $"TextChatCount: {eventData.TextChatCount}\n" +
                             $"VoiceChatDuration: {eventData.VoiceChatDuration}\n" +
                             $"InteractiveObjectsCount: {eventData.InteractiveObjectsCount}\n" +
                             $"EmotesSoloCount: {eventData.EmotesSoloCount}\n" +
                             $"EmotesPairedCount: {eventData.EmotesPairedCount}";
            
            logMessage.Log();
        }
    }
}

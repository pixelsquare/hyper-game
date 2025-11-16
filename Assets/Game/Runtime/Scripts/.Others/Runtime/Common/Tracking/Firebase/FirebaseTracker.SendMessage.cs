using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : ISendMessageHandle
    {
        public void OnSendMessageEvent(SendMessageEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("hangout_id", eventData.HangoutId),
                new("session_id", eventData.SessionId),
                new("max_visitors_count", eventData.MaxVisitorsCount),
                new("unique_visitors_count", eventData.UniqueVisitorsCount)
            };

            FirebaseAnalytics.LogEvent("send_message", parameters);
        }
    }
}

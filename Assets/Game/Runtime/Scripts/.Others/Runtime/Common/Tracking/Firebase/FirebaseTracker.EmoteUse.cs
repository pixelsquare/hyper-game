using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IEmoteUseHandle
    {
        public void OnEmoteUseEvent(EmoteUseEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("emote_id", eventData.EmoteId),
            };
            
            FirebaseAnalytics.LogEvent("emote_used", parameters);
        }
    }
}

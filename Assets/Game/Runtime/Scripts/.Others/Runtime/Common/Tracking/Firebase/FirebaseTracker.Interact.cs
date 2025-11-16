using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IInteractHandle
    {
        public void OnInteractEvent(InteractEvent eventData)
        {
            var parameters = new Parameter[]
            {
                new("player_id", eventData.PlayerId),
                new("player_level", eventData.PlayerLevel),
                new("hangout_id", eventData.HangoutId),
                new("interactive_object_id", eventData.InteractiveObjectId),
            };
            
            FirebaseAnalytics.LogEvent("interactive_object_used", parameters);
        }
    }
}

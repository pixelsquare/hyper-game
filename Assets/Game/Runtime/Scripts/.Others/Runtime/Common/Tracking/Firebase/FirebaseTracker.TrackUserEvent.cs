using Firebase.Analytics;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : ITrackUserHandle
    {
        public void OnTrackUserEvent(TrackUserEvent eventData)
        {
            FirebaseAnalytics.SetUserId(eventData.UserId);
        }
    }
}

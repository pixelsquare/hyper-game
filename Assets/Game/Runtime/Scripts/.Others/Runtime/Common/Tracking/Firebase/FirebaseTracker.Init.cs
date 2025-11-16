using Firebase.Analytics;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IEventsTrackerInit
    {
        public void Init(Slot<string> eventSlot)
        {
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
        }
    }
}

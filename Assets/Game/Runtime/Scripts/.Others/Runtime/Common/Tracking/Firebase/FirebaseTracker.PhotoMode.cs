using System;
using Firebase.Analytics;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class FirebaseTracker : IPhotoModeHandle
    {
        private long photomodeTimestamp;
        private int photosTakenCount;
        
        public void OnPhotoModeStart(IEvent<string> eventData)
        {
            photomodeTimestamp = DateTime.Now.Ticks;
            photosTakenCount = 0;
        }

        public void OnPhotoTaken(IEvent<string> eventData)
        {
            photosTakenCount++;
        }

        public void OnPhotoModeEnd(IEvent<string> eventData)
        {
            var now = DateTime.Now.Ticks;
            var duration = new TimeSpan(now - photomodeTimestamp);

            var parameters = new Parameter[]
            {
                new("player_id", ""),
                new("player_level", 1),
                new("duration", duration.Seconds),
                new("photos_taken_count", photosTakenCount),
            };
            
            FirebaseAnalytics.LogEvent("photomode_end", parameters);            
        }
    }
}

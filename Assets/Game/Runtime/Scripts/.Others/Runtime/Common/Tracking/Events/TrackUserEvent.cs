using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public class TrackUserEvent : Event<string>
    {
        public const string EVENT_ID = "TrackUserEvent";
        
        public string UserId { get; }
        
        public TrackUserEvent(string userId) : base(EVENT_ID)
        {
            UserId = userId;
        }
    }

    public interface ITrackUserHandle
    {
        public void OnTrackUserEvent(TrackUserEvent eventData);
    }
}

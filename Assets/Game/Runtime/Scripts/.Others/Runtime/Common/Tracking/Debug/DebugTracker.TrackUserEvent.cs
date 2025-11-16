using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : ITrackUserHandle
    {
        public void OnTrackUserEvent(TrackUserEvent eventData)
        {
            $"<color=#{HEX}>tracking user id {eventData.UserId}</color>".Log();
        }
    }
}

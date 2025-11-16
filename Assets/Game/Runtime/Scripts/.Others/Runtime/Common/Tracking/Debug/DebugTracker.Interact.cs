using Kumu.Extensions;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IInteractHandle
    {
        public void OnInteractEvent(InteractEvent eventData)
        {
            $"<color=#{HEX}>{eventData.PlayerId} interacted {eventData.InteractiveObjectId} in {eventData.HangoutId}</color>".Log();
        }
    }
}

using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public partial class DebugTracker : IEventsTrackerInit
    {
        private const string HEX = "ff99cc";
        
        public void Init(Slot<string> eventSlot)
        {
        }
    }
}

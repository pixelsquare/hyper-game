using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Tracking
{
    public interface IEventsTrackerInit
    {
        public void Init(Slot<string> eventSlot);
    }
}

using System;

namespace Kumu.Kulitan.Events
{
    public class Subscription<EventId> : ISubscription<EventId> where EventId : IComparable
    {
        private EventId eventID;

        public Action<IEvent<EventId>> Action { get; set; }

        public EventId ID
        {
            get => eventID;
            set => eventID = value;
        }

        public IChannel<EventId> Channel { get; set; }

        public void Cancel()
        {
            if (Channel != null)
            {
                Channel.Remove(this);
                Action = null;
                Channel = null;
            }
        }

        public bool Invoke(IEvent<EventId> evt)
        {
            if (Action == null || eventID.CompareTo(evt.ID) != 0)
            {
                return false;
            }
            Action(evt);
            return true;
        }

        public void Dispose()
        {
            Cancel();
        }
    }
}

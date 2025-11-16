using System;

namespace Kumu.Kulitan.Events
{
    public interface ISubscription<EventID> : IDisposable
    {
        Action<IEvent<EventID>> Action { get; set; }
        EventID ID { get; set; }
        IChannel<EventID> Channel { get; set; }
        bool Invoke(IEvent<EventID> evt);
        void Cancel();
    }
}

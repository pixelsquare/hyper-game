using System;

namespace Kumu.Kulitan.Events
{
    public interface IChannel<EventID>
    {
        ISubscription<EventID> Subscribe(Action<IEvent<EventID>> listener);
        void UnSubscribe(Action<IEvent<EventID>> listener);
        void UnSubscribeAll();
        void Trigger(IEvent<EventID> evt);
        void Remove(ISubscription<EventID> subscription);
    }
}

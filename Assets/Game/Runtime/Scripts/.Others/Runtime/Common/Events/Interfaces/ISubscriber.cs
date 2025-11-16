using System;

namespace Kumu.Kulitan.Events
{
    public interface ISubscriber<EventID>
    {
        ISubscription<EventID> SubscribeOn(EventID evt, Action<IEvent<EventID>> listener);
        void UnSubscribeFor(EventID evt, Action<IEvent<EventID>> listener);
        void UnSubscribeAllOn(EventID evt);
    }
}

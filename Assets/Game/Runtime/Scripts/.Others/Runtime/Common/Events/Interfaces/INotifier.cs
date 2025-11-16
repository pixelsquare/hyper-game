namespace Kumu.Kulitan.Events
{
    public interface INotifier<EventID> : ISubscriber<EventID>
    {
        void Trigger(EventID evtKey);
        void Trigger(IEvent<EventID> evt);
    }
}

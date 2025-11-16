namespace Kumu.Kulitan.Events
{
    public class Event<EventID> : IEvent<EventID>
    {
        public Event(EventID id)
        {
            this.ID = id;
        }
        
        public EventID ID { get; set; }
        public object UserData { get; set; }
    }
}

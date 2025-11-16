namespace Kumu.Kulitan.Events
{
    public interface IEvent<EventID>
    {
        EventID ID { get; }
        object UserData { get; set; }
    }
}

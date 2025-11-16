using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Backend
{
    public class RemoteDataNotificationReceivedEvent : Event<string>
    {
        public const string EVENT_NAME = "RemoteDataNotificationReceivedEvent";

        public IRemoteDataNotification Message { get; }
        
        public RemoteDataNotificationReceivedEvent(IRemoteDataNotification message) : base(EVENT_NAME)
        {
            Message = message;
        }
    }
}

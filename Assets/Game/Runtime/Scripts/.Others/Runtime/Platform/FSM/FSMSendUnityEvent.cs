using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Common
{
    public class FSMSendUnityEvent : Event<string>
    {
        public const string EVENT_NAME = "FSMSendUnityEvent";

        public FSMSendUnityEvent(string eventName) : base(EVENT_NAME)
        {
            EventName = eventName;
        }

        public string EventName { get; }
    }
}

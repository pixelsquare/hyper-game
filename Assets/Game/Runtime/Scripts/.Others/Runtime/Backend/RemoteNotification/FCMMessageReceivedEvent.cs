using System.Collections.Generic;
using Firebase.Messaging;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Backend
{
    public class FCMMessageReceivedEvent : Event<string> // TODO (cj): remove this
    {
        public const string EVENT_NAME = "FCMMessageReceivedEvent";

        public FCMMessageReceivedEvent(string from, string error, FirebaseMessage message) : base(EVENT_NAME)
        {
            From = from;
            Error = error;
            Message = message;
        }

        public string From { get; }
        public string Error { get; }
        public FirebaseMessage Message { get; }
    }
}

using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.Hangout
{
    public class ObjectInteractedEvent : Event<string>
    {
        public const string EVENT_NAME = "ObjectInteractedEvent";

        public ObjectInteractedEvent(GameObject interactedObject) : base(EVENT_NAME)
        {
            InteractedObject = interactedObject;
        }

        public GameObject InteractedObject { get; }
    }
}

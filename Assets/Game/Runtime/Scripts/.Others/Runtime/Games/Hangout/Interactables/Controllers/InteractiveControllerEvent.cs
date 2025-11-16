using Kumu.Kulitan.Events;
using Quantum;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveControllerEvent : Event<string>
    {
        public const string EVENT_NAME = "OnControllerInteract";

        public bool isInteracting;
        public EntityRef slotEntityRef;

        public InteractiveControllerEvent() : base(EVENT_NAME)
        {
        }
    }

    public class InteractiveControllerEventLocal : Event<string>
    {
        public const string EVENT_NAME = "OnControllerInteractLocal";

        public bool isInteracting;
        public EntityRef slotEntityRef;

        public InteractiveControllerEventLocal() : base(EVENT_NAME)
        {
        }
    }
}

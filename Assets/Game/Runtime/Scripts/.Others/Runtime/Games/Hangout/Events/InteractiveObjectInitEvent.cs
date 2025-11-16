using Kumu.Kulitan.Events;
using Quantum;

namespace Kumu.Kulitan.Hangout
{
    public class InteractiveObjectInitEvent : Event<string>
    {
        public const string EVENT_NAME = "InteractiveObjectInitEvent";
        
        public InteractiveObjectInitEvent(EntityRef entity, IInteractiveObject interactiveObject) : base(EVENT_NAME)
        {
            EntityRef = entity;
            InteractiveObject = interactiveObject;
        }
        
        public EntityRef EntityRef { get; }
        public IInteractiveObject InteractiveObject { get; }
    }
}

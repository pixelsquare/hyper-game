using Kumu.Kulitan.Events;
using Quantum;

namespace Kumu.Kulitan.Hangout
{
    public class PlayerNearInteractiveObjEvent : Event<string>
    {
        public const string EVENT_NAME = "PlayerNearInteractiveObjEvent";
        
        public PlayerNearInteractiveObjEvent(EntityRef entityRef, bool inProximity) : base(EVENT_NAME)
        {
            EntityRef = entityRef;
            IsInProximity = inProximity;
        }

        public EntityRef EntityRef { get;  }
        public bool IsInProximity { get;  }
    }
}

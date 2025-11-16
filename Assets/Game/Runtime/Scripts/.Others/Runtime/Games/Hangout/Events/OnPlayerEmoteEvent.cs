using Kumu.Kulitan.Events;
using Quantum;

namespace Kumu.Kulitan.Hangout
{
    public class OnPlayerEmoteEvent : Event<string>
    {
        public const string EVENT_NAME = "OnPlayerEmoteEvent";

        public OnPlayerEmoteEvent(EntityRef entityRef, AnimationDataAsset animDataAsset, bool isEmoting) : base(EVENT_NAME)
        {
            EntityRef = entityRef;
            AnimDataAsset = animDataAsset;
            IsEmoting = isEmoting;
        }

        public EntityRef EntityRef { get; }
        public AnimationDataAsset AnimDataAsset { get; }
        public bool IsEmoting { get; }
    }
}

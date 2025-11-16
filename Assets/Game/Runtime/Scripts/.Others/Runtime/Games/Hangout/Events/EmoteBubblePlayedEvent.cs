using Quantum;
using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Hangout
{
    public class EmoteBubblePlayedEvent : Event<string>
    {
        public const string EVENT_NAME = "EmoteBubblePlayedEvent";

        public EmoteBubblePlayedEvent(EntityRef entityRef, AnimationDataAsset animDataAsset) : base(EVENT_NAME)
        {
            EntityRef = entityRef;
            AnimDataAsset = animDataAsset;
        }

        public EntityRef EntityRef { get; }
        public AnimationDataAsset AnimDataAsset { get; }
    }
}

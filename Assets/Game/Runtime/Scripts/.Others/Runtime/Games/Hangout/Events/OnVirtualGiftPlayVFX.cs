using Kumu.Kulitan.Events;

namespace Kumu.Kulitan.Gifting
{
    public class OnVirtualGiftPlayVFX : Event<string>
    {
        public const string EVENT_NAME = "OnVirtualGiftPlayVFX";

        public enum VFXEventActorType
        {
            Gifter,
            Giftee
        }

        public OnVirtualGiftPlayVFX(string accountId, VFXEventActorType actorType, VgEffectData vgEffectData) : base(EVENT_NAME)
        {
            AccountId = accountId;
            ActorType = actorType;
            VgEffectData = vgEffectData;
        }
        
        public string AccountId { get; }
        public VFXEventActorType ActorType { get; }
        public VgEffectData VgEffectData { get;  }
    }
}

using Kumu.Kulitan.Events;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class OnVirtualGiftPlaySFX : Event<string>
    {
        public const string EVENT_NAME = "OnVirtualGiftPlaySFX";

        public enum SFXEventActorType
        {
            Gifter,
            Giftee
        }

        public OnVirtualGiftPlaySFX(string accountId, SFXEventActorType actorType, 
            VirtualGiftConfig.SFXClipType sfxType, AudioClip sfxClip) : base(EVENT_NAME)
        {
            AccountId = accountId;
            ActorType = actorType;
            SFXType = sfxType;
            SFXClip = sfxClip;
        }
        
        public string AccountId { get; }
        public SFXEventActorType ActorType { get; }
        public VirtualGiftConfig.SFXClipType SFXType { get; }
        public AudioClip SFXClip { get; }
    }
}

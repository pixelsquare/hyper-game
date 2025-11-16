using System;
using Kumu.Kulitan.Backend;
using Kumu.Kulitan.Common;
using Kumu.Kulitan.Multiplayer;
using UnityEngine;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftEffectSystem : MonoBehaviour
    {
        private void OnVirtualGiftReceived(VirtualGiftEventInfo obj)
        {
            if (BaseMatchmakingHandler.State != BaseMatchmakingHandler.RoomStatus.IN_ROOM)
            {
                return;
            }
            
            foreach (var giftData in obj.gifts)
            {
                var vgGift = VirtualGiftDatabase.Current.GetGift(giftData.id);
                var count = giftData.quantity;

                var vfxData = new VGVFXData
                {
                    gifterVFXConfig = vgGift.GifterVFX,
                    gifteeVFXConfig = vgGift.GifteeVFX,
                };

                var sfxData = new VGSFXData
                {
                    gifterSFXClip = vgGift.GifterSFXClip,
                    gifteeSFXClip = vgGift.GifteeSFXClip,
                    gifterSFXType = vgGift.GifterSFXType,
                    gifteeSFXType = vgGift.GifteeSFXType
                };

                var effectData = new VgEffectData()
                {
                    giftId = vgGift.Data.giftId,
                    gifterId = obj.gifter,
                    giftType = vgGift.Data.giftType,
                    giftees = obj.giftees,
                    vfxData = vfxData,
                    sfxData = sfxData,
                    count = count,
                    priority = vgGift.Data.priority
                };
                
                TriggerVFX(effectData);
            }
        }

        private void TriggerVFX(VgEffectData vgEffectData)
        {
            //send event for giftee VFX
            foreach (var giftee in vgEffectData.giftees)
            {
                GlobalNotifier.Instance.Trigger(new OnVirtualGiftPlayVFX(giftee, 
                    OnVirtualGiftPlayVFX.VFXEventActorType.Giftee, vgEffectData));
            }
        }

        private void Start()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent += OnVirtualGiftReceived;
        }

        private void OnDestroy()
        {
            Services.VirtualGiftService.VirtualGiftReceivedEvent -= OnVirtualGiftReceived;
        }
    }

    [Serializable]
    public struct VGVFXData
    {
        public VirtualGiftVFXConfig gifterVFXConfig;
        public VirtualGiftVFXConfig gifteeVFXConfig;
    }
    
    [Serializable]
    public struct VGSFXData
    {
        public AudioClip gifterSFXClip;
        public AudioClip gifteeSFXClip;
        public VirtualGiftConfig.SFXClipType gifterSFXType;
        public VirtualGiftConfig.SFXClipType gifteeSFXType;
    }
    
    [Serializable]
    public struct VgEffectData
    {
        public string giftId;
        public string gifterId;
        public string[] giftees;
        public VirtualGiftData.VGType giftType;
        public VGVFXData vfxData;
        public VGSFXData sfxData;
        public int count;
        public int priority;
    }
}

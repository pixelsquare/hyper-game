using System.Collections.Generic;
using System.Linq;
using Kumu.Kulitan.Common;
using UnityEngine;
using Kumu.Kulitan.Backend;

namespace Kumu.Kulitan.Gifting
{
    public class VirtualGiftingEffectPoolController : MonoBehaviour
    {
        [Header("Effect Containers")]
        [SerializeField] private Transform localParent;
        [SerializeField] private Transform particleFXTransform;
        [Header("Aura VFX")]
        [SerializeField] private GameObject gifteeParticleFxPrefab;
        [SerializeField] private GameObject gifterParticleFxPrefab;
        private Dictionary<string, IVirtualGiftBasicVFXDisplay> vfxPool = new();
        private List<VgEffectData> vfxQueue = new(); 
        private string currentlyPlayingGiftId;
        private ParticleSystem[] auraVfxPool = new ParticleSystem[2];
        
        public Transform ParticleFXTransform => particleFXTransform;

        public void InitVfxPool()
        {
            var vgConfigs = VirtualGiftDatabase.Current.GiftConfigs;
            for (var i = 0; i < vgConfigs.Count; i++)
            {
                var vfxType = vgConfigs[i].GifteeVFX.vfxType;
                var vfxPrefab = vgConfigs[i].GifteeVFX.vfxPrefab;

                if (vfxType != VirtualGiftVFXConfig.VFXType.WorldSpace)
                {
                    continue;
                }

                var vfxId = vgConfigs[i].Data.giftId;
                var newVFX = Instantiate(vfxPrefab, localParent);
                var vfxDisplay = newVFX.GetComponent<IVirtualGiftBasicVFXDisplay>();
                vfxDisplay.OnCompleted += OnVfxCompleted;
                newVFX.transform.localPosition = Vector3.zero;
                newVFX.transform.localScale = Vector3.one;
                newVFX.SetActive(false);
                vfxPool.Add(vfxId, vfxDisplay);
            }

            PoolAuraVfx();
        }

        public void QueueVgEffect(VgEffectData vgEffectData)
        {
            vfxQueue.Add(vgEffectData);
            var sortedQueue = vfxQueue.OrderByDescending(data => data.priority).ToList();
            vfxQueue = sortedQueue;

            if (string.IsNullOrWhiteSpace(currentlyPlayingGiftId))
            {
                PlayNextVfx();
            }
        }

        private IVirtualGiftBasicVFXDisplay GetVfxDisplayFromPool(string giftId)
        {
            vfxPool.TryGetValue(giftId, out var vfxDisplay);
            return vfxDisplay;
        }
        
        private void PlayNextVfx()
        {
            var vgEffectData = vfxQueue[0];
            var vgVfxConfig = vgEffectData.vfxData.gifteeVFXConfig;
            var vfxDisplay = GetVfxDisplayFromPool(vgEffectData.giftId);
            
            currentlyPlayingGiftId = vgEffectData.giftId;
            // play vfx
            vfxDisplay.Lifetime = vgVfxConfig.duration;
            vfxDisplay.ShowVFX(vgEffectData.count);
            TriggerEffects(vgEffectData);
            vfxQueue.RemoveAt(0);
        }

        private void OnVfxCompleted()
        {
            currentlyPlayingGiftId = "";

            if (vfxQueue.Count <= 0)
            {
                return;
            }
            PlayNextVfx();
        }

        private void TriggerEffects(VgEffectData vgEffectData)
        {
            GlobalNotifier.Instance.Trigger(new OnVirtualGiftPlayEffectEvent(vgEffectData));
            // play sfx
            TriggerSfx(vgEffectData);
            // play aura vfx
            TriggerAuraVfx(vgEffectData);
        }

        private void TriggerSfx(VgEffectData vgEffectData)
        {
            GlobalNotifier.Instance.Trigger(new OnVirtualGiftPlaySFX(vgEffectData.gifterId, 
                OnVirtualGiftPlaySFX.SFXEventActorType.Giftee, vgEffectData.sfxData.gifteeSFXType, 
                vgEffectData.sfxData.gifteeSFXClip));
        }

        private void TriggerAuraVfx(VgEffectData vgEffectData)
        {
            if (IsGifterLocal(vgEffectData.gifterId))
            {
                auraVfxPool[1].gameObject.SetActive(true);
                return;
            }
            auraVfxPool[0].gameObject.SetActive(true);
        }

        private bool IsGifterLocal(string gifterId)
        {
            var localId = UserProfileLocalDataManager.Instance.GetLocalUserProfile().accountId;
            return gifterId.Equals(localId);
        }

        private void PoolAuraVfx()
        {
            var gifteeEffect = Instantiate(gifteeParticleFxPrefab, particleFXTransform).GetComponent<ParticleSystem>();
            var gifterEffect = Instantiate(gifterParticleFxPrefab, particleFXTransform).GetComponent<ParticleSystem>();
            auraVfxPool[0] = gifteeEffect;
            auraVfxPool[1] = gifterEffect;
            foreach (var auraVfx in auraVfxPool)
            {
                auraVfx.gameObject.SetActive(false);
            }
        }
    }
}
